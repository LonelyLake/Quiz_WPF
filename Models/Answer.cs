using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Quiz.Model
{
    public class Answer : INotifyPropertyChanged
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
        public bool IsEnabled { get; set; } = true;

        private string _backgroundColor = "Transparent";
        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Answer(string answerText, bool isCorrect)
        {
            AnswerText = answerText;
            IsCorrect = isCorrect;
            if (isCorrect)
            {
                BackgroundColor = "LightBlue";
            }
            else
            {
                BackgroundColor = "Pink";
            }
        }

        public override string ToString()
        {
            return $"{AnswerText} (Correct: {IsCorrect})";
        }
    }
}
