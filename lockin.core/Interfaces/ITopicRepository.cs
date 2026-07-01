using System.Collections.Generic;
using System.Threading.Tasks; // REQUIRED for async operations
using lockin.core.Models;

namespace lockin.core.Interfaces
{
    public interface ITopicRepository
    {
        // Change from List<Topic> to Task<List<Topic>>
        Task<List<Topic>> GetAllTopicsAsync();
        Task<List<Question>> GetQuestionsByTopicIdAsync(int topicId);
        Task AddTopicAsync(Topic topic);
        Task DeleteTopicAsync(Topic topic);
        Task UpdateTopicAsync(Topic topic);
        Task<List<Question>> GetQuestionsAsync(int topicId, int difficulty);
    }
}