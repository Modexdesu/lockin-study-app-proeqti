using System.ComponentModel.DataAnnotations;

namespace lockin.core.Models
{
    public class Question
    {
        [Key]
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public string QuestionText { get; set; } 
        [Required]
        public int TopicId { get; set; }
        [Required]
        public int IncorrectCount { get; set; }
        [Required]
        public int QuestionDifficulty { get; set; }
        [Required]
        public string Option1 { get; set; }
        [Required]
        public string Option2 { get; set; } 

        [Required]
        public string Option3 { get; set; }
       
      public  string CorrectAnswer { get; set; }
        [Required]
        public Topic Topic { get; set; }



    }


  
}
