
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
			kFadeOut,
		}

		#endregion rType





		#region rConstants

		const double FADE_TIME = 1200.0;
		const double TRANSITION_TIME = 1100.0;
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
		Layout mBGLayout;

		// Area transition
		MainMenuArea mCurrentMenuArea;
		MainMenuArea mNextMenuArea;
		PercentageTimer mTransitionTimer;


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
			SetFadeState(FadeState.kFadeIn);
			FinishTransitionTo(MainMenuArea.kMainArea);

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
			UpdateScreenFade(gameTime);
		}



		/// <summary>
		/// Update screen fade logic
		/// </summary>
		public void UpdateScreenFade(GameTime gameTime)
		{
			switch (mFadeState)
			{
				case FadeState.kFadeIn:
					if(mFadeTimer.GetPercentageF() >= 1.0f)
					{
						mFadeState = FadeState.kActive;
						mFadeTimer.Stop();
					}
					break;
				case FadeState.kActive:
					break;
				case FadeState.kFadeOut:
					break;
				default:
					throw new NotImplementedException();
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
			string msgStr = msg.mMessage;
			if(msgStr.StartsWith("go"))
			{
				string enumStr = msgStr.Substring(2);
				MainMenuArea newArea = MonoAlg.GetEnumFromString<MainMenuArea>(enumStr);
				QueueAreaTransition(newArea);
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
					mMenuLayout.SetSelectedElement("newGameBtn");
					break;
				case MainMenuArea.kNewGameArea:
					mMenuLayout.SetSelectedElement("speedRunOpt");
					break;
				case MainMenuArea.kLoadGameArea:
					break;
				case MainMenuArea.kOptionsArea:
					mMenuLayout.SetSelectedElement("visionOpt");
					break;
				case MainMenuArea.kRebindArea:
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
				case FadeState.kFadeIn:
					mScreenFade.DrawAtTime(info, 1.0f - mFadeTimer.GetPercentageF());
					break;
				case FadeState.kActive:
					break;
				case FadeState.kFadeOut:
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
		void SetFadeState(FadeState state)
		{
			mFadeState = state;
			mFadeTimer.FullReset();
			mFadeTimer.Start();
		}

		#endregion rUtility
	}
}
