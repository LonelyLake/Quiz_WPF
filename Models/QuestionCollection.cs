using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class QuestionCollection : ObservableCollection<Question>
    {
        public QuestionCollection() {}

        public void AddQuestion(string questionText)
        {
            Question question = new Question(questionText);
            this.Add(question);
        }

        public void AddAnswersToQuestion(int questionIndex, List<string> answers)
        {
            if (questionIndex < 0 || questionIndex >= this.Count)
                throw new ArgumentOutOfRangeException(nameof(questionIndex), "Invalid question index.");
            Question question = this[questionIndex];
            foreach (var answerText in answers)
            {
                question.Answers.Add(new Answer(answerText, false));
            }
        }

        public void ModifyQuestion(int questionIndex, string newQuestionText, QuestionType type, List<string> newAnswers, List<bool> correctAnswers)
        {
            if (questionIndex < 0 || questionIndex >= this.Count)
                throw new ArgumentOutOfRangeException(nameof(questionIndex), "Invalid question index.");
            this.RemoveAt(questionIndex);
            Question question = new Question(newQuestionText, type);
            for (int i = 0; i < newAnswers.Count; i++)
            {
                question.Answers.Add(new Answer(newAnswers[i], correctAnswers[i]));
            }
            this.Insert(questionIndex, question);
        }

        public void RemoveQuestion(int questionIndex)
        {
            if (questionIndex < 0 || questionIndex >= this.Count)
                throw new ArgumentOutOfRangeException(nameof(questionIndex), "Invalid question index.");
            this.RemoveAt(questionIndex);
        }
    }
}
