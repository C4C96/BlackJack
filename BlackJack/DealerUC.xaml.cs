using BlackJackLib;
using System;
using System.Collections.Generic;
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
	/// DealerUC.xaml 的交互逻辑
	/// </summary>
	public partial class DealerUC : UserControl
	{
		private Dealer dealer;

		public Dealer Dealer
		{
			get => dealer;
			set
			{
				if (dealer != null)
				{
					dealer.PropertyChanged -= Dealer_PropertyChanged;
					dealer.AchieveCardEvent -= Dealer_AchieveCardEvent;
				}
				dealer = value;
				if (dealer != null)
				{
					dealer.PropertyChanged += Dealer_PropertyChanged;
					dealer.AchieveCardEvent += Dealer_AchieveCardEvent;
					Balance.Content = dealer.Balance;
					RefreshSumPoint();
				}
			}
		}

		private void Dealer_AchieveCardEvent(object sender, Card card)
		{
			RefreshSumPoint();
			card.PropertyChanged += (o, e) => 
			{
				if (e.PropertyName == "Seen_Blind")
					RefreshSumPoint();
			};
		}

		private void RefreshSumPoint()
		{
			if (dealer.HandCards.Where((card) => !card.Seen_Blind).Count() > 0)
				SumPoint.Content = "----";
			else
				SumPoint.Content = dealer.SumPoint;
		}

		private void Dealer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Balance":
					Balance.Content = dealer.Balance;
					break;
			}
		}

		public DealerUC()
		{
			InitializeComponent();
		}
	}
}
