using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Simple.DTOs;
using Simple.Models;
using Simple.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IImageService _imageService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _imageService = imageService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (string.IsNullOrEmpty(model.UserName)) return BadRequest("Kullanıcı adı zorunlu!");

            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null) return StatusCode(500, new { Message = "Bu kullanıcı adı dolu!" });

            AppUser user = new AppUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                FullName = model.UserName, // Varsayılan olarak kullanıcı adı
                IsProfilePublic = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return StatusCode(500, new { Message = "Kayıt başarısız", Errors = result.Errors });

            return Ok(new { Message = "Kayıt Başarılı!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Email veya şifre hatalı");

            // Kullanıcı rolünü güvenli bir şekilde al
            var userRoles = await _userManager.GetRolesAsync(user);
            var userRole = userRoles.FirstOrDefault();

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (userRole != null)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                role = userRole,
                userId = user.Id,
                email = user.Email,
                userName = user.UserName
            });
        }

        [Authorize]
        [HttpPost("toggle-privacy")]
        public async Task<IActionResult> TogglePrivacy()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            // Mevcut durumu tersine çevir
            if (user.IsProfilePublic == true)
                user.IsProfilePublic = false;
            else
                user.IsProfilePublic = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { isPublic = user.IsProfilePublic, message = "Gizlilik ayarı güncellendi" });
        }

        [Authorize]
        [HttpGet("profile-status")]
        public async Task<IActionResult> GetProfileStatus()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(new { isPublic = user.IsProfilePublic });
        }

        // ÖNEMLİ: Bu metot Navbar veya basit profil gösterimi için
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                FullName = user.FullName,
                JobTitle = user.JobTitle,
                Bio = user.Bio,
                ProfileImageUrl = user.ProfileImageUrl,
                // Yeni alanları buraya da ekledim ki her yerden erişilebilsin
                Languages = user.Languages,
                ReferenceName = user.ReferenceName,
                ReferenceContact = user.ReferenceContact
            });
        }

        // ÖNEMLİ: CV Builder ve Detaylı Profil Sayfası muhtemelen bunu kullanıyor
        [Authorize]
        [HttpGet("get-user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("User not found.");

            return Ok(new
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address ?? "",
                Bio = user.Bio,
                JobTitle = user.JobTitle,
                ProfileImageUrl = user.ProfileImageUrl,
                IsProfilePublic = user.IsProfilePublic,

                // BURASI EKSİKTİ, EKLENDİ:
                Languages = user.Languages,
                ReferenceName = user.ReferenceName,
                ReferenceContact = user.ReferenceContact,

                // Sosyal Medya (AppUser'da varsa)
                LinkedInUrl = user.LinkedInUrl,
                GitHubUrl = user.GitHubUrl,
                PersonalWebsiteUrl = user.PersonalWebsiteUrl
            });
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            // Temel Bilgiler
            user.FullName = model.FullName;
            user.JobTitle = model.JobTitle;
            user.Bio = model.Bio;

            // İletişim & Ek Bilgiler (DTO'dan gelen veriyi User'a yazıyoruz)
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;

            // Diller ve Referanslar
            user.Languages = model.Languages;
            user.ReferenceName = model.ReferenceName;
            user.ReferenceContact = model.ReferenceContact;

            // Sosyal Medya
            user.LinkedInUrl = model.LinkedInUrl;
            user.GitHubUrl = model.GitHubUrl;
            user.PersonalWebsiteUrl = model.PersonalWebsiteUrl;

            // Resim Yükleme
            if (model.ProfileImage != null)
            {
                var imagePath = await _imageService.SaveImageAsync(model.ProfileImage);
                user.ProfileImageUrl = imagePath;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Güncel halini geri dönüyoruz ki frontend state'i güncelleyebilsin
                return Ok(new
                {
                    message = "Profil güncellendi",
                    imageUrl = user.ProfileImageUrl,
                    languages = user.Languages,
                    references = user.ReferenceName
                });
            }

            return BadRequest(result.Errors);
        }
    }
}