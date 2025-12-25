using Microsoft.AspNetCore.Http;

namespace Simple.Services
{
    public interface IImageService
    {
        // Resmi alır, kaydeder ve geriye resmin URL'ini döner
        Task<string> SaveImageAsync(IFormFile file);
    }
}