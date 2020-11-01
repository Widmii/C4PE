using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace PictureGuessingGame.Pages
{
    public partial class GameSetupPage : Page
    {
		public static object Singleton;

		public GameSetupPage()
		{
			InitializeComponent();
			Singleton = this;

			// Create dynamic Combobox
			TopicsComboBox.Items.Add("All");
			List<string> CategoryList = GetAllCategories().Result;
			foreach (string str in CategoryList)
			{
				TopicsComboBox.Items.Add(str);
			}

			// Set default combo box values
			TopicsComboBox.SelectedIndex = 0;
			DifficultyComboBox.SelectedIndex = 0;
			BoardSizeComboBox.SelectedIndex = 0;
		}

		private void StartButtonClick(object sender, RoutedEventArgs e)
		{
			// User input values
			string selectedCategory;
			GameDifficulty selectedDifficulty;
			TimeSpan selecteLengthOfRound;
			int selectedNumberOfRounds;
			Vector selectedBoardSize;


			selectedCategory = TopicsComboBox.SelectedItem.ToString();
			selectedDifficulty = (GameDifficulty)Enum.Parse(typeof(GameDifficulty), DifficultyComboBox.SelectedIndex.ToString());
			
			selecteLengthOfRound = (
				LenghtOfRoundComboBox.SelectedIndex == 0 ? new TimeSpan(0, 1, 0) :
				LenghtOfRoundComboBox.SelectedIndex == 1 ? new TimeSpan(0, 2, 0) :
				LenghtOfRoundComboBox.SelectedIndex == 2 ? new TimeSpan(0, 3, 0) :
				LenghtOfRoundComboBox.SelectedIndex == 3 ? new TimeSpan(0, 5, 0) : new TimeSpan(0, 8, 0));

			selectedNumberOfRounds = (
				RoundsPerGameComboBox.SelectedIndex == 0 ? 1 :
				RoundsPerGameComboBox.SelectedIndex == 1 ? 2 :
				RoundsPerGameComboBox.SelectedIndex == 2 ? 3 :
				RoundsPerGameComboBox.SelectedIndex == 3 ? 5 :
				RoundsPerGameComboBox.SelectedIndex == 4 ? 10 : 15);


			switch (BoardSizeComboBox.SelectedIndex)
			{
				case 0:
					selectedBoardSize = new Vector(5, 5);
					break;
				case 1:
					selectedBoardSize = new Vector(10, 10);
					break;
				case 2:
					selectedBoardSize = new Vector(15, 15);
					break;
				case 3:
					selectedBoardSize = new Vector(20, 20);
					break;
				case 4:
					selectedBoardSize = new Vector(30, 30);
					break;
				case 5:
					selectedBoardSize = new Vector(40, 40);
					break;
				default:
					selectedBoardSize = new Vector(1, 1);
					break;
			}



			GameSession gameSession = new GameSession(selectedNumberOfRounds, selectedDifficulty, selectedBoardSize, 
													  selectedCategory, selecteLengthOfRound);

			NavigationService.Navigate(new Pages.GamePage(gameSession));
		}

		private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (DifficultyComboBox.SelectedIndex)
			{
				case 0: //Easy difficulty
					LenghtOfRoundComboBox.SelectedIndex = 3;
					RoundsPerGameComboBox.SelectedIndex = 1;
					BoardSizeComboBox.SelectedIndex = 0;
					break;

				case 1: //Medium difficulty
					LenghtOfRoundComboBox.SelectedIndex = 2;
					RoundsPerGameComboBox.SelectedIndex = 2;
					BoardSizeComboBox.SelectedIndex = 1;
					break;

				case 2: //Hard difficulty
					LenghtOfRoundComboBox.SelectedIndex = 1;
					RoundsPerGameComboBox.SelectedIndex = 3;
					BoardSizeComboBox.SelectedIndex = 2;
					break;
			}
		}

		private void LenghtOfRoundComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DifficultyPresetsVerification();
		}

		private void RoundsPerGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DifficultyPresetsVerification();
		}

		private void BoardSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DifficultyPresetsVerification();
		}

		private void DifficultyPresetsVerification()
		{
			//Easy - Change the difficulty to easy if the selection matches the preset
			if (LenghtOfRoundComboBox.SelectedIndex == 3 && RoundsPerGameComboBox.SelectedIndex == 1 && BoardSizeComboBox.SelectedIndex == 0)
			{
				DifficultyComboBox.SelectedIndex = 0;
			}
			//Medium - Change the difficulty to medium if the selection matches the preset
			else if(LenghtOfRoundComboBox.SelectedIndex == 2 && RoundsPerGameComboBox.SelectedIndex == 2 && BoardSizeComboBox.SelectedIndex == 1)
			{
				DifficultyComboBox.SelectedIndex = 1;
			}
			//Hard - Change the difficulty to hard if the selection matches the preset
			else if(LenghtOfRoundComboBox.SelectedIndex == 1 && RoundsPerGameComboBox.SelectedIndex == 3 && BoardSizeComboBox.SelectedIndex == 2)
			{
				DifficultyComboBox.SelectedIndex = 2;
			}
			else
			{
				DifficultyComboBox.SelectedIndex = 3;
			}
		}

		private void BackButtonClick(object sender, RoutedEventArgs e)
		{
			this.NavigationService.Navigate(MainMenuPage.Singleton);
		}

		static public async Task<List<string>> GetAllCategories()
		{
			List<string> Result = new List<string>();

			// Call asynchronous network methods in a try/catch block to handle exceptions
			try
			{
				HttpResponseMessage response = PictureGuessing.client.GetAsync("http://77.244.251.110:81/api/categories").Result;
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();

				JArray jarray = JArray.Parse(responseBody);
				
				foreach (var Element in jarray.Children<JObject>())
				{
					Result.Add((string)Element.GetValue("category"));
				}

				// Remove the non-user categories
				Result.RemoveRange(Result.Count - 2, 2);
			}
			catch (HttpRequestException e)
			{
				MessageBox.Show("\nException Caught!");
				MessageBox.Show("Message :{0} ", e.Message);
				return new List<string>();
			}

			return Result;
		}

	}

}
