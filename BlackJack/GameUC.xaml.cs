using BlackJackLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlackJack
{
    /// <summary>
    /// GameUC.xaml 的交互逻辑
    /// </summary>
    public partial class GameUC : UserControl, IGameInteraction
    {
		private Game game;

		private Rectangle[] playerCardAreaGroup = new Rectangle[Game.MAX_PLAYER];
		private PlayerUC[] playerUCGroup = new PlayerUC[Game.MAX_PLAYER];

		private List<CardUC> cardUCs = new List<CardUC>();
		private List<Image> fxs = new List<Image>(); // 特效的集合

		private SemaphoreSlim semaphore = new SemaphoreSlim(1); // 动画的信号量，同一时间只有一个动画

		public Game Game
		{
			get => game;
			set
			{
				if (game != null)
				{
					game.AchieveCard -= Game_AchieveCard;
					game.NewTurnStart -= Game_NewTurnStart;
					game.GamerBoom -= Game_GamerBoom;
					game.GamerBlackJack -= Game_GamerBlackJack;
					game.Finish -= Game_Finish;
					game.TurnFinish -= Game_TurnFinish;
				}
				foreach (var playerUC in playerUCGroup)
					playerUC.Player = null;
				DealerUC.Dealer = null;
				
				game = value;
				if (game != null)
				{
					game.AchieveCard += Game_AchieveCard;
					game.NewTurnStart += Game_NewTurnStart;
					game.GamerBoom += Game_GamerBoom;
					game.GamerBlackJack += Game_GamerBlackJack;
					game.Finish += Game_Finish;
					game.TurnFinish += Game_TurnFinish;

					int i = 0;
					foreach (var player in game.Players)
					{
						playerUCGroup[i++].Player = player;
					}
					DealerUC.Dealer = game.Dealer;
				}				
			}
		}

		private async void Game_TurnFinish(object sender, EventArgs e)
		{
			await semaphore.WaitAsync();
			semaphore.Release();

			NewTurnButton.Visibility = Visibility.Visible;
		}

		public GameUC()
		{
			InitializeComponent();
			playerCardAreaGroup[0] = PlayerCardArea1;
			playerCardAreaGroup[1] = PlayerCardArea2;
			playerCardAreaGroup[2] = PlayerCardArea3;
			playerCardAreaGroup[3] = PlayerCardArea4;
			playerCardAreaGroup[4] = PlayerCardArea5;
			playerUCGroup[0] = PlayerUC1;
			playerUCGroup[1] = PlayerUC2;
			playerUCGroup[2] = PlayerUC3;
			playerUCGroup[3] = PlayerUC4;
			playerUCGroup[4] = PlayerUC5;
		}

		private async void Game_Finish(object sender, Player player, bool? win)
		{
			Rectangle cardArea = playerCardAreaGroup[player.Id - 1];
			Uri uri;
			if (win == null)
				uri = new Uri(@"./Images/Draw.png", UriKind.Relative);
			else if (win.Value)
				uri = new Uri(@"./Images/Win.png", UriKind.Relative);
			else
				uri = new Uri(@"./Images/Lose.png", UriKind.Relative);
			await ShowImageOnCardArea(cardArea, uri);
		}

		private async void Game_GamerBoom(object sender, Gamer gamer)
		{
			Rectangle cardArea = gamer is Dealer ? DealerCardArea : playerCardAreaGroup[(gamer as Player).Id - 1];
			await ShowImageOnCardArea(cardArea, new Uri(@"./Images/boom.png", UriKind.Relative));
		}

		private async void Game_GamerBlackJack(object sender, Gamer gamer)
		{
			Rectangle cardArea = gamer is Dealer ? DealerCardArea : playerCardAreaGroup[(gamer as Player).Id - 1];
			await ShowImageOnCardArea(cardArea, new Uri(@"./Images/BlackJack.png", UriKind.Relative));
		}

		private async Task ShowImageOnCardArea(Rectangle cardArea, Uri imageUri)
		{
			Image image = new Image()
			{
				Source = new BitmapImage(imageUri),
				Width = 0,
				Height = 0,
			};
			Canvas.Children.Add(image);
			Canvas.SetLeft(image, Canvas.GetLeft(cardArea) + cardArea.Width / 2.0);
			Canvas.SetTop(image, Canvas.GetTop(cardArea) + cardArea.Height);
			fxs.Add(image);

			TimeSpan time = TimeSpan.FromSeconds(0.3);
			DoubleAnimation leftAnim = new DoubleAnimation(Canvas.GetLeft(cardArea), time);
			DoubleAnimation topAnim = new DoubleAnimation(Canvas.GetTop(cardArea), time);
			DoubleAnimation widthAnim = new DoubleAnimation(cardArea.Width, time);
			DoubleAnimation heightAnim = new DoubleAnimation(cardArea.Height, time);
			heightAnim.Completed += (o, e) => semaphore.Release();
			await semaphore.WaitAsync();

			image.BeginAnimation(Canvas.LeftProperty, leftAnim);
			image.BeginAnimation(Canvas.TopProperty, topAnim);
			image.BeginAnimation(WidthProperty, widthAnim);
			image.BeginAnimation(HeightProperty, heightAnim);
		}

		private void Game_NewTurnStart(object sender, EventArgs e)
		{
			DoubleAnimation removeAnim = new DoubleAnimation(-600, TimeSpan.FromSeconds(1));
			foreach (var cardUC in cardUCs)
			{
				removeAnim.Completed += (o, ev) => 
				{
					cardUC.Card = null;
					Canvas.Children.Remove(cardUC);
				};
				cardUC.BeginAnimation(Canvas.LeftProperty, removeAnim);
			}
			foreach (var image in fxs)
			{
				removeAnim.Completed += (o, ev) => Canvas.Children.Remove(image);
				image.BeginAnimation(Canvas.LeftProperty, removeAnim);
			}
		}

		private async void Game_AchieveCard(object sender, Gamer gamer, Card card)
		{ 
			CardUC cardUC = new CardUC()
			{
				Card = card,
				Width = CardTemp.ActualWidth,
				Height = CardTemp.ActualHeight,
				BeforeReverse = async () => await semaphore.WaitAsync(),
				AfterReverse = () => semaphore.Release(),
			};
			Canvas.Children.Add(cardUC);
			cardUCs.Add(cardUC);
			Canvas.SetLeft(cardUC, Canvas.GetLeft(CardTemp));
			Canvas.SetTop(cardUC, Canvas.GetTop(CardTemp));

			await semaphore.WaitAsync();

			var position = GetNewCardPos(gamer, card);
			DoubleAnimation xMoveAnim = new DoubleAnimation(position.Item1, TimeSpan.FromSeconds(1));
			DoubleAnimation yMoveAnim = new DoubleAnimation(position.Item2, TimeSpan.FromSeconds(1));
			yMoveAnim.Completed += (o, e) => semaphore.Release();
			cardUC.BeginAnimation(Canvas.LeftProperty, xMoveAnim);
			cardUC.BeginAnimation(Canvas.TopProperty, yMoveAnim);
		}

		private Tuple<Double, Double> GetNewCardPos(Gamer gamer, Card card)
		{
			int index = gamer.HandCards.IndexOf(card);
			Rectangle cardArea;
			if (gamer is Dealer)
				cardArea = DealerCardArea;	
			else
				cardArea = playerCardAreaGroup[(gamer as Player).Id - 1];
			double x = Canvas.GetLeft(cardArea);
			double y = Canvas.GetTop(cardArea);
			x += index * (cardArea.Width - CardTemp.ActualWidth) / 4.0;
			return new Tuple<double, double>(x, y);
		}

		public async Task<int> Bet(int playerId)
		{
			int? result = null;

			await semaphore.WaitAsync();
			semaphore.Release();

			playerUCGroup[playerId - 1].HighLight = true;
			BetGrid.Visibility = Visibility.Visible;
			BetTextBox.Focus();
			RoutedEventHandler handler = (o, e) => 
			{
				int input = 0;
				try
				{
					input = int.Parse(BetTextBox.Text);
				}
				catch
				{
					BetTextBox.Text = "";
				}
				if (input > 0)
					result = input;
				else
					BetTextBox.Text = "";
			};
			BetButton.Click += handler;
			do
			{
				await new ButtonAwaiter(BetButton);
			} while (result == null);
			BetButton.Click -= handler;
			BetGrid.Visibility = Visibility.Collapsed;
			playerUCGroup[playerId - 1].HighLight = false;
			return result.Value;
		}

		public async Task<bool> WantInsurance(int playerId)
		{
			bool? result = null;

			await semaphore.WaitAsync();
			semaphore.Release();

			playerUCGroup[playerId - 1].HighLight = true;
			InsuranceGrid.Visibility = Visibility.Visible;
			RoutedEventHandler positiveHandler = (o, e) => result = true;
			RoutedEventHandler negativeHandler = (o, e) => result = false;
			InsuranceButton.Click += positiveHandler;
			DontInsuranceButton.Click += negativeHandler;
			await new ButtonAwaiter(InsuranceButton, DontInsuranceButton);
			InsuranceButton.Click -= positiveHandler;
			DontInsuranceButton.Click -= negativeHandler;
			InsuranceGrid.Visibility = Visibility.Collapsed;
			playerUCGroup[playerId - 1].HighLight = false;
			return result.Value;
		}

		public async Task<bool> WantToDouble(int playerId)
		{
			bool? result = null;

			await semaphore.WaitAsync();
			semaphore.Release();

			playerUCGroup[playerId - 1].HighLight = true;
			DoubleGrid.Visibility = Visibility.Visible;
			RoutedEventHandler positiveHandler = (o, e) => result = true;
			RoutedEventHandler negativeHandler = (o, e) => result = false;
			DoubleButton.Click += positiveHandler;
			DontDoubleButton.Click += negativeHandler;
			await new ButtonAwaiter(DoubleButton, DontDoubleButton);
			DoubleButton.Click -= positiveHandler;
			DontDoubleButton.Click -= negativeHandler;
			DoubleGrid.Visibility = Visibility.Collapsed;
			playerUCGroup[playerId - 1].HighLight = false;
			return result.Value;
		}

		public async Task<bool> WantToHitMe(int playerId)
		{
			bool? result = null;

			await semaphore.WaitAsync();
			semaphore.Release();

			playerUCGroup[playerId - 1].HighLight = true;
			HitMeGrid.Visibility = Visibility.Visible;
			RoutedEventHandler positiveHandler = (o, e) => result = true;
			RoutedEventHandler negativeHandler = (o, e) => result = false;
			HitMeButton.Click += positiveHandler;
			StandButton.Click += negativeHandler;
			await new ButtonAwaiter(HitMeButton, StandButton);
			HitMeButton.Click -= positiveHandler;
			StandButton.Click -= negativeHandler;
			HitMeGrid.Visibility = Visibility.Collapsed;
			playerUCGroup[playerId - 1].HighLight = false;
			return result.Value;
		}

		private void NewTurnButton_Click(object sender, RoutedEventArgs e)
		{
			game.NextTurnAsync();
			NewTurnButton.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// 用于异步等待按钮被按的类
		/// </summary>
		public class ButtonAwaiter : INotifyCompletion
		{
			public Button[] Buttons { get; set; }

			public void OnCompleted(Action continuation)
			{
				RoutedEventHandler[] h = new RoutedEventHandler[Buttons.Length];
				for (int i = 0; i < Buttons.Length; i++)
				{
					h[i] = (o, e) =>
					{
						for (int j = 0; j < Buttons.Length; j++)
							Buttons[j].Click -= h[j];
						continuation();
					};
					Buttons[i].Click += h[i];
				}
			}

			public bool IsCompleted
			{ get => false; }

			public void GetResult()
			{ }

			public ButtonAwaiter(params Button[] buttons)
			{
				Buttons = buttons;
			}

			public ButtonAwaiter GetAwaiter()
			{
				return this;
			}
		}

	}
}
