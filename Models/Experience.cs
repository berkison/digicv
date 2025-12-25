namespace Simple.Models // Namespace adını kendi projene göre kontrol et
{
    public class Experience
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Boş ise "Devam Ediyor" demektir

        public string? UserId { get; set; }
    }
}