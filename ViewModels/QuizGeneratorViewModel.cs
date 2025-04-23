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

        private bool isQuestionModified = false;
        public bool IsQuestionModified
        {
            get => isQuestionModified;
            set
            {
                isQuestionModified = value;
                OnPropertyChanged(nameof(IsQuestionModified));
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

        private QuestionType selectedQuestionType;
        public QuestionType SelectedQuestionType
        {
            get => selectedQuestionType;
            set
            {
                selectedQuestionType = value;
                OnPropertyChanged(nameof(SelectedQuestionType));
            }
        }

        public string[] QuestionTypes => Enum.GetNames(typeof(QuestionType));
        public string SelectedQuestionTypeString
        {
            get => selectedQuestionType.ToString();
            set
            {
                if (Enum.TryParse(value, out QuestionType parsedType))
                {
                    SelectedQuestionType = parsedType;
                    OnPropertyChanged(nameof(SelectedQuestionType));
                }
            }
        }

        private AnswerCollection answers = null;
        public AnswerCollection Answers
        {
            get => answers;
            set
            {
                answers = value;
                OnPropertyChanged(nameof(Answers));
            }
        }

        private bool[] correctAnswers = new bool[4];
        public bool[] CorrectAnswers
        {
            get => correctAnswers;
            set
            {
                correctAnswers = value;
                OnPropertyChanged(nameof(CorrectAnswers));
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

                

                if (SelectedQuestionType == QuestionType.SingleChoice)
                {
                    newQuestion.Type = QuestionType.SingleChoice;
                    MessageBox.Show("Single choice question added.");
                }
                else if (SelectedQuestionType == QuestionType.MultipleChoice)
                {
                    newQuestion.Type = QuestionType.MultipleChoice;
                    MessageBox.Show("Multiple choice question added.");
                }
                else
                {
                    MessageBox.Show("Invalid question type.");
                    return;
                }
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
                if (CurrentAnswersTexts.Any(a => string.IsNullOrWhiteSpace(a)))
                {
                    MessageBox.Show("Answer text cannot be empty.");
                    return;
                }
                if (CurrentAnswersTexts.Length != 4)
                {
                    MessageBox.Show("Exactly 4 answers must be provided.");
                    return;
                }
                // Check if no answers were marked as correct (all bools in array false)
                foreach (var index in Enumerable.Range(0, 4))
                {
                    if (CorrectAnswers[index])
                    {
                        break;
                    }
                    if (index == 3)
                    {
                        MessageBox.Show("At least one answer must be marked as correct.");
                        return;
                    }
                }

                if (CurrentQuestion.Type == QuestionType.SingleChoice && CorrectAnswers.Count(c => c) > 1)
                {
                    MessageBox.Show("Only one answer can be correct for a single choice question.");
                    return;
                }

                if (CurrentQuestion.Type == QuestionType.MultipleChoice && CorrectAnswers.Count(c => c) == 0)
                {
                    MessageBox.Show("At least one answer must be correct for a multiple choice question.");
                    return;
                }
                foreach (var index in Enumerable.Range(0, 4))
                {
                    var text = CurrentAnswersTexts[index];
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        bool isCorrect = CorrectAnswers[index];
                        CurrentQuestion.Answers.AddAnswer(text, isCorrect);
                    }
                }
                CurrentAnswersTexts = new string[4];
                CorrectAnswers = new bool[4];
                CurrentAnswersTexts = new string[4]; // Это вызовет PropertyChanged
                MessageBox.Show(CurrentQuestion.Answers.ToString());
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

public enum QuestionType
{
    SingleChoice,
    MultipleChoice
}