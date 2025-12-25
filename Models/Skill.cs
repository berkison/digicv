namespace Simple.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Title { get; set; } // Örn: C#, React, Docker
        public byte Percent { get; set; } // Örn: 85, 90 (0-100 arası)
        public string ImageUrl { get; set; } // Logo yolu

        public string? UserId { get; set; }
    }
}