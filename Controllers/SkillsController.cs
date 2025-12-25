using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple.DTOs;
using Simple.Services;
using System.Security.Claims;

namespace Simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        // GET: Sadece GİRİŞ YAPAN kişinin yeteneklerini getir
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSkills()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı bulunamadı.");
            }

            // DİKKAT: Servisteki metot ismi GetAllSkillsByIdAsync idi, onu kullandım.
            var userSkills = await _skillService.GetAllSkillsByIdAsync(userId);
            return Ok(userSkills);
        }

        // POST: Yetenek ekle (Sahibi ile birlikte)
        // DİKKAT: (Roles = "Admin") kaldırdık! Her üye kendi yeteneğini ekleyebilsin.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSkill([FromForm] SkillCreateDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Servise UserId gönderiyoruz ki veritabanına "Bu yetenek bu kişinindir" diye yazsın.
            await _skillService.AddSkillAsync(model, userId);

            return Ok(new { message = "Yetenek profilinize eklendi." });
        }

        // DELETE: Yetenek sil (Sadece kendi yeteneğini)
        // DİKKAT: (Roles = "Admin") kaldırdık!
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Servise "Bu ID'yi sil ama sahibi Bensem sil" diyoruz.
            var result = await _skillService.DeleteSkillAsync(id, userId);

            if (!result) return NotFound("Yetenek bulunamadı veya size ait değil.");

            return Ok(new { message = "Yetenek silindi." });
        }
    }
}