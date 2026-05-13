using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class CourseRepo
    {
        Learn2EarnDBContext db;

        public CourseRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(Course c)
        {
            db.Courses.Add(c);
            return db.SaveChanges() > 0;
        }

        public Course Get(int id)
        {
            return db.Courses.Find(id);
        }

        public List<Course> Get()
        {
            return db.Courses.ToList();
        }

        public List<Course> GetByInstructor(int instructorId)
        {
            return db.Courses
                .Where(c => c.InstructorId == instructorId)
                .ToList();
        }

        public bool Update(Course c)
        {
            var exobj = Get(c.Id);
            db.Entry(exobj).CurrentValues.SetValues(c);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.Courses.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}
