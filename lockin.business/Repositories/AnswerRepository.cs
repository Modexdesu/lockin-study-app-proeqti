
using lockin.core.Interfaces;
using lockin.core.Models;
using lockin.business.Data; 

namespace lockin.business.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly lockindbcontext _context;

        // Constructor injection so the repository can talk to SQLEXPRESS
        public AnswerRepository(lockindbcontext context)
        {
            _context = context;
        }

        public List<Answer> GetAnswersByQuestionId(int questionId)
        {
            return _context.Answer
                           .Where(a => a.QuestionId == questionId)
                           .ToList();
        }

        public void AddAnswer(Answer answer)
        {
            _context.Answer.Add(answer);
            _context.SaveChanges(); // Saves it directly to  SQL table
        }

        public void UpdateAnswer(Answer answer)
        {
            _context.Answer.Update(answer);
            _context.SaveChanges();
        }
    }
}