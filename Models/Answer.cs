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
        public bool IsSelected { get; set; }
        public bool IsEnabled { get; set; } = true;

        // public string FullAnswerText { get; set; } // Полное текстовое представление ответа

        // Свойство для хранения индекса правильного ответа в вопросе
        private string _backgroundColor = null;

        // Twoja właściwość – jeśli została ustawiona, bierze Twoje tło
        public string BackgroundColor
        {
            get
            {
                return _backgroundColor ?? (IsCorrect ? "LightGreen" : "White");
            }
            set
            {
                _backgroundColor = value;
            }
        }

        public Answer(string answerText, bool isCorrect)
        {
            AnswerText = answerText;
            IsCorrect = isCorrect;

            /*
            if (IsCorrect)
            {
                FullAnswerText = $"{AnswerText} (Correct)";
            }
            else
            {
                FullAnswerText = AnswerText;
            }
            */
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
