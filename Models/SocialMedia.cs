namespace Simple.Models
{
    public class SocialMedia
    {
        public int Id { get; set; }
        public string PlatformName { get; set; } // Örn: LinkedIn
        public string Url { get; set; }          // Örn: https://linkedin.com/in/sen
        public string IconUrl { get; set; }      // İkon resmi veya FontAwesome class'ı

        public string? UserId { get; set; }
    }
}