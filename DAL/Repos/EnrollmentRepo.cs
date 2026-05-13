using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class EnrollmentRepo
    {
        Learn2EarnDBContext db;

        public EnrollmentRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(Enrollment e)
        {
            db.Enrollments.Add(e);
            return db.SaveChanges() > 0;
        }

        public Enrollment Get(int id)
        {
            return db.Enrollments.Find(id);
        }

        public List<Enrollment> Get()
        {
            return db.Enrollments.ToList();
        }

        public bool Update(Enrollment e)
        {
            var exobj = Get(e.Id);
            db.Entry(exobj).CurrentValues.SetValues(e);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.Enrollments.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}
