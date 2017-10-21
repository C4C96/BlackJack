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
    /// GameUC.xaml 的交互逻辑
    /// </summary>
    public partial class GameUC : UserControl, IGameInteraction
    {
		private Game game;

        public GameUC(Game game)
        {
            InitializeComponent();
			this.game = game;
			game.AchieveCard += Game_AchieveCard;
			game.NewTurnStart += Game_NewTurnStart;
			game.GamerBoom += Game_GamerBoom;
			game.Finish += Game_Finish;
        }

		private void Game_Finish(object sender, Player player, bool? win)
		{
			throw new NotImplementedException();
		}

		private void Game_GamerBoom(object sender, Gamer e)
		{
			throw new NotImplementedException();
		}

		private void Game_NewTurnStart(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Game_AchieveCard(object sender, Gamer gamer, Card card)
		{
			throw new NotImplementedException();
		}

		public int Bet(int playerId)
		{
			throw new NotImplementedException();
		}

		public bool WantInsurance(int playerId)
		{
			throw new NotImplementedException();
		}

		public bool WantToDouble(int playerId)
		{
			throw new NotImplementedException();
		}

		public bool WantToHitMe(int playerId)
		{
			throw new NotImplementedException();
		}
	}
}
