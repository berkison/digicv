namespace Simple.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now; // Otomatik şu anki zaman
    }
}