using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simple.Data;   // Namespace'lerine dikkat et
using Simple.Models; // Namespace'lerine dikkat et

namespace Simple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactMessagesController : ControllerBase
    {
        private readonly SimpleContext _context;

        public ContactMessagesController(SimpleContext context)
        {
            _context = context;
        }

        // GET: Sadece Admin okuyabilir!
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetMessages()
        {
            // En yeni mesaj en üstte gelsin
            return await _context.ContactMessages.OrderByDescending(m => m.SentAt).ToListAsync();
        }

        // POST: Herkes mesaj atabilir (Token istemiyoruz)
        [HttpPost]
        public async Task<ActionResult<ContactMessage>> SendMessage(ContactMessage message)
        {
            message.SentAt = DateTime.Now;
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
            return Ok(new { text = "Mesajınız alındı!" });
        }

        // DELETE: Mesaj Silme (YENİ EKLENDİ)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();

            _context.ContactMessages.Remove(msg);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}