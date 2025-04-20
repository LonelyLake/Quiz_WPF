using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Model
{
    public class Question
    {
        public string QuestionText { get; set; }
        public AnswerCollection Answers { get; set; } = new AnswerCollection();

        public QuestionType Type { get; set; } = QuestionType.SingleChoice;
        public Question(string questionText)
        {
            QuestionText = questionText;
        }

    }
}

