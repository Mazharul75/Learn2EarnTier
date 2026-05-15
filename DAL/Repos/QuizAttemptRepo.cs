using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class QuizAttemptRepo
    {
        Learn2EarnDBContext db;

        public QuizAttemptRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(QuizAttempt a)
        {
            db.QuizAttempts.Add(a);
            return db.SaveChanges() > 0;
        }

        public QuizAttempt Get(int id)
        {
            return db.QuizAttempts.Find(id);
        }

        public List<QuizAttempt> Get()
        {
            return db.QuizAttempts.ToList();
        }

        public List<QuizAttempt> GetByLearnerAndQuiz(int learnerId, int quizId)
        {
            return db.QuizAttempts
                .Where(a => a.LearnerId == learnerId && a.QuizId == quizId)
                .ToList();
        }

        public List<QuizAttempt> GetByLearner(int learnerId)
        {
            return db.QuizAttempts
                .Where(a => a.LearnerId == learnerId)
                .ToList();
        }

        public bool Update(QuizAttempt a)
        {
            var exobj = Get(a.Id);
            db.Entry(exobj).CurrentValues.SetValues(a);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.QuizAttempts.Remove(exobj);
            return db.SaveChanges() > 0;
        }
    }
}
