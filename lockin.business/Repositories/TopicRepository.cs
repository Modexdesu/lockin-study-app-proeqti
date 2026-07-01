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
        public async Task<List<Question>> GetQuestionsByTopicIdAsync(int topicId)
        {
            // Reaches into the SQLite database, filters by the TopicId, and returns a list.
            return await _context.Question
                                 .Where(q => q.TopicId == topicId)
                                 .ToListAsync();
        }


        public async Task<List<Question>> GetQuestionsAsync(int topicId, int difficulty)
        {
            return await _context.Question
                                 .Where(q => q.TopicId == topicId && q.QuestionDifficulty == difficulty)
                                 .ToListAsync();
        }
        public async Task AddTopicAsync(Topic topic)
        {
            // 1. Save to SQLite
            _context.Topic.Add(topic);
            await _context.SaveChangesAsync();

            // 2. Generate the text file in the background (Moved from ViewModel)
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TriviaQuestions");
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string safeFileName = string.Join("_", topic.TopicName.Split(Path.GetInvalidFileNameChars()));
            string filePath = Path.Combine(directoryPath, $"{safeFileName}_Questions.txt");

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, $"# Questions for {topic.TopicName}\n# Format: Question|Ans1|Ans2|Ans3|CorrectAns|DifficultyId(1-3)\n");
            }
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