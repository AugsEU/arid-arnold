
namespace AridArnold
{
	/// <summary>
	/// The root menu for the game.
	/// </summary>
	internal class MainMenuScreen : Screen
	{
		public MainMenuScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
		}

		public override void Update(GameTime gameTime)
		{
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			return mScreenTarget;
		}
	}
}
