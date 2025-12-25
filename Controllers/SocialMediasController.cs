using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple.Data;
using Simple.Models;
using System.Security.Claims;

namespace Simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediasController : ControllerBase
    {
        private readonly SimpleContext _context;

        public SocialMediasController(SimpleContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SocialMedia>>> GetSocialMedias()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bulunamadı.");
            }
            var userSocialMedias = await _context.SocialMedias.Where(x => x.UserId == userId).ToListAsync();
            return Ok(userSocialMedias);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SocialMedia>> PostSocialMedia(SocialMedia socialMedia)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            socialMedia.UserId = userId;
            _context.SocialMedias.Add(socialMedia);
            await _context.SaveChangesAsync();
            return Ok(socialMedia);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSocialMedia(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            var sm = await _context.SocialMedias.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (sm == null) return NotFound();

            _context.SocialMedias.Remove(sm);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}