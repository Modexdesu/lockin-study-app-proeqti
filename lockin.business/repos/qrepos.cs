
using lockin.core.Interfaces;
using lockin.core.models;
using lockin.business.Data;


namespace lockin.business.Repositories
{
    public class QuestionRepository : Iquestionrepository
    {
        // CREATE: Add a brand new question
        public void Add(Question question)
        {
            using (var context = new lockindbcontext())
            {
                context.Questions.Add(question);
                context.SaveChanges();
            }
        }

        // READ: Get all questions
        public List<Question> GetAll()
        {
            using (var context = new lockindbcontext())
            {
                return context.Questions.ToList();
            }
        }

        // READ: Find one specific question by ID
        public Question GetById(int id)
        {
            using (var context = new lockindbcontext())
            {
                return context.Questions.Find(id);
            }
        }

        // UPDATE: Modify an existing question (like incrementing those count columns!)
        public void Update(Question question)
        {
            using (var context = new lockindbcontext())
            {
                context.Questions.Update(question);
                context.SaveChanges();
            }
        }

        // DELETE: Remove a question completely
        public void Delete(int id)
        {
            using (var context = new lockindbcontext())
            {
                var questionToDelete = context.Questions.Find(id);
                if (questionToDelete != null)
                {
                    context.Questions.Remove(questionToDelete);
                    context.SaveChanges();
                }
            }
        }
    }
}