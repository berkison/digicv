using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple.Data;
using Simple.Models;

namespace Simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialsController : ControllerBase
    {
        private readonly SimpleContext _context;
        private readonly IWebHostEnvironment _env;

        public TestimonialsController(SimpleContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Testimonial>>> GetTestimonials()
        {
            return await _context.Testimonials.ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Testimonial>> PostTestimonial([FromForm] TestimonialDto dto)
        {
            // Resim Yükleme İşlemi
            string imagePath = "";
            if (dto.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
                imagePath = "/uploads/" + fileName;
            }

            var testimonial = new Testimonial
            {
                ClientName = dto.ClientName,
                CompanyOrRole = dto.CompanyOrRole,
                Comment = dto.Comment,
                ImageUrl = imagePath
            };

            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

            return Ok(testimonial);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestimonial(int id)
        {
            var t = await _context.Testimonials.FindAsync(id);
            if (t == null) return NotFound();

            _context.Testimonials.Remove(t);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // Dosya transferi için küçük bir yardımcı sınıf (Dosyanın en altına koyabilirsin)
    public class TestimonialDto
    {
        public string ClientName { get; set; }
        public string CompanyOrRole { get; set; }
        public string Comment { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}