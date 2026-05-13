using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class UserTypeRepo
    {
        Learn2EarnDBContext db;

        public UserTypeRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(UserType ut)
        {
            db.UserTypes.Add(ut);
            return db.SaveChanges() > 0;
        }

        public UserType Get(int id)
        {
            return db.UserTypes.Find(id);
        }

        public List<UserType> Get()
        {
            return db.UserTypes.ToList();
        }

        public bool Update(UserType ut)
        {
            var exobj = Get(ut.Id);
            db.Entry(exobj).CurrentValues.SetValues(ut);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.UserTypes.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}
