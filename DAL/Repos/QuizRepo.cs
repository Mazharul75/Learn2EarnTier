using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class QuizRepo
    {
        Learn2EarnDBContext db;

        public QuizRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(Quiz q)
        {
            db.Quizzes.Add(q);
            return db.SaveChanges() > 0;
        }

        public Quiz Get(int id)
        {
            return db.Quizzes.Find(id);
        }

        public List<Quiz> Get()
        {
            return db.Quizzes.ToList();
        }

        public bool Update(Quiz q)
        {
            var exobj = Get(q.Id);
            db.Entry(exobj).CurrentValues.SetValues(q);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.Quizzes.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}
