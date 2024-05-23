
using Microsoft.Xna.Framework.Graphics;

namespace HorsesAndGun
{
	internal class HorsesAndGun : AridArnold.ArcadeGame
	{
		Texture2D mCursor;

		public HorsesAndGun(GraphicsDeviceManager deviceManager, ContentManager content) : base(deviceManager, content)
		{
			mCursor = content.Load<Texture2D>("Arcade/HorsesAndGun/cursor");

			SoundManager.I.LoadContent(content);
			ScreenManager.I.LoadAllScreens(content, deviceManager);
			FontManager.I.LoadAllFonts(content);

			// ROSS BUTTON SCREEN FIRST FOR TESTING //

			ScreenManager.I.ActivateScreen(ScreenType.RossButtonsScreen);
		}

		public override void ResetGame()
		{
			SoundManager.I.StopMusic();
			ScreenManager.I.ActivateScreen(ScreenType.RossButtonsScreen);
			ScoreManager.I.ResetAll();
		}

		public override void Update(GameTime gameTime)
		{
			ScreenManager.I.Update(gameTime);
			TimeManager.I.Update(gameTime);
		}

		public override RenderTarget2D DrawToRenderTarget(AridArnold.DrawInfo info)
		{
			// These are actually different types. So convert them....
			DrawInfo frameInfo;

			frameInfo.graphics = info.graphics;
			frameInfo.spriteBatch = info.spriteBatch;
			frameInfo.gameTime = info.gameTime;
			frameInfo.device = info.device;

			RenderTarget2D screenTargetRef = null;

			//Draw active screen.
			Screen screen = ScreenManager.I.GetActiveScreen();

			if (screen != null)
			{
				screenTargetRef = screen.DrawToRenderTarget(frameInfo);

				// A TO DO: Remove this.
				MouseState mouseState = Mouse.GetState();
				Vector2 mouseScreenPoint = new Vector2(mouseState.Position.X - mCursor.Width / 2.0f, mouseState.Position.Y - mCursor.Height / 2.0f);
				info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);
				info.spriteBatch.Draw(mCursor, mouseScreenPoint, Color.White);
				info.spriteBatch.End();
			}

			return screenTargetRef;
		}
	}
}
