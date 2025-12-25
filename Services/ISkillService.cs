using Simple.DTOs;
using Simple.Models;

namespace Simple.Services
{
    public interface ISkillService
    {
        Task<IEnumerable<Skill>> GetAllSkillsByIdAsync(string userId);

        // Parametre olarak string userId eklendi
        Task AddSkillAsync(SkillCreateDto model, string userId);

        // Parametre olarak string userId eklendi
        Task<bool> DeleteSkillAsync(int id, string userId);
    }
}