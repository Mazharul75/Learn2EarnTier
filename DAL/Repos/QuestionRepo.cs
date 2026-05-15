using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class QuestionRepo
    {
        Learn2EarnDBContext db;

        public QuestionRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(Question q)
        {
            db.Questions.Add(q);
            return db.SaveChanges() > 0;
        }

        public Question Get(int id)
        {
            return db.Questions.Find(id);
        }

        public List<Question> Get()
        {
            return db.Questions.ToList();
        }

        public List<Question> GetByQuiz(int quizId)
        {
            return db.Questions
                .Where(q => q.QuizId == quizId)
                .ToList();
        }

        public bool Update(Question q)
        {
            var exobj = Get(q.Id);
            db.Entry(exobj).CurrentValues.SetValues(q);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.Questions.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}