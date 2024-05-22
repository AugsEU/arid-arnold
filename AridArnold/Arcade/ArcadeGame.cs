namespace AridArnold
{
	/// <summary>
	/// Provides interface for running arcade games
	/// </summary>
	abstract class ArcadeGame
	{
		public ArcadeGame(GraphicsDeviceManager deviceManager, ContentManager content)
		{
		}

		public abstract void ResetGame();

		public abstract void Update(GameTime gameTime);

		public abstract RenderTarget2D DrawToRenderTarget(DrawInfo info);
	}
}
