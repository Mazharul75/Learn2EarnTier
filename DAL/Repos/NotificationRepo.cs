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
