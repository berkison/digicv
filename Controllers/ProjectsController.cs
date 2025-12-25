using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple.Data;
using Simple.Models;
using Simple.DTOs; // Eğer DTO kullanıyorsan
using System.Security.Claims;

namespace Simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly SimpleContext _context;
        private readonly IWebHostEnvironment _env; // Resim yükleme için

        public ProjectsController(SimpleContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // --- İŞTE BU EKSİKTİ: [HttpGet] ---
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            // Token'ın içindeki UserId'yi bul
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Sadece bu ID'ye sahip projeleri filtrele
            return await _context.Projects
                                 .Where(p => p.UserId == userId)
                                 .ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject([FromForm] ProjectCreateDto projectDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            // Resim Yükleme İşlemi
            string imagePath = "";
            if (projectDto.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(projectDto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await projectDto.ImageFile.CopyToAsync(stream);
                }
                imagePath = "/uploads/" + fileName;
            }

            var project = new Project
            {
                Title = projectDto.Title,
                Description = projectDto.Description,
                GitHubLink = projectDto.GitHubLink,
                LiveDemoLink = projectDto.LiveDemoLink,
                ImageUrl = imagePath,
                UserId = userId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            var project = await _context.Projects
                                        .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}