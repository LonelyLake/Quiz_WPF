using Quiz.Services;
using Quiz.ViewModels.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quiz.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged("CurrentViewModel"); }
        }

        public ICommand NavigateQuizGeneratorCommand { get; }

        public ICommand NavigateQuizSolverCommand { get; }

        public MainViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.SetNavigator(vm => CurrentViewModel = vm);

            ViewModelBase qgvm = new QuizGeneratorViewModel();
            ViewModelBase qsvm = new QuizSolverViewModel();        
            
            NavigateQuizGeneratorCommand = new RelayCommand(_ => _navigationService.NavigateTo(qgvm), _ => true);
            NavigateQuizSolverCommand = new RelayCommand(_ => _navigationService.NavigateTo(qsvm), _ => true);

            CurrentViewModel = qgvm;
        }
    }
}
