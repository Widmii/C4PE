using System.Windows;
using System.Windows.Controls;


namespace PictureGuessingGame.Pages
{
    public partial class TitlePage : Page
    {
        public TitlePage()
        {
            InitializeComponent();
            ShowsNavigationUI = false;
        }

        // Redirects player to main menu
        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.MainMenuPage());
        }
    }
}
