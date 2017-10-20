namespace BlackJackLib
{
	/// <summary>
	/// 为Game类提供交互信息的接口
	/// </summary>
	public interface IGameInteraction
	{
		/// <summary>
		/// 询问赌金金额
		/// </summary>
		/// <param name="playerId">询问的玩家的Id</param>
		/// <returns>赌金金额，大于0</returns>
		int Bet(int playerId);

		/// <summary>
		/// 询问是否需要买保险金
		/// </summary>
		/// <param name="playerId">询问的玩家的Id</param>
		/// <returns>是否需要买保险金</returns>
		bool WantInsurance(int playerId);

		/// <summary>
		/// 询问玩家是否需要Double
		/// </summary>
		/// <param name="playerId">询问的玩家的Id</param>
		/// <returns>是否需要Double</returns>
		bool WantToDouble(int playerId);

		/// <summary>
		/// 询问玩家是否需要继续要牌
		/// </summary>
		/// <param name="playerId">询问的玩家的Id</param>
		/// <returns>是否需要继续要牌</returns>
		bool WantToHitMe(int playerId);
	}
}
