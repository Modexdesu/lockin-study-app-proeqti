using lockin.business.Data;
using lockin.core.Interfaces;
using lockin.core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lockin.business.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly lockindbcontext _context;

        public AnswerRepository(lockindbcontext context)
        {
            _context = context;
        }

        public async Task<List<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _context.Answer
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }
    }
}