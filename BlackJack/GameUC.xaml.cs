using BlackJackLib;
using System;
using System.Collections.Generic;
using System.Linq;
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
					game.Finish -= Game_Finish;
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
					game.Finish += Game_Finish;

					int i = 0;
					foreach (var player in game.Players)
						playerUCGroup[i++].Player = player;
					DealerUC.Dealer = game.Dealer;
				}				
			}
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

		private void Game_Finish(object sender, Player player, bool? win)
		{
			NewTurnButton.Visibility = Visibility.Visible;
		}

		private void Game_GamerBoom(object sender, Gamer e)
		{
			// TODO
		}

		private void Game_NewTurnStart(object sender, EventArgs e)
		{
			DoubleAnimation removeAnim = new DoubleAnimation(-CardTemp.ActualWidth, TimeSpan.FromSeconds(1));
			foreach (var cardUC in cardUCs)
			{
				removeAnim.Completed += (o, ev) => Canvas.Children.Remove(cardUC);
				cardUC.BeginAnimation(Canvas.LeftProperty, removeAnim);
			}
		}

		private async void Game_AchieveCard(object sender, Gamer gamer, Card card)
		{
			bool completed = false;

			CardUC cardUC = new CardUC();
			Canvas.Children.Add(cardUC);
			cardUCs.Add(cardUC);
			Canvas.SetLeft(cardUC, Canvas.GetLeft(CardTemp));
			Canvas.SetTop(cardUC, Canvas.GetTop(CardTemp));
			cardUC.Width = CardTemp.ActualWidth;
			cardUC.Height = CardTemp.ActualHeight;

			var position = GetNewCardPos(gamer);
			DoubleAnimation xMoveAnim = new DoubleAnimation(position.Item1, TimeSpan.FromSeconds(1));
			DoubleAnimation yMoveAnim = new DoubleAnimation(position.Item2, TimeSpan.FromSeconds(1));
			yMoveAnim.Completed += (o, e) =>
			{
				if (card.Seen_Blind)
				{
					card.Seen_Blind = false;
					cardUC.Card = card;
					card.Seen_Blind = true;
				}
				else
					cardUC.Card = card;
				completed = true;
			};
			cardUC.BeginAnimation(Canvas.LeftProperty, xMoveAnim);
			cardUC.BeginAnimation(Canvas.TopProperty, yMoveAnim);
			await Task.Factory.StartNew(() => { while (!completed || cardUC.IsRotating) ; });
		}

		private Tuple<Double, Double> GetNewCardPos(Gamer gamer)
		{
			int count = gamer.HandCards.Count;
			Rectangle cardArea;
			if (gamer is Dealer)
				cardArea = DealerCardArea;	
			else
			{
				int index = (gamer as Player).Id - 1;
				cardArea = playerCardAreaGroup[index];
			}
			double x = Canvas.GetLeft(cardArea);
			double y = Canvas.GetTop(cardArea);
			x += (count - 1) * (cardArea.Width - CardTemp.ActualWidth) / 4.0;
			return new Tuple<double, double>(x, y);
		}

		public async Task<int> Bet(int playerId)
		{
			int? result = null;
			playerUCGroup[playerId - 1].HighLight = true;
			BetGrid.Visibility = Visibility.Visible;			
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
			await Task.Factory.StartNew(() => { while (result == null) ; });
			BetButton.Click -= handler;
			BetGrid.Visibility = Visibility.Collapsed;
			playerUCGroup[playerId - 1].HighLight = false;
			return result.Value;
		}

		public async Task<bool> WantInsurance(int playerId)
		{
			bool? result = null;
			playerUCGroup[playerId - 1].HighLight = true;
			InsuranceGrid.Visibility = Visibility.Visible;
			RoutedEventHandler positiveHandler = (o, e) => result = true;
			RoutedEventHandler negativeHandler = (o, e) => result = false;
			InsuranceButton.Click += positiveHandler;
			DontInsuranceButton.Click += negativeHandler;
			await Task.Factory.StartNew(() => { while (result == null) ; });
			InsuranceButton.Click -= positiveHandler;
			DontInsuranceButton.Click -= negativeHandler;
			InsuranceGrid.Visibility = Visibility.Collapsed;
			playerUCGroup[playerId - 1].HighLight = false;
			return result.Value;
		}

		public async Task<bool> WantToDouble(int playerId)
		{
			bool? result = null;
			playerUCGroup[playerId - 1].HighLight = true;
			DoubleGrid.Visibility = Visibility.Visible;
			RoutedEventHandler positiveHandler = (o, e) => result = true;
			RoutedEventHandler negativeHandler = (o, e) => result = false;
			DoubleButton.Click += positiveHandler;
			DontDoubleButton.Click += negativeHandler;
			await Task.Factory.StartNew(() => { while (result == null) ; });
			DoubleButton.Click -= positiveHandler;
			DontDoubleButton.Click -= negativeHandler;
			DoubleGrid.Visibility = Visibility.Collapsed;
			playerUCGroup[playerId - 1].HighLight = false;
			return result.Value;
		}

		public async Task<bool> WantToHitMe(int playerId)
		{
			bool? result = null;
			playerUCGroup[playerId - 1].HighLight = true;
			HitMeGrid.Visibility = Visibility.Visible;
			RoutedEventHandler positiveHandler = (o, e) => result = true;
			RoutedEventHandler negativeHandler = (o, e) => result = false;
			HitMeButton.Click += positiveHandler;
			StandButton.Click += negativeHandler;
			await Task.Factory.StartNew(() => { while (result == null) ; });
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
	}
}
