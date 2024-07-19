
namespace AridArnold
{
	/// <summary>
	/// The root menu for the game.
	/// </summary>
	internal class MainMenuScreen : Screen
	{
		#region rType

		/// <summary>
		/// Represents the different areas of the menu.
		/// </summary>
		public enum MainMenuArea
		{
			kMainArea,
			kNewGameArea,
			kLoadGameArea,
			kOptionsArea,
			kNumAreas
		}

		/// <summary>
		/// Handle fade in/fade out states
		/// </summary>
		public enum MainMenuState
		{
			kFadeIn,
			kActive,
			kFadeOut,
		}

		#endregion rType





		#region rConstants

		const double FADE_TIME = 2000.0;

		#endregion rConstants





		#region rMembers

		Layout mMenuLayout;
		Layout mBGLayout;
		
		Fade mScreenFade;
		PercentageTimer mFadeTimer;
		MainMenuState mFadeState;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Initialise main menu.
		/// </summary>
		public MainMenuScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mFadeTimer = new PercentageTimer(FADE_TIME);
		}



		/// <summary>
		/// Load base content for main menu.
		/// </summary>
		public override void LoadContent()
		{
			mMenuLayout = new Layout("Layouts/MainMenu.mlo");

			// Temp
			mBGLayout = new Layout("UI/Menu/SteamPlant/MenuBG.mlo");

			mScreenFade = new ScreenStars(10.0f, SCREEN_RECTANGLE);

			base.LoadContent();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Called when the screen is activated.
		/// </summary>
		public override void OnActivate()
		{
			SetFadeState(MainMenuState.kFadeIn);

			base.OnActivate();
		}



		/// <summary>
		/// Update main menu. Called every frame.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mMenuLayout.Update(gameTime);
			mBGLayout.Update(gameTime);

			mFadeTimer.Update(gameTime);
			UpdateScreenFade(gameTime);
		}



		/// <summary>
		/// Update screen fade logic
		/// </summary>
		public void UpdateScreenFade(GameTime gameTime)
		{
			switch (mFadeState)
			{
				case MainMenuState.kFadeIn:
					if(mFadeTimer.GetPercentageF() >= 1.0f)
					{
						mFadeState = MainMenuState.kActive;
						mFadeTimer.Stop();
					}
					break;
				case MainMenuState.kActive:
					break;
				case MainMenuState.kFadeOut:
					break;
				default:
					throw new NotImplementedException();
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Render main menu to target
		/// </summary>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			StartScreenSpriteBatch(info);

			mMenuLayout.Draw(info);
			mBGLayout.Draw(info);
			DrawFade(info);

#if DEBUG
			// Debug stuff
			MonoDebug.DrawDebugRects(info);
#endif

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}



		/// <summary>
		/// Draw the fade in/out
		/// </summary>
		void DrawFade(DrawInfo info)
		{
			switch (mFadeState)
			{
				case MainMenuState.kFadeIn:
					mScreenFade.DrawAtTime(info, 1.0f - mFadeTimer.GetPercentageF());
					break;
				case MainMenuState.kActive:
					break;
				case MainMenuState.kFadeOut:
					mScreenFade.DrawAtTime(info, mFadeTimer.GetPercentageF());
					break;
				default:
					throw new NotImplementedException();
			}
		}

		#endregion rDraw





		#region rUtility

		void SetFadeState(MainMenuState state)
		{
			mFadeState = state;
			mFadeTimer.FullReset();
			mFadeTimer.Start();
		}

		#endregion rUtility
	}
}
