using Quiz.ViewModels.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quiz.ViewModels;

namespace Quiz.Services
{
    public class NavigationService
    {
        private Action<ViewModelBase> _navigate;
        private ViewModelBase _currentViewModel;

        public void SetNavigator(Action<ViewModelBase> navigator)
        {
            _navigate = navigator;
        }

        public void NavigateTo(ViewModelBase newViewModel)
        {
            // Jeśli aktualny ViewModel to QuizSolverViewModel — zresetuj timer
            if (_currentViewModel is QuizSolverViewModel quizSolverVM)
            {
                if(quizSolverVM.IsQuizStarted)
                {
                    quizSolverVM.EndQuiz();
                }
            }

            _currentViewModel = newViewModel;
            _navigate?.Invoke(newViewModel);
        }
    }
}
