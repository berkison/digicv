using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Simple.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        // IWebHostEnvironment: Sunucunun hangi klasörde çalıştığını bulmamızı sağlar
        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null) return null;

            // --- GÜNCELLENEN KISIM ---
            // Eğer WebRootPath null gelirse (klasör yoksa), ana dizine (ContentRootPath) wwwroot ekle
            string webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");


            // 1. Klasör Yolu Belirle (wwwroot/uploads)
            var uploadsFolder = Path.Combine(webRootPath, "uploads");

            // Eğer klasör yoksa oluştur
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 2. Benzersiz Dosya Adı Oluştur (Guid kullanıyoruz ki aynı isimli dosyalar çakışmasın)
            // Örn: kalem.jpg -> 550e8400-e29b...jpg
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            
            // 3. Dosyanın Kaydedileceği Tam Yol
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Dosyayı Kaydet
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 5. Geriye resmin URL yolunu dönüyoruz (/uploads/resim.jpg)
            return "/uploads/" + uniqueFileName;
        }
    }
}