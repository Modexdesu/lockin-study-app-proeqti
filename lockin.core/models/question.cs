using System.Collections.Generic;

namespace lockin.core.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionDifficulty { get; set; }
        public int TopicId { get; set; }

        // Navigation Property: One Question has many Answers
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}