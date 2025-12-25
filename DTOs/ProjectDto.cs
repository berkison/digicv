namespace Simple.DTOs
{
    public class ProjectDto
    {
        // ID istemiyoruz çünkü eklerken ID'yi DB verir.
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string GitHubLink { get; set; }
        public string? LiveDemoLink { get; set; }
    }
}