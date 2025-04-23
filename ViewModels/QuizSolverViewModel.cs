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
            p => true
        )));

        private ICommand endQuizCommand;
        public ICommand EndQuizCommand => (endQuizCommand ?? (endQuizCommand = new RelayCommand(
            p =>
            {
                if (_timer != null && millisecondsLeft > 0)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }
            },
            p => true
        )));

        


    }
}
