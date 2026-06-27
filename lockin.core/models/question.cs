using System.ComponentModel.DataAnnotations;

namespace lockin.core.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } 
        public int TopicId { get; set; }
        public int IncorrectCount { get; set; }
        public int QuestionDifficulty { get; set; }
        public int CorrectAnswerId { get; set; }
        public Topic Topic { get; set; }

        public List<Answer> Answers { get; set; } = new List<Answer>();

    }


  
}
