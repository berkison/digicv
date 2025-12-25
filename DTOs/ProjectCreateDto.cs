using Microsoft.AspNetCore.Http; // Bunu eklemeyi unutma

namespace Simple.DTOs
{
    public class ProjectCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string GitHubLink { get; set; }
        public string? LiveDemoLink { get; set; }

        // DİKKAT: Burası artık string değil, Dosya!
        public IFormFile ImageFile { get; set; }
    }
}