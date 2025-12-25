using Microsoft.AspNetCore.Http;

namespace Simple.DTOs
{
    public class UserUpdateDto
    {
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Bio { get; set; }

        // İŞTE EKSİK OLAN BAĞLANTILAR BURADA:
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        // Diller ve Referanslar
        public string? Languages { get; set; }       // <-- Bunu ekleyince AppUser'daki 0, 1 olacak
        public string? ReferenceName { get; set; }   // <-- Bunu ekleyince AppUser'daki 0, 1 olacak
        public string? ReferenceContact { get; set; }

        // Sosyal Medya
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PersonalWebsiteUrl { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}