namespace AridArnold
{
	/// <summary>
	/// Provides interface for running arcade games
	/// </summary>
	abstract class ArcadeGame
	{
		public enum ArcadeGameState
		{
			kNotPlaying,
			kPlaying,
			kGameOver
		}

		ArcadeGameState mCurrState;

		public ArcadeGame(GraphicsDeviceManager deviceManager, ContentManager content)
		{
			mCurrState = ArcadeGameState.kNotPlaying;
		}

		public virtual void ResetGame()
		{
			mCurrState = ArcadeGameState.kPlaying;
		}

		public ArcadeGameState GetState()
		{
			return mCurrState;
		}

		protected void SetState(ArcadeGameState state)
		{
			mCurrState = state;
		}

		public abstract void Update(GameTime gameTime);

		public abstract ulong GetScore();

		public abstract string GetMusicID();

		public abstract RenderTarget2D DrawToRenderTarget(DrawInfo info);
	}
}
