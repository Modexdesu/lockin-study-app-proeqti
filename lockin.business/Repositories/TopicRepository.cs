using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // REQUIRED for ToListAsync()
using lockin.core.Interfaces;
using lockin.core.Models;
using lockin.business.Data;

namespace lockin.business.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly lockindbcontext _context;

        public TopicRepository(lockindbcontext context)
        {
            _context = context;
        }

        // Add 'async' and wrap the return in a Task
        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            // Use 'await' and 'ToListAsync()' so the thread releases while SQL works
            return await _context.Topic.ToListAsync();
        }
    }
}