using Microsoft.EntityFrameworkCore;
using Simple.Data;
using Simple.DTOs;
using Simple.Models;

namespace Simple.Services
{
    public class SkillService : ISkillService
    {
        private readonly SimpleContext _context;
        private readonly IImageService _imageService;

        public SkillService(SimpleContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: Belirli bir kullanıcının yeteneklerini getir
        public async Task<IEnumerable<Skill>> GetAllSkillsByIdAsync(string userId)
        {
            return await _context.Skills
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        // POST: Ekleme yaparken UserId'yi de kaydet (DÜZELTİLDİ)
        public async Task AddSkillAsync(SkillCreateDto model, string userId)
        {
            // Resmi kaydet, yolunu al
            string imagePath = "";
            if (model.ImageFile != null)
            {
                imagePath = await _imageService.SaveImageAsync(model.ImageFile);
            }

            var skill = new Skill
            {
                Title = model.Title,
                Percent = model.Percent,
                ImageUrl = imagePath,
                UserId = userId // <--- ARTIK SAHİBİNİ KAYDEDİYORUZ
            };

            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();
        }

        // DELETE: Silerken sahibi kim kontrol et (DÜZELTİLDİ)
        public async Task<bool> DeleteSkillAsync(int id, string userId)
        {
            // Sadece ID yetmez, UserId de eşleşmeli!
            var skill = await _context.Skills
                                      .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (skill == null) return false; // Bulunamadı veya başkasının malı

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}