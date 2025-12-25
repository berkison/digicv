using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // BU EKLENDİ
using Microsoft.EntityFrameworkCore;
using Simple.Models;

namespace Simple.Data
{
    // DİKKAT: Artık DbContext değil, IdentityDbContext'ten miras alıyor!
    public class SimpleContext : IdentityDbContext<AppUser>
    {
        public SimpleContext(DbContextOptions<SimpleContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
    }
}