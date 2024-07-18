
namespace AridArnold
{
	/// <summary>
	/// Screen displayed for the arcade manuals
	/// </summary>
	internal class ArcadeManualsScreen : Screen
	{
		#region rConstants

		const float SCROLL_SPEED = 42.0f;
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
			mLerpTimer.Update(gameTime);

			float dt = Util.GetDeltaT(gameTime);

			mManualPages.Update(gameTime);

			if (InputManager.I.KeyPressed(AridArnoldKeys.Pause))
			{
				ScreenManager.I.ActivateScreen(ScreenType.Game);
				return;
			}
			
			if(mLerpTimer.IsPlaying())
			{
				if (mLerpTimer.GetPercentageF() >= 1.0f)
				{
					mLerpTimer.FullReset();
					mSelectedColumn = mTargetColumn;
					mDownwardsOffset = 0.0f;
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

			StartScreenSpriteBatch(info, new Color(219, 209, 192));

			// Overlay
			MonoDraw.DrawTextureDepth(info, mOverlay, basePosition, DrawLayer.Front);

			// Selector
			Vector2 selectorPos = CalcSelectorPos();
			MonoDraw.DrawTextureDepth(info, mSelector, basePosition + selectorPos, DrawLayer.Front);

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
			float t = SlideLerp(mLerpTimer.GetPercentageF());
			Vector2 basePos = new Vector2(65.0f + 278.0f * mSelectedColumn, 14.0f);
			Vector2 finalPos = new Vector2(65.0f + 278.0f * mTargetColumn, 14.0f);
			return MonoMath.Lerp(basePos, finalPos, t);
		}



		/// <summary>
		/// Calculate the manuals position.
		/// </summary>
		/// <returns></returns>
		Vector2 CalcManualsPosition()
		{
			float slideT = SlideLerp(mLerpTimer.GetPercentageF());

			float downOffsetT = Math.Clamp(mLerpTimer.GetPercentageF() * 3.0f, 0.0f, 1.0f);
			float downOffset = (1.0f - MonoMath.SmoothZeroToOne(downOffsetT)) * mDownwardsOffset;

			Vector2 basePos = new Vector2(960.0f * mSelectedColumn, downOffset - 89.0f);
			Vector2 finalPos = new Vector2(960.0f * mTargetColumn, downOffset - 89.0f);

			return MonoMath.Lerp(basePos, finalPos, slideT);
		}


		/// <summary>
		/// Special lerp function to emulate slider
		/// </summary>
		float SlideLerp(float t)
		{
			const float a = 2.2f;
			if (mDownwardsOffset > 60.0f)
			{
				t *= 1.5f;
				t -= 0.5f;
			}
			t = Math.Clamp(t, 0.0f, 1.0f);
			t = t * t;
			t *= (a * t * t - (2 * a + 1.0f) * t + a + 2.0f);
			return t;
		}

		#endregion rDraw
	}
}
