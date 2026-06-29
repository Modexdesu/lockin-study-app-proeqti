using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using lockin.core.Interfaces;
using lockin.core.Models;
using lockin.business.Data; // 1. Crucial: This points to your lockindbcontext file

namespace lockin.business.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        // 2. Uses your exact DbContext class name
        private readonly lockindbcontext _context;

        public lockindbcontext Context => _context;

        public TopicRepository(lockindbcontext context)
        {
            _context = context;
        }

        // 3. Returns List<Topic> to satisfy error CS0738
        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            // 4. Uses singular '.Topic' to match your database schema
            return await _context.Topic.ToListAsync();
        }

        public async Task AddTopicAsync(Topic topic)
        {
            await _context.Topic.AddAsync(topic);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTopicAsync(Topic topic)
        {
            _context.Topic.Remove(topic);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTopicAsync(Topic topic)
        {
            _context.Topic.Update(topic);
            await _context.SaveChangesAsync();
        }
    }
}