using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quiz.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool showingGenerator = true;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SwitchView_Click(object sender, RoutedEventArgs e)
    {
        showingGenerator = !showingGenerator;

        generatorControl.Visibility = showingGenerator ? Visibility.Visible : Visibility.Collapsed;
        solverControl.Visibility = showingGenerator ? Visibility.Collapsed : Visibility.Visible;
    }
}