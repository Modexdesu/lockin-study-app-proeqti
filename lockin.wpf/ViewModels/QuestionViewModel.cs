using lockin.core.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

public class QuestionViewModel
{
    private readonly Question _question;
    public string QuestionText => _question.QuestionText;
    public List<Answer> ShuffledAnswers { get; }

    public QuestionViewModel(Question question)
    {
        _question = question;
        // Logic lives here, not in the UI
        ShuffledAnswers = question.Answers.OrderBy(x => Guid.NewGuid()).ToList();
    }

    public bool IsAnswerCorrect(Answer selectedAnswer)
    {
        return selectedAnswer.IsCorrect;
    }
}
