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
        public void AddAnswer(string answerText, bool isCorrect, int questionId)
        {
            this.Add(new Answer(answerText, isCorrect, questionId));
        }
        public void RemoveAnswer(Answer answer)
        {
            this.Remove(answer);
        }
    }
    }
