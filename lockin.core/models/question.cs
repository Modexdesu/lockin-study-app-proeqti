using System.ComponentModel.DataAnnotations;

namespace lockin.core.models
{
    public class Question
    {
        [Key]
        public int id { get; set; }
        public string QuestionText { get; set; } 

    }
}
