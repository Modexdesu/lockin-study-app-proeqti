namespace lockin.core.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }

        // Navigation Property: Each Answer belongs to one Question
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}