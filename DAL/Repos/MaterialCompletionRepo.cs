using System;
using System.Collections.Generic;
using System.Text;
using DAL.EF;
using DAL.EF.Tables;

namespace DAL.Repos
{
    public class MaterialCompletionRepo
    {
        Learn2EarnDBContext db;

        public MaterialCompletionRepo(Learn2EarnDBContext db)
        {
            this.db = db;
        }

        public bool Create(MaterialCompletion mc)
        {
            db.MaterialCompletions.Add(mc);
            return db.SaveChanges() > 0;
        }

        public MaterialCompletion? Get(int id)
        {
            return db.MaterialCompletions.Find(id);
        }

        public List<MaterialCompletion> Get()
        {
            return db.MaterialCompletions.ToList();
        }

        public bool Update(MaterialCompletion mc)
        {
            var exobj = Get(mc.Id);
            if (exobj == null) return false;
            db.Entry(exobj).CurrentValues.SetValues(mc);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            if (exobj == null) return false;
            db.MaterialCompletions.Remove(exobj);
            return db.SaveChanges() > 0;
        }

        // === Specialized queries ===

        public bool IsCompleted(int learnerId, int courseId)
        {
            return db.MaterialCompletions.Any(m => m.LearnerId == learnerId && m.CourseId == courseId);
        }

        public List<MaterialCompletion> GetByLearner(int learnerId)
        {
            return db.MaterialCompletions.Where(m => m.LearnerId == learnerId).ToList();
        }
    }
}