using System.Windows;
using System.Windows.Controls;


namespace PictureGuessingGame.Pages
{
    public partial class MainMenuPage : Page
    {
        public static object Singleton;
        public MainMenuPage()
        {
            InitializeComponent();
            Singleton = this;
        }

        // Redirects you to Game setup
        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Pages.GameSetupPage());
        }

        // Shutsdown the App
        private void QuitButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
