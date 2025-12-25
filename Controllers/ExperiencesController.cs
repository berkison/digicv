using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple.Data;
using Simple.Models;
using System.Security.Claims;

namespace Simple.Controllers
{
    [Route("api/[controller]")] // Burası api/Experiences adresini oluşturur
    [ApiController]
    public class ExperiencesController : ControllerBase
    {
        private readonly SimpleContext _context;

        public ExperiencesController(SimpleContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Experience>>> GetExperiences()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bulunamadı.");
            }

            // DİKKAT: Servisteki metot ismi GetAllSkillsByIdAsync idi, onu kullandım.
            var userExperience = await _context.Experiences.Where(x => x.UserId == userId).OrderByDescending(x => x.StartDate).ToListAsync();
            return Ok(userExperience);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Experience>> PostExperience(Experience experience)
        {
            // Giriş yapan kullanıcının ID'sini alıyoruz
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            // Deneyimi bu kullanıcıya mühürlüyoruz
            experience.UserId = userId;

            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();

            return Ok(experience);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            var experience = await _context.Experiences
                                   .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (experience == null) return NotFound();

            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}