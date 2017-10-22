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
    /// Dealer.xaml 的交互逻辑
    /// </summary>
    public partial class DealerUC : UserControl
    {
		private Dealer dealer;

		public Dealer Dealer
		{
			get => dealer;
			set
			{
				dealer = value;
				DataContext = dealer;
			}
		}

        public DealerUC()
        {
            InitializeComponent();
        }
    }
}
