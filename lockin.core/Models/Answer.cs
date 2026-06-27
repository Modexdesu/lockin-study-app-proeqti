using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace lockin.core.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public string AnswerText { get; set; }
        
        public bool IsCorrect {  get; set; }
        public int QuestionId{ get; set; }
        public Question Question { get; set; }

    }
}
