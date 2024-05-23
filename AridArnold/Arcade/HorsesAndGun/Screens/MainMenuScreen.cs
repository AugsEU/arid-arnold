namespace HorsesAndGun
{
	internal class MainMenuScreen : Screen
	{
		public MainMenuScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
		{

		}

		public override void LoadContent(ContentManager content)
		{

		}

		public override void OnActivate()
		{

		}

		public override void OnDeactivate()
		{

		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(Color.SaddleBrown);

			info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);

			Color color = Color.White;

			info.spriteBatch.DrawString(pixelFont, "Horses & Gun.", new Vector2(30.0f, 40.0f), color);
			info.spriteBatch.DrawString(pixelFont, "Press enter to play.", new Vector2(30.0f, 290.0f), color);

			info.spriteBatch.End();

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			if (InputManager.I.KeyPressed(Keys.Enter) || InputManager.I.LClick())
			{
				ScreenManager.I.ActivateScreen(ScreenType.MainGame);
			}
		}
	}
}
