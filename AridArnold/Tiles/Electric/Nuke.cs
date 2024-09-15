namespace AridArnold
{
	internal class Nuke : SquareTile
	{
		#region rConstants

		const double KILL_ALL_TIME = 4000.0;
		const double READY_TO_BLOW_TIME = 1200.0;
		const int NUM_EXPLOSION_POINT = 5;

		#endregion rConstants





		#region rMembers

		MonoTimer mExplodeTimer;
		bool mHasKilledAll;
		bool mHasPushedRumble;

		Animator mReadyToBlowAnim;

		GameSFX mWarningSFX;
		GameSFX mExplodeSFX;

		Vector2[] mExplodePoints;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Make nuke
		/// </summary>
		public Nuke(Vector2 position) : base(position)
		{
			mHasKilledAll = false;
		}



		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/NukeOff");
			mReadyToBlowAnim = new Animator(Animator.PlayType.Repeat, ("Tiles/Lab/NukeOn", 0.2f), ("Tiles/Lab/NukeOff", 0.2f));
			mExplodeTimer = new MonoTimer();

			mWarningSFX = new GameSFX(AridArnoldSFX.NukeAlarm, 0.1f);
			mWarningSFX.GetBuffer().SetLoop(true);
			mExplodeSFX = new GameSFX(AridArnoldSFX.NukeExplode, 0.6f);

			MonoRandom rng = RandomManager.I.GetDraw();
			Rect2f possiblePointRect = GetExplodeRect(GetCentre(), 150.0f);
			mExplodePoints = new Vector2[NUM_EXPLOSION_POINT];
			mExplodePoints[0] = GetCentre();
			for (int i = 1; i < mExplodePoints.Length; i++)
			{
				mExplodePoints[i] = rng.PointIn(possiblePointRect);
			}
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update nuke
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mExplodeTimer.Update(gameTime);

			if (!mExplodeTimer.IsPlaying())
			{
				//Check surrounding tiles.
				EMField.ScanResults scan = TileManager.I.GetEMField().ScanAdjacent(mTileMapIndex);

				if (scan.mTotalPositiveElectric > 0.75f && !mExplodeTimer.IsPlaying())
				{
					mExplodeTimer.Start();
					mReadyToBlowAnim.Play();
					SFXManager.I.PlaySFX(mWarningSFX, 0.0f);
				}
			}
			else
			{
				if (mExplodeTimer.GetElapsedMs() > READY_TO_BLOW_TIME && !mHasPushedRumble)
				{
					Camera gameCamera = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
					gameCamera.QueueMovement(new CameraShake(35.0f, 7.0f, 166.0f));
					mHasPushedRumble = true;

					SFXManager.I.PlaySFX(mExplodeSFX);
				}

				if (mExplodeTimer.GetElapsedMs() > KILL_ALL_TIME && !mHasKilledAll)
				{
					KillAll();
				}
			}



			mReadyToBlowAnim.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Explode
		/// </summary>
		public void KillAll()
		{
			for (int i = 0; i < EntityManager.I.GetEntityNum(); i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				entity.Kill();
			}

			mHasKilledAll = true;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get the nuke texture
		/// </summary>
		public override Texture2D GetTexture()
		{
			if (mExplodeTimer.IsPlaying())
			{
				return mReadyToBlowAnim.GetCurrentTexture();
			}

			return mTexture;
		}

		/// <summary>
		/// Draw the nuke effect
		/// </summary>
		public override void DrawExtra(DrawInfo info)
		{
			if (mExplodeTimer.GetElapsedMs() > READY_TO_BLOW_TIME)
			{
				float t = (float)(mExplodeTimer.GetElapsedMs() - READY_TO_BLOW_TIME) / 100.0f;

				for (int i = 0; i < mExplodePoints.Length; i++)
				{
					float expT = t - i * 2.0f;
					if (expT > 0.0f)
					{
						DrawExplositionAt(info, mExplodePoints[i], t);
					}
				}
			}

			base.DrawExtra(info);
		}


		private void DrawExplositionAt(DrawInfo info, Vector2 centre, float t)
		{
			float mainExplodeT = t * t * t * 0.2f;
			Rect2f mainExplodeRect = GetExplodeRect(centre, mainExplodeT);
			MonoDraw.DrawRectDepth(info, mainExplodeRect, new Color(240, 240, 240), DrawLayer.Projectiles);

			for (float pt = t; pt > 0.0f; pt -= 1.4f)
			{
				float extraExplodeT = pt * pt * pt * 0.6f;
				Rect2f extraExplodeRect = GetExplodeRect(centre,extraExplodeT);

				float colorT = Math.Clamp(pt / 14.0f, 0.0f, 1.0f);
				Color explodeColor = MonoMath.Lerp(Color.WhiteSmoke, Color.LightGray, colorT);
				explodeColor.A = 255;

				MonoDraw.DrawRectHollow(info, extraExplodeRect, 1.0f + colorT * 2.0f, explodeColor, explodeColor, new Vector2(1.0f, 1.0f), DrawLayer.Bubble);
			}
		}


		private Rect2f GetExplodeRect(Vector2 centre, float t)
		{
			t *= 0.5f;
			Vector2 min = centre - new Vector2(t, t);
			Vector2 max = centre + new Vector2(t, t);
			return new Rect2f(min, max);
		}

		#endregion rDraw
	}
}
