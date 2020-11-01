using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Http;


namespace PictureGuessingGame.Pages
{
    struct GamePanel
	{
		public static Grid PanelGridRef;

		public static int lengthX = 0;
		public static int lengthY = 0;

		public Label thisPanel;


		public GamePanel(int posX, int posY)
		{
			thisPanel = new Label();
			thisPanel.FontSize = 8;
			thisPanel.Background = new SolidColorBrush(Color.FromRgb( 50, 50, 50));

			PanelGridRef.Children.Add(thisPanel);

			Grid.SetColumn(thisPanel, posX);
			Grid.SetRow(thisPanel, posY);
		}

		public void HidePanel()
		{
			thisPanel.Visibility = Visibility.Hidden;
		}

		public void UnhidePanel()
		{
			thisPanel.Visibility = Visibility.Visible;
		}
	}
	
	
	public partial class GamePage : Page
	{
		public static GameSession gameSession;

		GameRound roundInProgress;
		
		DispatcherTimer timer;
		TimeSpan timeSpan;

		int changeColorCounter = 0;

		int maxNumberOfLogs = 16;
		List<List<GamePanel>> gamePanels = new List<List<GamePanel>>();
		List<List<GamePanel>> coveringPanels;

		public GamePage(GameSession gameSession)
		{
			InitializeComponent();

			GamePage.gameSession = gameSession;
			ConstructGameBoard();
		}

		void ConstructGameBoard()
		{
			GamePanel.PanelGridRef = PanelGrid;

			// Add Columns (X)
			for (int AxysX = 0; AxysX < gameSession.boardSize.X; AxysX++)
				PanelGrid.ColumnDefinitions.Add(new ColumnDefinition());

			// Add Rows (Y)
			for (int AxysY = 0; AxysY < gameSession.boardSize.Y; AxysY++)
			{
				gamePanels.Add(new List<GamePanel>());
				PanelGrid.RowDefinitions.Add(new RowDefinition());
				// Add Tiles
				for (int AxysX = 0; AxysX < gameSession.boardSize.X; AxysX++)
					gamePanels[AxysY].Add(new GamePanel(AxysX, AxysY));
			}

			GamePanel.lengthX = gamePanels[0].Count();
			GamePanel.lengthY = gamePanels.Count();

			NextRound();
		}

		public void StartRound()
		{
			timeSpan = roundInProgress.timeToGuess;

			labelTimer.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

			// Timer set-up
			timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
			{
				labelTimer.Content = String.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
				if (timeSpan == TimeSpan.Zero)
				{
					RoundLost();
				}
				else if (timeSpan.Seconds % roundInProgress.unhideDelay.Seconds == 0)
				{
					HideGamePanel();
				}

				timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
			}, Application.Current.Dispatcher);

			timer.Start();

			// Enter affects submit button
			buttonSubmit.IsDefault = true;


			// Input box setup
			inputTextBox.Focus();

			guessLog.Text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n";
			buttonSubmit.Focusable = false;
		}

		private void HideAllGamePanels()
		{
			foreach (List<GamePanel> row in gamePanels)
			{
				foreach(GamePanel gamePanel in row)
				{
					gamePanel.HidePanel();
				}
			}
		}

		private void UnhideAllGamePanels()
		{
			foreach (List<GamePanel> row in gamePanels)
			{
				foreach (GamePanel gamePanel in row)
				{
					gamePanel.UnhidePanel();
				}
			}
		}

		private void HideGamePanel()
		{
			Random random = new Random();

			int randomRow;
			int randomColumn;

			bool bExit = true;
			foreach (var panelRow in coveringPanels)
			{
				if (panelRow.Count() > 0)
				{
					bExit = false;
					break;
				}
			}

			if (bExit)
				return;


			do
			{
				randomRow = random.Next(GamePanel.lengthY);
			} while (coveringPanels[randomRow].Count() < 1);

			randomColumn = random.Next(coveringPanels[randomRow].Count());

			// Randomness
			if (random.Next(0, 101) < 50)
			{
				do
				{
					randomRow = random.Next(GamePanel.lengthY);
				} while (coveringPanels[randomRow].Count() < 1);
				randomColumn = random.Next(coveringPanels[randomRow].Count());
			}

			if (random.Next(0, 1001) < 871)
			{
				do
				{
					randomRow = random.Next(GamePanel.lengthY);
				} while (coveringPanels[randomRow].Count() < 1);
				randomColumn = random.Next(coveringPanels[randomRow].Count());
			}
			
			if (random.Next(0, 1001) < random.Next(0, 501))
			{
				do
				{
					randomRow = random.Next(GamePanel.lengthY);
				} while (coveringPanels[randomRow].Count() < 1);
				randomColumn = random.Next(coveringPanels[randomRow].Count());
			}

			coveringPanels[randomRow][randomColumn].HidePanel();
			coveringPanels[randomRow].Remove(coveringPanels[randomRow][randomColumn]);
		}


		private void OnClickButtonSubmit(object sender, RoutedEventArgs e)
		{
			if (inputTextBox.Text != "" && timeSpan > TimeSpan.Zero && !roundInProgress.bIsWon)
			{
				string textToLog = "";
				int filledLines = 0;

				string[] test = guessLog.Text.Split('\n');

				foreach (string str in test)
				{
					if (str != String.Empty)
						filledLines++;
				}

				for (int line = filledLines + 1; line < maxNumberOfLogs; line++)
				{
					textToLog += "\n";
				}


				if (!(maxNumberOfLogs - filledLines - 1 >= maxNumberOfLogs - 1))
				{
					string tempString = String.Join("\n", test);
					if (textToLog.Length == 0)
						textToLog = tempString.Substring(test[0].Length + 1) + "\n" + inputTextBox.Text;
					else
						textToLog = textToLog + tempString.Substring(textToLog.Length + 1) + "\n" + inputTextBox.Text;

				}
				else
				{
					textToLog = textToLog + inputTextBox.Text;
				}

				guessLog.Text = textToLog;


				labelGuessCounter.Content = "Guess " + ++roundInProgress.numberOfGuesses;
				CheckCorrectAnswer(inputTextBox.Text);

				inputTextBox.Text = "";
			}
		}

		void CheckCorrectAnswer(string playerGuess)
		{
			try
			{
				string GuessReq = gameSession.GameRounds[gameSession.currentRound].roundID.ToString() + "/" + playerGuess;
				HttpResponseMessage request = PictureGuessing.client.GetAsync("http://77.244.251.110:81/api/games/" + GuessReq).Result;
				request.EnsureSuccessStatusCode();

				string answer = request.Content.ReadAsStringAsync().Result;

				if (answer == "true")
					RoundWon();
				else
					ChangeGuessColor();
			}
			catch (HttpRequestException e)
			{
				MessageBox.Show("\nException Caught!");
				MessageBox.Show("Message :{0} ", e.Message);
			}


			return;
		}

		async void RoundWon()
		{
			roundInProgress.bIsWon = true;

			timer.Stop();

			LabelAnswerLength.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0));
			LabelAnswerLength.Content = guessLog.Text.Split('\n').Last();

			// Show correct image
			HideAllGamePanels();
			await Task.Delay(2000);
			
			NextRound();
		}

		async void RoundLost()
		{
			timer.Stop();

			timeSpan = TimeSpan.Zero;
			labelTimer.Foreground = new SolidColorBrush(Color.FromRgb(200, 0, 0));

			HideAllGamePanels();
			await Task.Delay(2000);

			NextRound();
		}


		void NextRound()
		{
			UnhideAllGamePanels();

			// Deep copy of gamePanels List
			coveringPanels = new List<List<GamePanel>>();
			foreach (var AxysY in gamePanels)
			{
				coveringPanels.Add(new List<GamePanel>());
				foreach(var AxysX in AxysY)
				{
					coveringPanels[coveringPanels.Count() - 1].Add(AxysX);
				}
			}

			gameSession.currentRound++;

			if (gameSession.currentRound >= gameSession.GameRounds.Count)
			{
				NavigationService.Navigate(GameSetupPage.Singleton);
			}
			else
			{
				gameSession.GameRounds[gameSession.currentRound] = gameSession.GameRounds[gameSession.currentRound].InitializeGameRound(gameSession).Result;
				
				roundInProgress = gameSession.GameRounds[gameSession.currentRound];
				GuessingImage.Source = gameSession.GameRounds[gameSession.currentRound].GetImageFromURL().Source;

				// Show answer length
				string answerLengthText = "";
				for (int Loop = 0; Loop < gameSession.GameRounds[gameSession.currentRound].answerLength; Loop++)
					answerLengthText += "-";
				LabelAnswerLength.Content = answerLengthText;

				inputTextBox.Text = "";
				LabelAnswerLength.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
				changeColorCounter = 0;

				StartRound();
				labelGuessCounter.Content = "Guess " + roundInProgress.numberOfGuesses;
			}
		}

		// Keep focus on textbox
		private void OnLostFocusInputTextbox(object sender, RoutedEventArgs e)
		{
			inputTextBox.Focus();
		}

		async void ChangeGuessColor()
		{
			changeColorCounter++;
			LabelAnswerLength.Foreground = new SolidColorBrush(Color.FromRgb(200, 0, 0));
			await Task.Delay(2000);

			changeColorCounter--;
			if (changeColorCounter == 0)
				LabelAnswerLength.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
		}
	}
}