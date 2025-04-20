using Quiz.ViewModels.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModels
{
    using Quiz.Model;
    using System.Collections;
    using System.Windows;
    using System.Windows.Input;

    public class QuizGeneratorViewModel : ViewModelBase
    {
        private string quizTitle = "";
        public string QuizTitle
        {
            get => quizTitle;
            set
            {
                quizTitle = value;
                OnPropertyChanged(nameof(QuizTitle));
            }
        }

        private Quiz quiz = null;
        public Quiz Quiz
        {
            get => quiz;
            set
            {
                quiz = value;
                OnPropertyChanged(nameof(Quiz));
            }
        }

        private bool isQuizCreated = false;
        public bool IsQuizCreated
        {
            get => isQuizCreated;
            set
            {
                isQuizCreated = value;
                OnPropertyChanged(nameof(IsQuizCreated));
            }
        }

        private ICommand createQuizCommand;
        public ICommand CreateQuizCommand => (createQuizCommand ?? (createQuizCommand = new RelayCommand(
            p =>
            {
                if (quiz != null)
                {
                    MessageBox.Show("Quiz already created.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(quizTitle))
                {
                    MessageBox.Show("Quiz title cannot be empty.");
                    return;
                }
                quiz = new Quiz(quizTitle);
                IsQuizCreated = true;
            },
            p => true
        )));
    }
}