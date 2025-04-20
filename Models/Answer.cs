using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Model
{
    public class Answer
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public Answer(string answerText, bool isCorrect, int questionId)
        {
            AnswerText = answerText;
            IsCorrect = isCorrect;
            QuestionId = questionId;
        }
    }
}
