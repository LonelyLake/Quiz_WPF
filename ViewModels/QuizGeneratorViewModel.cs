using Quiz.ViewModels.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModels
{
    using Quiz.Model;
    using Quiz.Models;
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
                if (Quiz != null)
                {
                    Quiz.Title = quizTitle;
                    OnPropertyChanged(nameof(Quiz));
                }
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

        private bool isQuestionAdded = false;
        public bool IsQuestionAdded
        {
            get => isQuestionAdded;
            set
            {
                isQuestionAdded = value;
                OnPropertyChanged(nameof(IsQuestionAdded));
            }
        }

        public QuestionCollection Questions { get; set; } = new QuestionCollection();
        private Question currentQuestion = null;
        public Question CurrentQuestion
        {
            get => currentQuestion;
            set
            {
                currentQuestion = value;
                OnPropertyChanged(nameof(CurrentQuestion));
            }
        }

        private string currentQuestionText;
        public string CurrentQuestionText
        {
            get => currentQuestionText;
            set
            {
                currentQuestionText = value;
                OnPropertyChanged(nameof(CurrentQuestionText));
            }
        }

        private string[] currentAnswersTexts = new string[4];
        public string[] CurrentAnswersTexts
        {
            get => currentAnswersTexts;
            set
            {
                currentAnswersTexts = value;
                OnPropertyChanged(nameof(CurrentAnswersTexts));
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

        private ICommand addQuestionCommand;
        public ICommand AddQuestionCommand => (addQuestionCommand ?? (addQuestionCommand = new RelayCommand(
            p =>
            {
                if (Quiz == null)
                {
                    MessageBox.Show("Quiz not created yet.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(CurrentQuestionText))
                {
                    MessageBox.Show("Question text cannot be empty.");
                    return;
                }

                var newQuestion = new Question(CurrentQuestionText);
                Questions.Add(newQuestion);
                Quiz.Questions.Add(newQuestion);
                CurrentQuestionText = ""; // очистить поле
                CurrentQuestion = newQuestion;
                IsQuestionAdded = true;
            },
            p => true
        )));

        private ICommand addAnswersCommand;
        public ICommand AddAnswersCommand => (addAnswersCommand ?? (addAnswersCommand = new RelayCommand(
            p =>
            {
                if (CurrentQuestion == null)
                {
                    MessageBox.Show("No question selected.");
                    return;
                }
                if (CurrentAnswersTexts == null || CurrentAnswersTexts.Length == 0)
                {
                    MessageBox.Show("No answers provided.");
                    return;
                }
                foreach (var answerText in CurrentAnswersTexts)
                {
                    CurrentQuestion.Answers.Add(new Answer(answerText, false, Questions.IndexOf(CurrentQuestion)));
                }
                CurrentAnswersTexts = new string[4]; // Это вызовет PropertyChanged
                CurrentQuestion = null; // Сбросить текущий вопрос
                IsQuestionAdded = false; // Сбросить состояние добавления вопроса
            },
            p => true
        )));

        public QuizGeneratorViewModel()
        {
            Questions = new QuestionCollection();
            CurrentQuestion = new Question("Hello");
        }
    }
}