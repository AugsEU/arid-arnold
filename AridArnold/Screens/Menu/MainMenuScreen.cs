
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

		#endregion rType


		#region rMembers

		Layout mMenuLayout;
		Layout mBGLayout;
		
		Fade mScreenFade;

		#endregion rMembers



		#region rInit

		/// <summary>
		/// Initialise main menu.
		/// </summary>
		public MainMenuScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
		}



		/// <summary>
		/// Load base content for main menu.
		/// </summary>
		public override void LoadContent()
		{
			mMenuLayout = new Layout("Layouts/MainMenu.mlo");

			// Temp
			mBGLayout = new Layout("UI/Menu/SteamPlant/MenuBG.mlo");

			//mFader = new ScreenStars();

			base.LoadContent();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update main menu. Called every frame.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mMenuLayout.Update(gameTime);
			mBGLayout.Update(gameTime);
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

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		#endregion rDraw
	}
}
