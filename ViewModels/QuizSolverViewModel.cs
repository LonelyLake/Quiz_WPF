using Quiz.ViewModels.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers;
using System.Windows;
using Quiz.Model;
using System.Collections.ObjectModel;
using QuizModel = Quiz.Model.Quiz;
using Quiz.Services;
using System.Collections.ObjectModel;

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


        private ICommand startTimerCommand;
        public ICommand StartTimerCommand => (startTimerCommand ?? (startTimerCommand = new RelayCommand(
            p =>
            {
                if (loadedQuiz == null) return;

                DisplayedQuestions.Clear();
                foreach (var q in loadedQuiz.Questions)
                {
                    DisplayedQuestions.Add(q);
                }

                IsQuizStarted = true;

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
                            MessageBox.Show(
                                "Time is up! Check your score and correct answers.",
                                "Finish!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                        });

                        return;
                    }
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        TimeLeft = (millisecondsLeft / 1000).ToString("F2");
                    });
                };

                _timer.Start();
            },
            p => IsQuizLoaded && !IsQuizStarted
        )));

        private ICommand endQuizCommand;
        public ICommand EndQuizCommand => endQuizCommand ??= new RelayCommand(
    p =>
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        Points = 0;

        foreach (var question in DisplayedQuestions)
        {
            var selected = question.Answers.Where(a => a.IsSelected).ToList();
            var correct = question.Answers.Where(a => a.IsCorrect).ToList();

            if (selected.Count == correct.Count && !selected.Except(correct).Any())
            {
                Points++;
            }
        }

        MessageBox.Show($"Quiz has ended!\nYour result: {Points} points.", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
    },
    p => IsQuizStarted
);


        private QuizModel loadedQuiz;
        private bool isQuizStarted;

        public ObservableCollection<Question> DisplayedQuestions { get; set; } = new ObservableCollection<Question>();

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

        private ICommand loadQuizCommand;
        public ICommand LoadQuizCommand => loadQuizCommand ??= new RelayCommand(
            p =>
            {
                loadedQuiz = QuizFileManager.LoadQuizFromFile();
                if (loadedQuiz != null)
                {
                    MessageBox.Show("Quiz has been successfully loaded.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    OnPropertyChanged(nameof(IsQuizLoaded));
                }
            },
            p => true
        );

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

    }
}
