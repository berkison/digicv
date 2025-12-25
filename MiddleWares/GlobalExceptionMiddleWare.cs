using System.Net;
using System.Text.Json;

namespace Simple.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Her istek bu metodun içinden geçer
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // İsteği bir sonraki aşamaya (Controller'a) gönder
                await _next(context);
            }
            catch (Exception ex)
            {
                // HATA OLURSA BURAYA DÜŞER!
                // Panik yok, hatayı biz yöneteceğiz.
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Cevabın tipi JSON olsun
            context.Response.ContentType = "application/json";

            // Hata kodu 500 (Internal Server Error) olsun
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Kullanıcıya dönecek mesaj
            var response = new
            {
                Hata = "Sunucuda beklenmedik bir hata oluştu.",
                Detay = exception.Message, // Gerçek projede burayı gizleriz, şimdilik görelim diye açık.
                Zaman = DateTime.Now
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}