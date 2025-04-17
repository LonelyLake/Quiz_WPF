using Quiz.ViewModels;
using Quiz.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Navigation;
using Quiz.Services;

namespace Quiz
{ 
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // utworzenie obiektu odpowiedzialnego za nawigację
            var navigationService = new Quiz.Services.NavigationService();

            // utworzenie modelu widoku dla widoku startowego
            var quizGeneratorViewModel = new QuizGeneratorViewModel();
            //navigationService.NavigateTo<HomeViewModel>();
            navigationService.NavigateTo(quizGeneratorViewModel);

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(navigationService)
            };

            mainWindow.Show();
        }
    }
}