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

        private bool isQuestionSectionEnabled = false;
        public bool IsQuestionSectionEnabled
        {
            get => isQuestionSectionEnabled;
            set
            {
                isQuestionSectionEnabled = value;
                OnPropertyChanged(nameof(IsQuestionSectionEnabled));
            }
        }

        private bool isAnswerSectionEnabled = false;
        public bool IsAnswerSectionEnabled
        {
            get => isAnswerSectionEnabled;
            set
            {
                isAnswerSectionEnabled = value;
                OnPropertyChanged(nameof(IsAnswerSectionEnabled));
            }
        }

        private bool isQuestionCreating = true;
        public bool IsQuestionCreating
        {
            get => isQuestionCreating;
            set
            {
                isQuestionCreating = value;
                OnPropertyChanged(nameof(IsQuestionCreating));
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

        private Question selectedQuestion = null;
        public Question SelectedQuestion
        {
            get => selectedQuestion;
            set
            {
                selectedQuestion = value;
                OnPropertyChanged(nameof(SelectedQuestion));
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

        private bool canModifyQuestion = true;
        public bool CanModifyQuestion
        {
            get => canModifyQuestion;
            set
            {
                canModifyQuestion = value;
                OnPropertyChanged(nameof(CanModifyQuestion));
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
                IsQuestionSectionEnabled = true;
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
                SelectedQuestion = newQuestion; // установить текущий вопрос
                IsQuestionSectionEnabled = false; // отключить секцию вопросов
                IsAnswerSectionEnabled = true;
                CanModifyQuestion = false;

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
                IsAnswerSectionEnabled = false; // Сбросить состояние добавления вопроса
                IsQuestionSectionEnabled = true; // Включить секцию вопросов
                CanModifyQuestion = true; // Включить возможность модификации вопроса
            },
            p => true
        )));

        private ICommand modifyQuestionCommand;
        public ICommand ModifyQuestionCommand => (modifyQuestionCommand ?? (modifyQuestionCommand = new RelayCommand(
            p =>
            {
                if (SelectedQuestion == null)
                {
                    MessageBox.Show("No question selected.");
                    return;
                }

                IsQuestionSectionEnabled = true;
                IsAnswerSectionEnabled = true;
                // IsQuestionModifying = true;
                IsQuestionCreating = false;
                CurrentQuestion = SelectedQuestion;
                CurrentQuestionText = SelectedQuestion.QuestionText;
                SelectedQuestionType = SelectedQuestion.Type;
                CurrentAnswersTexts = SelectedQuestion.Answers.Select(a => a.AnswerText).ToArray();
                CorrectAnswers = SelectedQuestion.Answers.Select(a => a.IsCorrect).ToArray();
                //CanModifyQuestion = false;

            },
            p => true
        )));

        private ICommand saveChangesCommand;
        public ICommand SaveChangesCommand => (saveChangesCommand ?? (saveChangesCommand = new RelayCommand(
            p =>
            {
                if (SelectedQuestion == null)
                {
                    MessageBox.Show("No question selected.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(CurrentQuestionText))
                {
                    MessageBox.Show("Question text cannot be empty.");
                    return;
                }

                Questions.ModifyQuestion(Questions.IndexOf(CurrentQuestion), CurrentQuestionText, SelectedQuestionType, CurrentAnswersTexts.ToList());
                MessageBox.Show("Changes saved.");
                resetCurrentQuestion();
                //CanModifyQuestion = true;
            },
            p => true
        )));

        private ICommand deleteQuestionCommand;
        public ICommand DeleteQuestionCommand => (deleteQuestionCommand ?? (deleteQuestionCommand = new RelayCommand(
            p =>
            {
                if (SelectedQuestion == null)
                {
                    MessageBox.Show("No question selected.");
                    return;
                }

                Questions.Remove(SelectedQuestion);
                Quiz.Questions.Remove(SelectedQuestion);
                MessageBox.Show("Question deleted.");
            },
            p => true
        )));

        public QuizGeneratorViewModel()
        {
            Questions = new QuestionCollection();
            CurrentQuestion = new Question("Hello");
        }

        private void resetCurrentQuestion()
        {
            CurrentQuestion = null;
            CurrentQuestionText = string.Empty;
            CurrentAnswersTexts = new string[4];
            CorrectAnswers = new bool[4];
        }
    }
}

public enum QuestionType
{
    SingleChoice,
    MultipleChoice
}