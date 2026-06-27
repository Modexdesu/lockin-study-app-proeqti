using System.Collections.Generic;
using lockin.core.Models;

namespace lockin.core.Interfaces
{
    public interface IAnswerRepository
    {
        // Get all answers for a specific multiple-choice question
        List<Answer> GetAnswersByQuestionId(int questionId);

        // Add a new answer choice to the database
        void AddAnswer(Answer answer);

        // Update an existing answer (like changing the text or fixing the IsCorrect flag)
        void UpdateAnswer(Answer answer);
    }
}