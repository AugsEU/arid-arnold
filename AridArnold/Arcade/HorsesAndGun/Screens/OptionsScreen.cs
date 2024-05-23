using System;

namespace HorsesAndGun
{
	internal class OptionsScreen : Screen
	{
		Button mBackButton;
		Texture2D mBG;

		public OptionsScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
		{
		}

		public override void LoadContent(ContentManager content)
		{
			mBackButton = new Button(content.Load<Texture2D>("wooden_button"), content.Load<SpriteFont>("Fonts/Pixica"))
			{
				mPosition = new Vector2(366.5f, 290.0f),
				mText = "Back"
			};

			mBackButton.mOnMouseClick += BackButtonClick;

			mBG = content.Load<Texture2D>("help-screen");
		}

		public override void OnActivate()
		{

		}

		public override void OnDeactivate()
		{

		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(Color.SaddleBrown);

			info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);



			info.spriteBatch.Draw(mBG, Vector2.Zero, Color.White);
			mBackButton.Draw(info);
			info.spriteBatch.End();

			return mScreenTarget;
		}

		private void BackButtonClick(object sender, EventArgs e)
		{
			ScreenManager.I.ActivateScreen(ScreenType.RossButtonsScreen);
		}

		public override void Update(GameTime gameTime)
		{
			mBackButton.Update();
		}
	}
}
