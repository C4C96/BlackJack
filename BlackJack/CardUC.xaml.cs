using BlackJackLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
	/// CardUC.xaml 的交互逻辑
	/// </summary>
	public partial class CardUC : UserControl
	{
		private Card card;

		public Func<Task> BeforeReverse;
		public Action AfterReverse;

		public Card Card
		{
			get => card;
			set
			{
				if (card != null)
					card.PropertyChanged -= Card_PropertyChanged;
				card = value;

				DataContext = card;
				RefreshCover();
				RefreshBack();
				card.PropertyChanged += Card_PropertyChanged;
			}
		}

		public CardUC()
		{
			InitializeComponent();
			Card = new Card(Suit.Club, Rank.Ace, false);
		}

		public CardUC(Card card)
		{
			InitializeComponent();
			Card = card;			
		}

		private void RefreshCover()
		{
			CoverImage.Source = new BitmapImage(new Uri($@"./Images/Cards/{(int)card.Suit}-{(int)card.Rank}.png", UriKind.Relative));
		}

		private void RefreshBack()
		{
			BackImage.Visibility = card.Seen_Blind ? Visibility.Hidden : Visibility.Visible;
		}

		private void Card_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Seen_Blind":
					ReverseCardAsync();
					break;
				case "Suit":
				case "Rank":
					RefreshCover();
					break;
			}
		}

		/// <summary>
		/// 翻转卡牌（带动画）
		/// </summary>
		/// <param name="seen_blind">true表示翻到正面，false表示翻到反面</param>
		private async Task ReverseCardAsync()
		{
			if (BeforeReverse != null)
				await BeforeReverse();
			DoubleAnimation toZeroAnim = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.75));
			DoubleAnimation toOrignalAnim = new DoubleAnimation(Width, TimeSpan.FromSeconds(0.75));
			bool toZeroRunning = true;
			toZeroAnim.Completed += (o, e) =>
			{
				if (!toZeroRunning) return;
				toZeroRunning = false;
				RefreshBack();
				CoverImage.BeginAnimation(WidthProperty, toOrignalAnim, HandoffBehavior.Compose);
				BackImage.BeginAnimation(WidthProperty, toOrignalAnim, HandoffBehavior.Compose);
			};
			bool toOrignalRunning = true;
			toOrignalAnim.Completed += (o, e) =>
			{
				if (!toOrignalRunning) return;
				toOrignalRunning = false;
				if (AfterReverse != null)
					AfterReverse();
			};
			CoverImage.Width = BackImage.Width = Width;
			CoverImage.BeginAnimation(WidthProperty, toZeroAnim, HandoffBehavior.Compose);
			BackImage.BeginAnimation(WidthProperty, toZeroAnim, HandoffBehavior.Compose);
		}

	}
}
