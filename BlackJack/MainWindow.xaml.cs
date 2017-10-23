using BlackJackLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private Game game;
		private GameUC gameUC;
		public MainWindow()
		{
			InitializeComponent();
			StartNewGame(1);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			gameUC.Game_AchieveCard(null, null, new Card(Suit.Club, Rank.Four));
		}

		private void StartNewGame(int playerNum)
		{
			gameUC = new GameUC();
			game = new Game(gameUC, playerNum);
			gameUC.Game = game;
			GameCanvas.Children.Clear();
			GameCanvas.Children.Add(gameUC);
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://baike.baidu.com/item/21点/5481683");
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			var window = new AboutWindow()
			{
				Owner = this,
			};
			window.ShowDialog();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			var result = MessageBox.Show("确定要退出吗？", "BlackJack", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.No)
				e.Cancel = true;
		}
	}
}
