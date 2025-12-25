namespace Simple.Models
{
    public class Testimonial
    {
        public int Id { get; set; }
        public string ClientName { get; set; }      // Örn: Ahmet Hoca
        public string CompanyOrRole { get; set; }   // Örn: Bölüm Başkanı
        public string Comment { get; set; }         // Yorumu
        public string ImageUrl { get; set; }        // Fotoğrafı
    }
}