
using Microsoft.Xna.Framework.Graphics;

namespace WormWarp
{
	class SnakeGameArcade : AridArnold.ArcadeGame
	{
		protected RenderTarget2D mScreenTarget;
		protected GraphicsDeviceManager mGraphics;

		MainGame mGameToEmulate;

		public SnakeGameArcade(GraphicsDeviceManager deviceManager, ContentManager content) : base(deviceManager, content)
		{
			mGraphics = deviceManager;
			mScreenTarget = new RenderTarget2D(deviceManager.GraphicsDevice, MainGame.SCREEN_WIDTH, MainGame.SCREEN_HEIGHT);
			mGameToEmulate = new MainGame();

			// Hack to emulate original env
			content.RootDirectory = "Content/Arcade/WormWarp";
			mGameToEmulate.LoadContent(content);
			content.RootDirectory = "Content";
		}

		public override void Update(GameTime gameTime)
		{
			if (GetState() == ArcadeGameState.kPlaying)
			{
				mGameToEmulate.Update(gameTime);
				if(mGameToEmulate.QuitRequested())
				{
					SetState(ArcadeGameState.kGameOver);
				}
			}
		}

		public override void ResetGame()
		{
			mGameToEmulate.ResetGame();
			base.ResetGame();
		}

		public override RenderTarget2D DrawToRenderTarget(AridArnold.DrawInfo info)
		{
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(Color.Black);
			info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);

			mGameToEmulate.Draw(info);

			info.spriteBatch.End();

			return mScreenTarget;
		}

		public override int GetScore()
		{
			return mGameToEmulate.GetScore();
		}
	}
}
