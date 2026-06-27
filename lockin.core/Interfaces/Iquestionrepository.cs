using System.Collections.Generic;
using System.Threading.Tasks; // Required for Async
using lockin.core.Models;

namespace lockin.core.Interfaces
{
    public interface IQuestionRepository
    {


        Task AddAsync(Question question);
        Task <List<Question>> GetAllAsync();
        Task<Question> GetByIdAsync(int id);
        Task UpdateAsync(Question question);
        Task DeleteAsync(int id);
    }
}