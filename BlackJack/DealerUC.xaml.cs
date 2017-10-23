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
					dealer.PropertyChanged -= Dealer_PropertyChanged;
				dealer = value;
				if (dealer != null)
				{
					dealer.PropertyChanged += Dealer_PropertyChanged;
					Balance.Content = dealer.Balance;
				}
			}
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
