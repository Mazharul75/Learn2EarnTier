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

        // ===== Create a notification for one user =====
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

        // ===== Create notifications for many users =====
        public void NotifyMany(IEnumerable<int> userIds, string message, string? link = null)
        {
            foreach (var uid in userIds)
            {
                Notify(uid, message, link);
            }
        }

        // ===== Notify every user of a given role (e.g., all Learners) =====
        public void NotifyByRole(int userTypeId, string message, string? link = null)
        {
            var users = userRepo.Get().Where(u => u.UserTypeId == userTypeId).Select(u => u.Id);
            NotifyMany(users, message, link);
        }

        // ===== Get all notifications for a user, newest first =====
        public List<NotificationDTO> GetForUser(int userId)
        {
            var list = notificationRepo.GetByUser(userId);
            return mapper.Map<List<NotificationDTO>>(list);
        }

        // ===== Just the unread count for the nav badge =====
        public int GetUnreadCount(int userId)
        {
            return notificationRepo.GetUnreadCount(userId);
        }

        // ===== Mark one notification as read =====
        public bool MarkAsRead(int notificationId)
        {
            return notificationRepo.MarkAsRead(notificationId);
        }

        // ===== Mark all notifications as read for a user =====
        public bool MarkAllAsRead(int userId)
        {
            return notificationRepo.MarkAllAsRead(userId);
        }
    }
}