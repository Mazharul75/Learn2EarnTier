using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class CourseTaskRepo
    {
        Learn2EarnDBContext db;

        public CourseTaskRepo(Learn2EarnDBContext db) { this.db = db; }

        public bool Create(CourseTask t) { db.CourseTasks.Add(t); return db.SaveChanges() > 0; }
        public CourseTask? Get(int id) => db.CourseTasks.Find(id);
        public List<CourseTask> Get() => db.CourseTasks.ToList();
        public bool Update(CourseTask t)
        {
            var ex = Get(t.Id);
            if (ex == null) return false;
            db.Entry(ex).CurrentValues.SetValues(t);
            return db.SaveChanges() > 0;
        }
        public bool Delete(int id)
        {
            var ex = Get(id);
            if (ex == null) return false;
            db.CourseTasks.Remove(ex);
            return db.SaveChanges() > 0;
        }

        public List<CourseTask> GetByCourse(int courseId)
        {
            return db.CourseTasks.Where(t => t.CourseId == courseId).ToList();
        }
    }
}