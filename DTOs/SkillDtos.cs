using Microsoft.AspNetCore.Http;

namespace Simple.DTOs
{
    // Listelerken kullanacağımız (Resim string URL)
    public class SkillDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte Percent { get; set; }
        public string ImageUrl { get; set; }
    }

    // Eklerken kullanacağımız (Resim Dosya)
    public class SkillCreateDto
    {
        public string Title { get; set; }
        public byte Percent { get; set; } // 0-100 arası validasyon eklenebilir
        public IFormFile ImageFile { get; set; }
    }
}