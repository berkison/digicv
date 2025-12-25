using Simple.DTOs;
using Simple.Models;

namespace Simple.Services
{
    public interface IProjectService
    {
        // Tüm projeleri getir (Herkes görebilir)
        Task<List<Project>> GetAllProjectsAsync();

        // Yeni proje ekle (Sadece Admin)
        Task AddProjectAsync(ProjectDto model);

        // Proje sil (Sadece Admin)
        Task<bool> DeleteProjectAsync(int id);
    }
}