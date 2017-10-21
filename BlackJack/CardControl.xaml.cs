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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlackJack
{
	/// <summary>
	/// Card.xaml 的交互逻辑
	/// </summary>
	public partial class CardControl : UserControl
	{
		/// <summary>
		/// 是否是翻面状态
		/// </summary>
		public bool UpsideDown
		{
			get
			{
				return BackImage.Visibility == Visibility.Visible;
			}
			set
			{
				BackImage.Visibility = value ? Visibility.Visible : Visibility.Hidden;
			}
		}

		private void RefreshCover()
		{
			CoverImage.Source = new BitmapImage(new Uri($@"./Images/Cards/{(int)Suit}-{(int)Rank}.png", UriKind.Relative));
		}

		public CardControl(Suit suit, Rank rank)
		{
			InitializeComponent();
			Suit = suit;
			Rank = rank;
			UpsideDown = false;
		}
	}
}
