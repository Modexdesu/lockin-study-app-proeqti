using System.Collections.Generic;
using System.Threading.Tasks; // REQUIRED for async operations
using lockin.core.Models;

namespace lockin.core.Interfaces
{
    public interface ITopicRepository
    {
        // Change from List<Topic> to Task<List<Topic>>
        Task<List<Topic>> GetAllTopicsAsync();
    }
}