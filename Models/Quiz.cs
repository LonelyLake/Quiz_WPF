using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Model
{
    public class Quiz
    {
        private string title;
        public QuestionCollection Questions { get; set; } = new QuestionCollection();
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                // NotifyPropertyChanged(nameof(Title));
            }
        }

        public const int ANSWER_COUNT = 4;

        public Quiz(string title)
        {
            this.Title = title;
        }

        public override string ToString()
        {
            string separator = ";";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Quiz Title: {Title}" + separator);
            foreach (var question in Questions)
            {
                sb.AppendLine(question.ToString() + separator);
            }
            return sb.ToString();
        }
    }
}
