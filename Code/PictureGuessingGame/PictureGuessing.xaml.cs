using System.Windows;
using System.Net.Http;

namespace PictureGuessingGame
{ 
    public partial class PictureGuessing : Window
    {
        public static readonly HttpClient client = new HttpClient();

        public PictureGuessing()
        {
            InitializeComponent();
        }

        // Displays the TitlePage
        private void PictureGuessing_Loaded(object sender, RoutedEventArgs e)
        {
            PictureGuessingFrame.NavigationService.Navigate(new Pages.TitlePage());
        }
    }
}
