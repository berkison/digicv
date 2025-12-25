using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Simple.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Simple.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager; // Identity kullanıyorsan
        private readonly IConfiguration _config; // appsettings'den Key çekmek için

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        public async Task<bool> RegisterAsync(UserDto request)
        {
            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if(result.Succeeded) 
            { 
                await _userManager.AddToRoleAsync(user, "User");
                return true;
            }

            return false;
        }
        public async Task<string> LoginAsync(UserDto request)
        {
            // 1. Kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null; // veya hata fırlat

            // 2. Şifre Kontrolü (Identity check)
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return null;

            // --- ROL İŞLEMİ BURADA YAPILIR ---
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
             

            // Rolleri token'a ekliyoruz
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3. Token Oluşturma (Senin mevcut kodun buraya gelecek)
            var token = GetToken(authClaims); // Alt tarafta private bir metod

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Token oluşturma kodlarını private metod olarak buraya saklayabilirsin
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
