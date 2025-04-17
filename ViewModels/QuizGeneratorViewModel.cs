using Quiz.ViewModels.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModels
{
    public class QuizGeneratorViewModel : ViewModelBase
    {
        public string WelcomeMessage => "Witamy w aplikacji!";

        private int _fontSize;
        public int FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }
    }
}
