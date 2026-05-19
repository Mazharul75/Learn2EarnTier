using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BLL.DTOs;
using DAL.EF.Tables;
using DAL.Repos;

namespace BLL.Services
{
    public class NotificationService
    {
        NotificationRepo notificationRepo;
        UserRepo userRepo;
        Mapper mapper;

        public NotificationService(NotificationRepo notificationRepo, UserRepo userRepo)
        {
            this.notificationRepo = notificationRepo;
            this.userRepo = userRepo;
            mapper = MapperConfig.GetMapper();
        }

        public void Notify(int userId, string message, string? link = null)
        {
            var n = new Notification
            {
                UserId = userId,
                Message = message,
                Link = link,
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            notificationRepo.Create(n);
        }

        public void NotifyMany(IEnumerable<int> userIds, string message, string? link = null)
        {
            foreach (var uid in userIds)
            {
                Notify(uid, message, link);
            }
        }

        public void NotifyByRole(int userTypeId, string message, string? link = null)
        {
            var users = userRepo.Get().Where(u => u.UserTypeId == userTypeId).Select(u => u.Id);
            NotifyMany(users, message, link);
        }

        public List<NotificationDTO> GetForUser(int userId)
        {
            var list = notificationRepo.GetByUser(userId);
            return mapper.Map<List<NotificationDTO>>(list);
        }

        public int GetUnreadCount(int userId)
        {
            return notificationRepo.GetUnreadCount(userId);
        }

        public bool MarkAsRead(int notificationId)
        {
            return notificationRepo.MarkAsRead(notificationId);
        }

        public bool MarkAllAsRead(int userId)
        {
            return notificationRepo.MarkAllAsRead(userId);
        }
    }
}