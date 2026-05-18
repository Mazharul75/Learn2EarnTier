using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class TaskSubmissionRepo
    {
        Learn2EarnDBContext db;

        public TaskSubmissionRepo(Learn2EarnDBContext db) { this.db = db; }

        public bool Create(TaskSubmission s) { db.TaskSubmissions.Add(s); return db.SaveChanges() > 0; }
        public TaskSubmission? Get(int id) => db.TaskSubmissions.Find(id);
        public List<TaskSubmission> Get() => db.TaskSubmissions.ToList();
        public bool Update(TaskSubmission s)
        {
            var ex = Get(s.Id);
            if (ex == null) return false;
            db.Entry(ex).CurrentValues.SetValues(s);
            return db.SaveChanges() > 0;
        }
        public bool Delete(int id)
        {
            var ex = Get(id);
            if (ex == null) return false;
            db.TaskSubmissions.Remove(ex);
            return db.SaveChanges() > 0;
        }

        public TaskSubmission? GetByLearnerAndTask(int learnerId, int taskId)
        {
            return db.TaskSubmissions.FirstOrDefault(s => s.LearnerId == learnerId && s.TaskId == taskId);
        }

        public List<TaskSubmission> GetByLearnerAndCourse(int learnerId, int courseId)
        {
            // Submissions for any task in the given course, by this learner
            return (from s in db.TaskSubmissions
                    join t in db.CourseTasks on s.TaskId equals t.Id
                    where s.LearnerId == learnerId && t.CourseId == courseId
                    select s).ToList();
        }

        public List<TaskSubmission> GetPendingByCourse(int courseId)
        {
            return (from s in db.TaskSubmissions
                    join t in db.CourseTasks on s.TaskId equals t.Id
                    where t.CourseId == courseId && s.Status == "Pending"
                    select s).ToList();
        }
    }
}