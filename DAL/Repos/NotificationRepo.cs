using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class NotificationRepo
    {
        Learn2EarnDBContext db;

        public NotificationRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(Notification n)
        {
            db.Notifications.Add(n);
            return db.SaveChanges() > 0;
        }

        public Notification Get(int id)
        {
            return db.Notifications.Find(id);
        }

        public List<Notification> Get()
        {
            return db.Notifications.ToList();
        }

        public List<Notification> GetByUser(int userId)
        {
            return db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public int GetUnreadCount(int userId)
        {
            return db.Notifications.Count(n => n.UserId == userId && !n.IsRead);
        }

        public bool MarkAsRead(int notificationId)
        {
            var n = db.Notifications.Find(notificationId);
            if (n == null) return false;
            n.IsRead = true;
            return db.SaveChanges() > 0;
        }

        public bool MarkAllAsRead(int userId)
        {
            var unread = db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToList();
            foreach (var n in unread) n.IsRead = true;
            return db.SaveChanges() > 0;
        }

        public bool Update(Notification n)
        {
            var exobj = Get(n.Id);
            db.Entry(exobj).CurrentValues.SetValues(n);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.Notifications.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}
