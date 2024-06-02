
namespace AridArnold
{
	/// <summary>
	/// Screen displayed for the arcade manuals
	/// </summary>
	internal class ArcadeManualsScreen : Screen
	{
		#region rConstants

		const float SCROLL_SPEED = 22.0f;
		const float MAX_SCROLL = 1288.0f;
		const double LERP_TIME = 900.0;
		const int NUM_MANUALS = 3;

		#endregion rConstants


		#region rMembers

		Texture2D mOverlay;
		Texture2D mSelector;
		Layout mManualPages;

		int mSelectedColumn;
		int mTargetColumn;
		float mDownwardsOffset;
		PercentageTimer mLerpTimer;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create arcade manual screen
		/// </summary>
		public ArcadeManualsScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mLerpTimer = new PercentageTimer(LERP_TIME);
		}


		/// <summary>
		/// Load screen textures
		/// </summary>
		public override void LoadContent()
		{
			mOverlay = MonoData.I.MonoGameLoad<Texture2D>("Arcade/Manuals/Overlay");
			mSelector = MonoData.I.MonoGameLoad<Texture2D>("Arcade/Manuals/Selector");
			mManualPages = new Layout("Arcade/Manuals/Manual.mlo");
			base.LoadContent();
		}

		public override void OnActivate()
		{
			mSelectedColumn = 0;
			mDownwardsOffset = 0.0f;
			base.OnActivate();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update the manuals screen
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			mManualPages.Update(gameTime);

			if (InputManager.I.KeyPressed(AridArnoldKeys.Pause))
			{
				ScreenManager.I.ActivateScreen(ScreenType.Game);
				return;
			}
			
			if(mLerpTimer.IsPlaying())
			{
				if(mLerpTimer.GetPercentageF() >= 1.0f)
				{
					mLerpTimer.FullReset();
					mSelectedColumn = mTargetColumn;
				}
				return;
			}

			if (InputManager.I.KeyHeld(AridArnoldKeys.ArnoldLeft) && mSelectedColumn > 0)
			{
				mTargetColumn = mSelectedColumn - 1;
				mLerpTimer.Start();
			}
			else if(InputManager.I.KeyHeld(AridArnoldKeys.ArnoldRight) && mSelectedColumn < NUM_MANUALS - 1)
			{
				mTargetColumn = mSelectedColumn + 1;
				mLerpTimer.Start();
			}
			else if(InputManager.I.KeyHeld(AridArnoldKeys.ArnoldUp))
			{
				mDownwardsOffset -= SCROLL_SPEED * dt;
			}
			else if (InputManager.I.KeyHeld(AridArnoldKeys.ArnoldDown))
			{
				mDownwardsOffset += SCROLL_SPEED * dt;
			}

			mDownwardsOffset = Math.Clamp(mDownwardsOffset, 0.0f, MAX_SCROLL);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the screen
		/// </summary>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			// Manuals
			Vector2 basePosition = MonoMath.Round(CalcManualsPosition());

			Camera screenCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
			screenCam.ForcePosition(basePosition);

			StartScreenSpriteBatch(info);

			// BG
			MonoDraw.DrawTexture(info, mOverlay, basePosition);

			// Selector
			Vector2 selectorPos = CalcSelectorPos();
			MonoDraw.DrawTexture(info, mSelector, basePosition + selectorPos);

			// Layout
			mManualPages.Draw(info);

			EndScreenSpriteBatch(info);
			return mScreenTarget;
		}



		/// <summary>
		/// Choose point to put
		/// </summary>
		Vector2 CalcSelectorPos()
		{
			return new Vector2(0.0f, 0.0f);
		}



		/// <summary>
		/// Calculate the manuals position.
		/// </summary>
		/// <returns></returns>
		Vector2 CalcManualsPosition()
		{
			float t = SlideLerp(mLerpTimer.GetPercentageF());

			Vector2 basePos = new Vector2(960.0f * mSelectedColumn, mDownwardsOffset - 89.0f);
			Vector2 finalPos = new Vector2(960.0f * mTargetColumn, mDownwardsOffset - 89.0f);

			return MonoMath.Lerp(basePos, finalPos, t);
		}


		/// <summary>
		/// Special lerp function to emulate slider
		/// </summary>
		float SlideLerp(float t)
		{
			t = t * t;
			t *= 2.4f - 1.4f * t;
			return t;
		}

		#endregion rDraw
	}
}
