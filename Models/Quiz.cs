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
        public Quiz(string title)
        {
            this.Title = title;
        }

    }
}
