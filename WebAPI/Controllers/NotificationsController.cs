using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using Data.Entities.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotifications();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(string id)
        {
            var notification = await _notificationService.GetNotificationById(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification(Notification notification)
        {
            await _notificationService.CreateNotification(notification);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(string id, Notification notification)
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            await _notificationService.UpdateNotification(id, notification);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            await _notificationService.DeleteNotification(id);
            return NoContent();
        }
    }
}
