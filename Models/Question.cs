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
        public int QuestionId { get; set; }


        public QuestionType Type { get; set; } = QuestionType.SingleChoice;
        public Question(string questionText)
        {
            QuestionText = questionText;
        }

        public void AddAnswer(string answerText, bool isCorrect)
        {
            var answer = new Answer(answerText, isCorrect);
            Answers.Add(answer);
        }
        public void AddAnswers(AnswerCollection answers)
        {
            foreach (var answer in answers)
            {
                Answers.Add(answer);
            }
        }

        public void RemoveAnswer(Answer answer)
        {
            Answers.Remove(answer);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Question: {QuestionText}");
            foreach (var answer in Answers)
            {
                sb.AppendLine(answer.ToString());
            }
            return sb.ToString();
        }
    }
}

