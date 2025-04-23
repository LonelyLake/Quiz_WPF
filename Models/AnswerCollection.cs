using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Quiz.Models
{
    public class AnswerCollection : ObservableCollection<Answer>
    {
        public AnswerCollection() { }
        public AnswerCollection(IEnumerable<Answer> answers) : base(answers) { }
        public void AddAnswer(string answerText, bool isCorrect)
        {
            this.Add(new Answer(answerText, isCorrect));
        }
        public void RemoveAnswer(Answer answer)
        {
            this.Remove(answer);
        }

        public void AddAnswers(AnswerCollection answers)
        {
            foreach (var answer in answers)
            {
                this.Add(answer);
            }
        }

        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var answer in this)
            {
                sb.AppendLine(answer.ToString());
            }
            return sb.ToString();
        }
    }
    }
