using lockin.core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lockin.core.Interfaces
{
    public interface IAnswerRepository
    {
        Task<List<Answer>> GetAnswersByQuestionIdAsync(int questionId);
    }
}