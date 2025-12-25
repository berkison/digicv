using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple.Data;   // Burası SimpleContext'in olduğu yer
using Simple.Models; // Burası AppUser'ın olduğu yer
using System;
using Simple.DTOs;

namespace Simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        // DÜZELTME 1: AppDbContext yerine SimpleContext yazıldı
        private readonly SimpleContext _context;
        private readonly UserManager<AppUser> _userManager;

        // DÜZELTME 2: Constructor içinde de SimpleContext kullanıldı
        public PublicController(SimpleContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("GetPortfolio/{identifier}")]
        public async Task<IActionResult> GetPortfolio(string identifier)
        {
            // 1. Önce KULLANICI ADI ile ara (asfasfasf)
            var user = await _userManager.FindByNameAsync(identifier);

            // 2. Bulamazsan EMAIL ile ara (test@test.com)
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(identifier);
            }

            // 3. Hala yoksa 404 ver
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            // 4. Profil Gizli mi kontrol et
            if (user.IsProfilePublic == false)
            {
                return StatusCode(403, "Bu profil gizli.");
            }

            // 5. Verileri Paketle
            var portfolioData = new
            {
                User = new
                {
                    user.FullName,
                    user.JobTitle,
                    user.Bio,
                    user.ProfileImageUrl,
                    user.Email,
                    user.UserName,
                    user.IsProfilePublic
                },
                Projects = await _context.Projects.Where(p => p.UserId == user.Id).ToListAsync(),
                Skills = await _context.Skills.Where(p => p.UserId == user.Id).ToListAsync(),
                Experiences = await _context.Experiences.Where(p => p.UserId == user.Id).OrderByDescending(e => e.StartDate).ToListAsync(),
                SocialMedias = await _context.SocialMedias.Where(s => s.UserId == user.Id).ToListAsync()
            };

            return Ok(portfolioData);
        }
    }
}