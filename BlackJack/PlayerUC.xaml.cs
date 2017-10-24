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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlackJack
{
	/// <summary>
	/// PlayerUC.xaml 的交互逻辑
	/// </summary>
	public partial class PlayerUC : UserControl
	{
		private Player player;

		private bool highLight;

		public Player Player
		{
			get => player;
			set
			{
				if (player != null)
				{
					player.PropertyChanged -= Player_PropertyChanged;
					player.AchieveCardEvent -= Player_AchieveCardEvent;
				}
				player = value;
				if (player != null)
				{
					player.PropertyChanged += Player_PropertyChanged;
					player.AchieveCardEvent += Player_AchieveCardEvent;
					Balance.Content = player.Balance;
					Insurance.Content = player.Insurance == null ? "----" : player.Insurance.ToString();
					Stake.Content = player.Stake;
					SumPoint.Content = player.SumPoint;
				}
			}
		}

		private void Player_AchieveCardEvent(object sender, Card e)
		{
			SumPoint.Content = player.SumPoint;
		}

		public bool HighLight
		{
			get => highLight;
			set
			{
				highLight = value;
				if (highLight)
					PlayerGrid.Background = new SolidColorBrush(Colors.GreenYellow);
				else
					PlayerGrid.Background = new SolidColorBrush(Colors.SpringGreen);
			}
		}

		private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Balance":
					Balance.Content = player.Balance;
					break;
				case "Insurance":
					Insurance.Content = player.Insurance == null ? "----" : player.Insurance.ToString();
					break;
				case "Stake":
					Stake.Content = player.Stake;
					break;
			}
		}

		public PlayerUC()
		{
			InitializeComponent();
		}
	}
}
