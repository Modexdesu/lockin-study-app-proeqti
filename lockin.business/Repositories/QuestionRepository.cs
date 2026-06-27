using lockin.business.Data;
using lockin.core.Interfaces;
using lockin.core.Models;
using Microsoft.EntityFrameworkCore; // CRUCIAL for ToListAsync() and FindAsync()
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lockin.business.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly lockindbcontext _context;

        public QuestionRepository(lockindbcontext context)
        {
            _context = context;
        }


        public async Task AddAsync(Question question)
        {
            // Use AddAsync for the DbSet and SaveChangesAsync for the context
            await _context.Question.AddAsync(question);
            await _context.SaveChangesAsync();
        }
        // ASYNC READ: Get all questions without blocking threads
        public async Task<List<Question>> GetAllAsync()
        {
            return await _context.Question.ToListAsync();
        }

        // ASYNC READ: Find specific question
        public async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Question.FindAsync(id);
        }

        // ASYNC UPDATE: Modify and save asynchronously 
        public async Task UpdateAsync(Question question)
        {
            _context.Question.Update(question);
            await _context.SaveChangesAsync();
        }

        // ASYNC DELETE: Remove completely
        public async Task DeleteAsync(int id)
        {
            var questionToDelete = await _context.Question.FindAsync(id);
            if (questionToDelete != null)
            {
                _context.Question.Remove(questionToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}