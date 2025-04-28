using Quiz.ViewModels.BaseClass;
using Quiz.Model;
using Quiz.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Quiz.ViewModels
{
    public class QuizSolverViewModel : ViewModelBase
    {
        private string timeLeft = "30,00";
        public string TimeLeft
        {
            get => timeLeft;
            set
            {
                if (timeLeft != value)
                {
                    timeLeft = value;
                    OnPropertyChanged(nameof(TimeLeft));
                }
            }
        }

        private System.Timers.Timer _timer;
        private double millisecondsLeft;

        private Quiz.Model.Quiz loadedQuiz;
        private bool isQuizStarted;
        private bool isQuizEnded;

        public ObservableCollection<Question> DisplayedQuestions { get; set; } = new ObservableCollection<Question>();

        private int points;
        public int Points
        {
            get => points;
            set
            {
                points = value;
                OnPropertyChanged(nameof(Points));
            }
        }

        public bool IsQuizLoaded => loadedQuiz != null;

        public bool IsQuizStarted
        {
            get => isQuizStarted;
            set
            {
                isQuizStarted = value;
                OnPropertyChanged(nameof(IsQuizStarted));
            }
        }

        public bool IsQuizEnded
        {
            get => isQuizEnded;
            set
            {
                isQuizEnded = value;
                OnPropertyChanged(nameof(IsQuizEnded));
                RaiseAllCommandsCanExecuteChanged();
            }
        }

        // ==== Komendy ====

        private ICommand loadQuizCommand;
        public ICommand LoadQuizCommand => loadQuizCommand ??= new RelayCommand(
            p =>
            {
                loadedQuiz = QuizFileManager.LoadQuizFromFile();
                if (loadedQuiz != null)
                {
                    MessageBox.Show("Quiz has been successfully loaded.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    OnPropertyChanged(nameof(IsQuizLoaded));
                    RaiseAllCommandsCanExecuteChanged();
                }
            },
            p => !IsQuizLoaded // Load aktywny tylko przed załadowaniem
        );

        private ICommand startTimerCommand;
        public ICommand StartTimerCommand => startTimerCommand ??= new RelayCommand(
    p =>
    {
        if (loadedQuiz == null) return;

        DisplayedQuestions.Clear(); // Czyścimy pytania przed startem

        foreach (var question in loadedQuiz.Questions)
        {
            var newQuestion = new Question(question.QuestionText, question.Type);
            foreach (var answer in question.Answers)
            {
                newQuestion.Answers.Add(new Answer(answer.AnswerText, answer.IsCorrect)
                {
                    IsEnabled = true,
                    BackgroundColor = "Transparent",
                    IsSelected = false
                });
            }

            DisplayedQuestions.Add(newQuestion);
        }

        // ===> Tutaj odświeżamy IsQuizReady
        OnPropertyChanged(nameof(IsQuizReady));


        IsQuizStarted = true;
        IsQuizEnded = false;

        millisecondsLeft = 30_000;
        TimeLeft = (millisecondsLeft / 1000).ToString("F2");

        _timer = new System.Timers.Timer(10);
        _timer.Elapsed += (s, e) =>
        {
            millisecondsLeft -= 10;

            if (millisecondsLeft <= 0)
            {
                millisecondsLeft = 0;
                _timer.Stop();

                App.Current.Dispatcher.Invoke(() =>
                {
                    TimeLeft = "0,00";
                    EndQuiz(); // Automatyczne zakończenie quizu
                });

                return;
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                TimeLeft = (millisecondsLeft / 1000).ToString("F2");
            });
        };

        _timer.Start();
        RaiseAllCommandsCanExecuteChanged();
    },
    p => IsQuizLoaded && !IsQuizStarted && !IsQuizEnded
);


        private ICommand endQuizCommand;
        public ICommand EndQuizCommand => endQuizCommand ??= new RelayCommand(
            p =>
            {
                EndQuiz();
            },
            p => IsQuizStarted && !IsQuizEnded // End aktywny tylko w czasie quizu
        );

        // ==== Metody pomocnicze ====

        private void EndQuiz()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            IsQuizStarted = false;
            IsQuizEnded = true;

            Points = 0;

            foreach (var question in DisplayedQuestions)
            {
                foreach (var answer in question.Answers)
                {
                    // Blokujemy możliwość zmiany odpowiedzi
                    answer.IsEnabled = false;

                    // Ustawiamy kolory odpowiedzi
                    if (answer.IsCorrect)
                    {
                        answer.BackgroundColor = "LightGreen"; // poprawne odpowiedzi na zielono
                    }
                    else if (answer.IsSelected && !answer.IsCorrect)
                    {
                        answer.BackgroundColor = "LightCoral"; // źle zaznaczone odpowiedzi na czerwono
                    }
                    else
                    {
                        answer.BackgroundColor = "Transparent"; // reszta bez koloru
                    }
                }

                // Liczenie punktów
                var selected = question.Answers.Where(a => a.IsSelected).ToList();
                var correct = question.Answers.Where(a => a.IsCorrect).ToList();

                if (selected.Count == correct.Count && !selected.Except(correct).Any())
                {
                    Points++;
                }
            }

            MessageBox.Show($"Quiz has ended!\nYour result: {Points} points.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);

            RaiseAllCommandsCanExecuteChanged();

            // ===> Tutaj odświeżamy IsQuizReady
            OnPropertyChanged(nameof(IsQuizReady));

        }

        private void RaiseAllCommandsCanExecuteChanged()
        {
            (loadQuizCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (startTimerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (endQuizCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public bool IsQuizReady
        {
            get => DisplayedQuestions != null && DisplayedQuestions.Count > 0;
        }

        private ICommand resetQuizCommand = null;
        public ICommand ResetQuizCommand => resetQuizCommand ??= new RelayCommand(
            p =>
            {
                ResetQuiz();
            },
            p => true // <-- ZAWSZE aktywny!
        );


        private void ResetQuiz()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            loadedQuiz = null;
            DisplayedQuestions.Clear();
            Points = 0;
            IsQuizStarted = false;
            IsQuizEnded = false;

            OnPropertyChanged(nameof(IsQuizLoaded));
            OnPropertyChanged(nameof(IsQuizReady));
            RaiseAllCommandsCanExecuteChanged();
        }

    }
}
