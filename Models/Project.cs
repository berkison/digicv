namespace Simple.Models // Namespace'i kendi projene göre ayarla
{
    public class Project
    {
        public int Id { get; set; }

        public string Title { get; set; } // Proje Başlığı

        public string Description { get; set; } // Açıklama

        public string ImageUrl { get; set; } // Proje görseli (yolu)

        public string GitHubLink { get; set; } // GitHub linki

        public string? LiveDemoLink { get; set; } // Canlı site linki (Opsiyonel olabilir o yüzden ?)

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ne zaman eklendi?

        public string? UserId { get; set; }
    }
}