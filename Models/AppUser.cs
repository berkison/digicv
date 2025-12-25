using Microsoft.AspNetCore.Identity;

namespace Simple.Models
{
    public class AppUser : IdentityUser
    {
        // Temel Bilgiler
        public string? FullName { get; set; }
        public string? JobTitle { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsProfilePublic { get; set; } = true;

        // İletişim ve Lokasyon (Dinamik CV için)
        public string? Address { get; set; }

        // Önemli: IdentityUser içinde 'PhoneNumber' ve 'Email' zaten var. 
        // Eğer özel bir format istemiyorsan onları buraya tekrar yazmana gerek yok.

        // Dinamik Alanlar
        public string? Languages { get; set; } // Örn: "Türkçe (Anadil), İngilizce (C1)"

        // Referans Bilgileri (Basit tutmak için string, detaylı istersen ayrı tablo)
        public string? ReferenceName { get; set; }
        public string? ReferenceContact { get; set; }

        // Sosyal Medya Linkleri (Direkt profilden çekmek bazen daha hızlıdır)
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PersonalWebsiteUrl { get; set; }
    }
}