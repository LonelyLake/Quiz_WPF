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

        // Свойство для хранения индекса правильного ответа в вопросе
        public string BackgroundColor => IsCorrect ? "LightGreen" : "White";

        public Answer(string answerText, bool isCorrect)
        {
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }

        // Метод для сброса других правильных ответов, если выбран одиночный выбор
        public void SetCorrect(bool value)
        {
            IsCorrect = value;
        }

        public override string ToString()
        {
            return $"{AnswerText} (Correct: {IsCorrect})";
        }
    }
}
