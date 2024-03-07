using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Notification;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Service.IServices;

namespace Service.Service
{
    public class NotificationService : INotificationService
    {
        private readonly MongoDbContext _context;

        public NotificationService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllNotifications()
        {
            return await _context.Notifications.Find(_ => true).ToListAsync();
        }

        public async Task<Notification> GetNotificationById(string id)
        {
            return await _context.Notifications.Find(notification => notification.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateNotification(Notification notification)
        {
            await _context.Notifications.InsertOneAsync(notification);
        }

        public async Task UpdateNotification(string id, Notification notification)
        {
            await _context.Notifications.ReplaceOneAsync(n => n.Id == id, notification);
        }

        public async Task DeleteNotification(string id)
        {
            await _context.Notifications.DeleteOneAsync(n => n.Id == id);
        }
        
        public async Task<List<Notification>> GetNewestNotificationsForSeller(string userId)
        {
            var notifications = await _context.Notifications
                .Find(notification => notification.BuyerId == userId)
                .SortByDescending(notification => notification.CreatedAt)
                .ToListAsync();

            return notifications;
        }
    }
}