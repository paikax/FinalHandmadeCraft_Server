using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities.Notification;

namespace Service.IServices
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllNotifications();
        Task<Notification> GetNotificationById(string id);
        Task CreateNotification(Notification notification);
        Task UpdateNotification(string id, Notification notification);
        Task DeleteNotification(string id);
    }
}