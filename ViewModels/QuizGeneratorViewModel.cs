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
    using Quiz.Services;
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

        private string selectedQuestionTypeString;
        public string SelectedQuestionTypeString
        {
            get => selectedQuestionTypeString;
            set
            {
                selectedQuestionTypeString = value;
                OnPropertyChanged(nameof(SelectedQuestionTypeString));
                if (Enum.TryParse(selectedQuestionTypeString, out QuestionType type))
                {
                    SelectedQuestionType = type;
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
                CurrentAnswersTexts = new string[4];
                CurrentQuestion = null;
                SelectedQuestion = null;
                IsAnswerSectionEnabled = false;
                IsQuestionSectionEnabled = true;
                CanModifyQuestion = true;
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
                SelectedQuestionTypeString = SelectedQuestion.Type.ToString();
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
                if (IsQuestionCreating)
                {
                    MessageBox.Show("Question is not being modified.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(CurrentQuestionText))
                {
                    MessageBox.Show("Question text cannot be empty.");
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
                if (correctAnswers == null || correctAnswers.Length == 0)
                {
                    MessageBox.Show("No correct answers provided.");
                    return;
                }
                if (SelectedQuestionType == QuestionType.SingleChoice && CorrectAnswers.Count(c => c) > 1)
                {
                    MessageBox.Show("Only one answer can be correct for a single choice question.");
                    return;
                }

                Quiz.Questions.ModifyQuestion(Questions.IndexOf(CurrentQuestion), CurrentQuestionText, SelectedQuestionType, CurrentAnswersTexts.ToList(), CorrectAnswers.ToList());
                Questions.ModifyQuestion(Questions.IndexOf(CurrentQuestion), CurrentQuestionText, SelectedQuestionType, CurrentAnswersTexts.ToList(), CorrectAnswers.ToList());
                MessageBox.Show("Changes saved.");
                resetCurrentQuestion();
                IsQuestionCreating = true;
                IsQuestionSectionEnabled = true;
                IsAnswerSectionEnabled = false;
            },
            p => true
        )));

        private ICommand deleteQuestionCommand;
        public ICommand DeleteQuestionCommand => (deleteQuestionCommand ?? (deleteQuestionCommand = new RelayCommand(
            p =>
            {
                if (isAnswerSectionEnabled)
                {
                    MessageBox.Show("Finish creating question before deleting.");
                    return;
                }

                if (SelectedQuestion == null)
                {
                    MessageBox.Show("No question selected.");
                    return;
                }

                if ((!IsQuestionSectionEnabled && SelectedQuestion.Equals(CurrentQuestion)))
                {
                    IsQuestionSectionEnabled = true;
                    IsAnswerSectionEnabled = false;
                }
                if (!IsQuestionCreating && SelectedQuestion.Equals(CurrentQuestion))
                {
                    IsQuestionCreating = true;
                    IsQuestionSectionEnabled = true;
                    IsAnswerSectionEnabled = false;
                }

                Quiz.Questions.Remove(SelectedQuestion);
                Questions.Remove(SelectedQuestion);
                
                
                MessageBox.Show("Question deleted.");

                IsQuestionSectionEnabled = true;
                IsAnswerSectionEnabled = false;
                IsQuestionCreating = true; //
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

        private ICommand saveQuizToFileCommand;
        public ICommand SaveQuizToFileCommand => (saveQuizToFileCommand ?? (saveQuizToFileCommand = new RelayCommand(
            p =>
            {
                if (Quiz == null)
                {
                    MessageBox.Show("No quiz to save.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(QuizTitle))
                {
                    MessageBox.Show("Quiz title cannot be empty.");
                    return;
                }
                if (Quiz.Questions.Count == 0)
                {
                    MessageBox.Show("No questions in the quiz.");
                    return;
                }
                if (!isQuestionSectionEnabled)
                {
                    MessageBox.Show("Finish creating question before saving.");
                    return;
                }

                try
                {
                    // Implement the logic to save the quiz to a file
                    QuizFileManager.SaveQuizToFile(Quiz);
                    // You can use serialization or any other method to save the quiz data
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving quiz: {ex.Message}");
                }

            },
            p => true
        )));

        private ICommand loadQuizFromFileCommand;
        public ICommand LoadQuizFromFileCommand => (loadQuizFromFileCommand ?? (loadQuizFromFileCommand = new RelayCommand(
            p =>
            {
                try
                {
                    // Implement the logic to load the quiz from a file
                    Quiz loadedQuiz = QuizFileManager.LoadQuizFromFile();
                    if (loadedQuiz == null)
                    {
                        MessageBox.Show("No quiz loaded.");
                        return;
                    }


                    if (loadedQuiz != null)
                    {
                        Quiz = loadedQuiz;
                        QuizTitle = loadedQuiz.Title;
                        Questions.Clear();
                        foreach (var question in loadedQuiz.Questions)
                        {
                            Questions.Add(question);
                        }
                        MessageBox.Show("Quiz loaded from file.");

                        IsQuestionSectionEnabled = true;
                        IsAnswerSectionEnabled = false;
                        IsQuestionCreating = true;
                        CanModifyQuestion = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading quiz: {ex.Message}");
                }
            },
            p => true
        )));

        private void resetQuiz()
        {
            Quiz = null;
            QuizTitle = string.Empty;
            Questions.Clear();
            CurrentQuestion = null;
            CurrentQuestionText = string.Empty;
            CurrentAnswersTexts = new string[4];
            CorrectAnswers = new bool[4];
            IsQuestionSectionEnabled = false;
            IsAnswerSectionEnabled = false;
            CanModifyQuestion = true;
        }
    }
}

public enum QuestionType
{
    SingleChoice,
    MultipleChoice
}