
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
			kMainArea = 0,
			kNewGameArea = 1,
			kLoadGameArea = 2,
			kOptionsArea = 3,
			kRebindArea = 4,
			kNumAreas
		}

		/// <summary>
		/// Handle fade in/fade out states
		/// </summary>
		public enum FadeState
		{
			kFadeIn,
			kActive,
			kFadeOutNewScreen,
			kFadeOutNewCampaign,
		}

		#endregion rType





		#region rConstants

		const float BG_SCALE_FACTOR = 2.0f;
		const double FADE_TIME = 1200.0;
		const double TRANSITION_TIME = 900.0;
		static Vector2[] MENU_AREAS_POSITIONS =
		{
			new Vector2(0.0f, 0.0f),		/* kMainArea */
			new Vector2(0.0f, -540.0f),		/* kNewGameArea */
			new Vector2(-960.0f, 0.0f),		/* kLoadGameArea */
			new Vector2(0.0f, 540.0f),		/* kOptionsArea */
			new Vector2(0.0f, 1080.0f),		/* kRebindArea */
		};

		#endregion rConstants





		#region rMembers

		Layout mMenuLayout;

		// BG
		RenderTarget2D mBGTarget;
		Layout mBGLayout;

		// Area transition
		MainMenuArea mCurrentMenuArea;
		MainMenuArea mNextMenuArea;
		PercentageTimer mTransitionTimer;
		ScreenType mNextScreen = ScreenType.MainMenu;

		// Campaign load
		string mPendingCampaignLoad = null;


		// Fade
		Fade mScreenFade;
		PercentageTimer mFadeTimer;
		FadeState mFadeState;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Initialise main menu.
		/// </summary>
		public MainMenuScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mFadeTimer = new PercentageTimer(FADE_TIME);
			mTransitionTimer = new PercentageTimer(TRANSITION_TIME);

			mBGTarget = new RenderTarget2D(graphics.GraphicsDevice, SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
		}



		/// <summary>
		/// Load base content for main menu.
		/// </summary>
		public override void LoadContent()
		{
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
			SaveManager.I.ScanExistingProfiles();

			// Reload the menu
			mMenuLayout = new Layout("Layouts/MainMenu.mlo");

			StartFade(FadeState.kFadeIn);
			FinishTransitionTo(MainMenuArea.kMainArea);

			Camera cameraBG = CameraManager.I.GetCamera(CameraManager.CameraInstance.MenuBGCamera);
			cameraBG.ForcePosition(Vector2.Zero);

			MusicManager.I.RequestTrackPlay("MainMenu");


			base.OnActivate();
		}



		/// <summary>
		/// Update main menu. Called every frame.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mMenuLayout.Update(gameTime);
			mBGLayout.Update(gameTime);

			ProcessAllMessages();
			UpdateTransition(gameTime);

			mFadeTimer.Update(gameTime);
			UpdateScreenFade();
		}



		/// <summary>
		/// Update screen fade logic
		/// </summary>
		public void UpdateScreenFade()
		{
			// Move the fade to the camera
			if(mFadeTimer.IsPlaying())
			{
				Camera myCamera = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
				Vector2 position = myCamera.GetCurrentSpec().mPosition;
				Rectangle screenRectWithCamera = new Rectangle((int)position.X, (int)position.Y, SCREEN_WIDTH, SCREEN_HEIGHT);
				mScreenFade.SetArea(screenRectWithCamera);
			}

			if (mFadeTimer.GetPercentageF() >= 1.0f)
			{
				// Do thing at end.
				switch (mFadeState)
				{
					case FadeState.kFadeIn:
						mFadeState = FadeState.kActive;
						break;
					case FadeState.kActive:
						break;
					case FadeState.kFadeOutNewScreen:
						ScreenManager.I.ActivateScreen(mNextScreen);
						break;
					case FadeState.kFadeOutNewCampaign:
						StartNewCampaign(mPendingCampaignLoad);
						break;
					default:
						throw new NotImplementedException();
				}

				mFadeTimer.FullReset();
			}
		}

		#endregion rUpdate





		#region rMessages

		/// <summary>
		/// Process all messages from the layout
		/// </summary>
		void ProcessAllMessages()
		{
			ElementMsg? currentMsg = mMenuLayout.PopMessage();
			while(currentMsg != null)
			{
				ProcessMessage(currentMsg.Value);
				currentMsg = mMenuLayout.PopMessage();
			}
		}



		/// <summary>
		/// Process single message
		/// </summary>
		void ProcessMessage(ElementMsg msg)
		{
			string msgHeader = msg.mHeader;
			string msgStr = msg.mMessage;

			switch (msgHeader)
			{
				case "go": // Go to sub-screen
					MainMenuArea newArea = MonoEnum.GetEnumFromString<MainMenuArea>(msgStr);
					QueueAreaTransition(newArea);
					break;
				case "sc": // Go to screen
					mNextScreen = MonoEnum.GetEnumFromString<ScreenType>(msgStr);
					StartFade(FadeState.kFadeOutNewScreen);
					break;
				case "lo": // Load campaign
					MusicManager.I.StopMusic(300.0);
					mPendingCampaignLoad = msgStr;
					StartFade(FadeState.kFadeOutNewCampaign);
					break;
				default:
					break;
			}
		}

		#endregion rMessages





		#region rMenuArea

		/// <summary>
		/// Handle update for transition
		/// </summary>
		void UpdateTransition(GameTime gameTime)
		{
			mTransitionTimer.Update(gameTime);

			if(mTransitionTimer.IsPlaying())
			{
				mMenuLayout.SetSelectionBlocker(true);

				// Lerp camera
				LerpTransitionCam(gameTime);

				if(mTransitionTimer.GetPercentageF() >= 1.0f)
				{
					FinishTransitionTo(mNextMenuArea);
				}
			}
		}



		/// <summary>
		/// Called when transition is over
		/// </summary>
		void FinishTransitionTo(MainMenuArea area)
		{
			mTransitionTimer.FullReset();
			mNextMenuArea = area;
			mCurrentMenuArea = area;

			// Unblock selection
			mMenuLayout.SetSelectionBlocker(false);

			switch (area)
			{
				case MainMenuArea.kMainArea:
					SaveManager.I.SaveGlobalSettings();// Just got back from options menu or something. Good time to save.
					mMenuLayout.SetSelectedElement("newGameBtn");
					break;
				case MainMenuArea.kNewGameArea:
					mMenuLayout.SetSelectedElement("profileNameOpt");
					break;
				case MainMenuArea.kLoadGameArea:
					mMenuLayout.SetSelectedElement("saveFileLstTop");
					break;
				case MainMenuArea.kOptionsArea:
					mMenuLayout.SetSelectedElement("visionOpt");
					break;
				case MainMenuArea.kRebindArea:
					mMenuLayout.SetSelectedElement("upRbnd");
					break;
				case MainMenuArea.kNumAreas:
					break;
			}
		}

		/// <summary>
		/// Lerp the transition timer
		/// </summary>
		void LerpTransitionCam(GameTime gameTime)
		{
			float t = mTransitionTimer.GetPercentageF();
			t = MonoMath.LeapZeroToSmoothOne(t);
			Vector2 start = MENU_AREAS_POSITIONS[(int)mCurrentMenuArea];
			Vector2 end = MENU_AREAS_POSITIONS[(int)mNextMenuArea];

			Vector2 pos = MonoMath.Lerp(start, end, t);

			Camera camera = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
			camera.ForcePosition(pos);

			Camera cameraBG = CameraManager.I.GetCamera(CameraManager.CameraInstance.MenuBGCamera);
			cameraBG.ForcePosition(pos / BG_SCALE_FACTOR);
		}



		/// <summary>
		/// Queue up a transition
		/// </summary>
		/// <param name="newArea"></param>
		void QueueAreaTransition(MainMenuArea newArea)
		{
			if (mTransitionTimer.IsPlaying())
			{
				return;
			}

			mNextMenuArea = newArea;
			mTransitionTimer.ResetStart();
		}

		#endregion rMenuArea





		#region rDraw

		/// <summary>
		/// Render main menu to target
		/// </summary>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			DrawBGToTarget(info);

			StartScreenSpriteBatch(info);

			Vector2 cameraPos = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera).GetCurrentSpec().mPosition;
			MonoDraw.DrawTextureDepthScale(info, mBGTarget, cameraPos, BG_SCALE_FACTOR, DrawLayer.Background);

			mMenuLayout.Draw(info);
			DrawFade(info);

#if DEBUG
			// Debug stuff
			MonoDebug.DrawDebugRects(info);
#endif

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}


		/// <summary>
		/// Draw background to the render target
		/// </summary>
		void DrawBGToTarget(DrawInfo info)
		{
			Camera camBG = CameraManager.I.GetCamera(CameraManager.CameraInstance.MenuBGCamera);
			camBG.StartSpriteBatch(info, new Vector2(mBGTarget.Width, mBGTarget.Height), mBGTarget, Color.Black);

			mBGLayout.Draw(info);

			camBG.EndSpriteBatch(info);
		}


		/// <summary>
		/// Draw the fade in/out
		/// </summary>
		void DrawFade(DrawInfo info)
		{
			switch (mFadeState)
			{
				case FadeState.kFadeIn:
					mScreenFade.DrawAtTime(info, 1.0f - mFadeTimer.GetPercentageF());
					break;
				case FadeState.kActive:
					break;
				case FadeState.kFadeOutNewScreen:
				case FadeState.kFadeOutNewCampaign:
					mScreenFade.DrawAtTime(info, mFadeTimer.GetPercentageF());
					break;
				default:
					throw new NotImplementedException();
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Enter fade transition
		/// </summary>
		void StartFade(FadeState state)
		{
			mFadeState = state;
			mFadeTimer.ResetStart();
			UpdateScreenFade();
		}



		/// <summary>
		/// Load a campaign
		/// </summary>
		void StartNewCampaign(string campaignName)
		{
			// Reset everything...
			Main.DefaultGameplayManagers();

			// Create campaign from meta file
			CampaignManager.I.BeginCampaign(campaignName);

			// Activate the gameplay screen.
			ScreenManager.I.ActivateScreen(ScreenType.Game);

			// Check for opening cutscene
			CampaignManager.I.CheckCinematicTriggers(CinematicTrigger.TriggerType.Opening);
		}

		#endregion rUtility
	}
}
