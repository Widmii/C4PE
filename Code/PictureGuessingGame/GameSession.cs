using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using PictureGuessingGame.Pages;

namespace PictureGuessingGame
{
	public enum GameDifficulty { Easy, Medium, Hard, Custom}

	public struct GameRound
	{
		public Guid roundID;
		public string imageCategory;
		string imageURL;
		public int answerLength;

		public bool bIsWon;
		public int numberOfGuesses;
		public TimeSpan timeToGuess;

		public TimeSpan unhideDelay;


		public GameRound(TimeSpan timeToGuess, string category)
		{
			bIsWon = false;
			numberOfGuesses = 0;
			this.timeToGuess = timeToGuess;
			unhideDelay = new TimeSpan(0, 0, 0, 1, 0);
			imageCategory = category;


			imageURL = "";
			roundID = new Guid();
			answerLength = 0;
		}

		// Create Round (Get image to guess)
		public async Task<GameRound> InitializeGameRound(GameSession gameSession)
		{
			try
			{
				HttpResponseMessage request = PictureGuessing.client.PostAsJsonAsync("http://77.244.251.110:81/api/games", 
																					 new GameRoundInitObject
				{ GameDifficulty = Convert.ToSingle(gameSession.difficulty), Category = imageCategory}).Result;

				request.EnsureSuccessStatusCode();

				string answer = await request.Content.ReadAsStringAsync();
				
				JObject json = JObject.Parse(answer);

				
				roundID = (Guid)json.GetValue("gameId");
				imageURL = (string)json.GetValue("pictureURL");
				answerLength = (int)json.GetValue("answerLength");
			}
			catch (HttpRequestException e)
			{
				MessageBox.Show("\nException Caught!");
				MessageBox.Show("Message :{0} ", e.Message);
			}

			return this;
		}

		public System.Windows.Controls.Image GetImageFromURL()
		{
			var image = new System.Windows.Controls.Image();
			var fullFilePath = imageURL;

			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
			bitmap.EndInit();

			image.Source = bitmap;
			return image;
		}
	}


	public class GameSession
	{
		public int currentRound;
		public GameDifficulty difficulty;
		public string category;
		public Vector boardSize;

		public List<GameRound> GameRounds = new List<GameRound>();
			
			
		public GameSession(int numberOfRounds, GameDifficulty gameDifficulty, Vector gameBoardSize, string gameCategory, TimeSpan timeToGuess)
		{
			category = gameCategory;
			difficulty = gameDifficulty;
			boardSize = gameBoardSize;

			if (gameCategory == "All")
			{
				List<string> CategoryList = GameSetupPage.GetAllCategories().Result;
				Random random = new Random();

				for (int loop = 1; loop <= numberOfRounds; loop++)
				{
					int randomCategory = random.Next(0, CategoryList.Count);
					GameRounds.Add(new GameRound(timeToGuess, CategoryList[randomCategory]));
				}
			}
			else
			{
				for (int loop = 1; loop <= numberOfRounds; loop++)
				{
					GameRounds.Add(new GameRound(timeToGuess, gameCategory));
				}
			}
			
			currentRound = -1;
		}
		
	}

	public class GameRoundInitObject
	{
		public float GameDifficulty { get; set; }
		public string Category { get; set; }
	}
}