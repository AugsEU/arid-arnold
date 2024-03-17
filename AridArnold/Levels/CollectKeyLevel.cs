using System;

namespace AridArnold
{
	/// <summary>
	/// Level where you collect a flag to win.
	/// </summary>
	class CollectKeyLevel : Level
	{
		#region rInitialisation

		/// <summary>
		/// Collect flag level construct
		/// </summary>
		public CollectKeyLevel(AuxData auxData, int id) : base(auxData, id)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update, checks for win condition
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		protected override void UpdateInternal(GameTime gameTime)
		{
			if (EventManager.I.IsSignaled(EventType.KeyCollect))
			{
				mLevelStatus = LevelStatus.Win;
			}
		}

		#endregion rUpdate
	}
}