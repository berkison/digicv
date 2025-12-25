using Microsoft.EntityFrameworkCore;
using Simple.Data;
using Simple.DTOs;
using Simple.Models;

namespace Simple.Services
{
    public class ProjectService : IProjectService
    {
        private readonly SimpleContext _context;

        public ProjectService(SimpleContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            // Veritabanındaki tüm projeleri listeye çevirip getirir
            // OrderByDescending ile en son eklenen en başta gözükür
            return await _context.Projects
                                 .OrderByDescending(x => x.CreatedDate)
                                 .ToListAsync();
        }

        public async Task AddProjectAsync(ProjectDto model)
        {
            // DTO'dan gelen veriyi gerçek Veritabanı Nesnesine (Entity) çeviriyoruz
            var newProject = new Project
            {
                Title = model.Title,
                Description = model.Description,
                GitHubLink = model.GitHubLink,
                ImageUrl = model.ImageUrl,
                LiveDemoLink = model.LiveDemoLink,
                CreatedDate = DateTime.Now
            };

            await _context.Projects.AddAsync(newProject);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return false; // Proje bulunamadı

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}