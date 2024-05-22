namespace GMTK2023
{
	/// <summary>
	/// Gameplay screen
	/// </summary>
	internal class GameScreen : Screen
	{
		public static Rectangle PLAYABLE_AREA = new Rectangle(0, 135, 960, 370);
		public const double READY_TIME = 1500.0;
		public const double GO_TIME = 500.0;
		const double WIN_TIME = 1500.0;
		const double LOSS_TIME = 1500.0;

		#region rMembers

		// Textures
		Texture2D mBG;
		Texture2D mSkyline;
		Texture2D mHealthBarBG;
		Texture2D mHealthBarSegStart;
		Texture2D mHealthBarSegMid;
		Texture2D mHealthBarSegEnd;

		Player mPlayer;

		MonoTimer mReadyGoTimer;
		PercentageTimer mWinTimer;
		PercentageTimer mLossTimer;

		bool mIsPaused = false;

		#endregion rMembers



		#region rInit

		public GameScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			FXManager.I.Init(SCREEN_WIDTH, SCREEN_HEIGHT);
			mReadyGoTimer = new MonoTimer();
			mWinTimer = new PercentageTimer(WIN_TIME);
			mLossTimer = new PercentageTimer(LOSS_TIME);

		}

		public override void LoadContent()
		{
			mBG = MonoData.I.MonoGameLoad<Texture2D>("Backgrounds/GameScreen");
			mSkyline = MonoData.I.MonoGameLoad<Texture2D>("Backgrounds/Skyline");

			mHealthBarBG = MonoData.I.MonoGameLoad<Texture2D>("UI/HealthBar");
			mHealthBarSegStart = MonoData.I.MonoGameLoad<Texture2D>("UI/HealthBarSegStart");
			mHealthBarSegMid = MonoData.I.MonoGameLoad<Texture2D>("UI/HealthBarSegMid");
			mHealthBarSegEnd = MonoData.I.MonoGameLoad<Texture2D>("UI/HealthBarSegEnd");


			base.LoadContent();
		}

		public override void OnActivate()
		{
			FXManager.I.Clear();
			mReadyGoTimer.FullReset();
			mReadyGoTimer.Start();

			if (RunManager.I.HasStarted() == false)
			{
				RunManager.I.StartRun();
				SoundManager.I.PlayMusic(SoundManager.MusicType.MainGame, 0.55f);
			}

			EntityManager.I.ClearEntities();
			AITargetManager.I.Init();

			SpawnInitialEntities();
			base.OnActivate();

			mWinTimer.FullReset();
			mLossTimer.FullReset();
		}


		void SpawnInitialEntities()
		{
			RandomManager.I.GetWorld().ChugNumber(DateTime.Now.Millisecond);

			AITargetManager.I.Init();
			Vector2 playerSpawn = new Vector2(100.0f, 400.0f);
			mPlayer = new Player(playerSpawn, 0.0f);

			EntityManager.I.RegisterEntity(mPlayer);
			AITargetManager.I.RegisterPos(playerSpawn);

			mPlayer.SetHealth(RunManager.I.GetHealth());

			int numToSpawn = RunManager.I.GetNumberOfEnemies();

			for (int i = 0; i < numToSpawn; i++)
			{
				Vector2 pos = AITargetManager.I.GiveMeATarget();
				pos.X = (pos.X - SCREEN_WIDTH) / 2.0f + SCREEN_WIDTH;

				AIEntity newEntity = new AIEntity(pos, MathF.PI);
				EntityManager.I.RegisterEntity(newEntity);
			}

			AITargetManager.I.Init();
		}

		#endregion rInit



		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			if (InputManager.I.KeyPressed(GameKeys.Pause))
			{
				mIsPaused = !mIsPaused;
			}

			if (mIsPaused)
			{
				return;
			}

			FXManager.I.Update(gameTime);

			if (mLossTimer.IsPlaying() || mWinTimer.IsPlaying())
			{
				if (mLossTimer.GetPercentageF() >= 1.0f)
				{
					ScreenManager.I.ActivateScreen(ScreenType.GameOver);
				}
				else if (mWinTimer.GetPercentageF() >= 1.0f)
				{
					ScreenManager.I.ActivateScreen(ScreenType.Game);
				}

				return;
			}

			if (mReadyGoTimer.GetElapsedMs() > GetReadyTime())
			{
				EntityManager.I.Update(gameTime);
			}

			CheckForWinOrLoss();
		}


		void CheckForWinOrLoss()
		{
			if (EntityManager.I.GetAllOfType(typeof(Player)).Count == 0)
			{
				LoseGame();
				return;
			}

			List<Entity> aiBots = EntityManager.I.GetAllOfType(typeof(AIEntity));
			foreach (AIEntity aIEntity in aiBots)
			{
				if (aIEntity.GetTeam() == AITeam.Enemy)
				{
					return;
				}
			}

			WinRound();
		}

		void WinRound()
		{
			mWinTimer.Start();
			RunManager.I.EndRound(mPlayer);
		}

		void LoseGame()
		{
			mLossTimer.Start();
			RunManager.I.EndRun();
		}

		#endregion rUpdate



		#region rDraw

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			StartScreenSpriteBatch(info);

			float scrollAmount = -mPlayer.GetCentrePos().X / 5.0f;
			float scrollAmountSky = -mPlayer.GetCentrePos().X / 50.0f;

			Vector2 startBG = new Vector2(scrollAmount - mBG.Width, 0.0f);
			Vector2 startSky = new Vector2(scrollAmountSky - mSkyline.Width, 0.0f);

			for (int i = 0; i < 3; i++)
			{
				MonoDraw.DrawTextureDepth(info, mBG, startBG, DrawLayer.Background);
				MonoDraw.DrawTextureDepth(info, mSkyline, startSky, DrawLayer.BackgroundElement);
				startBG.X += mBG.Width;
				startSky.X += mSkyline.Width;
			}




			EntityManager.I.Draw(info);

			DrawUI(info);

			FXManager.I.Draw(info);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}


		public void DrawUI(DrawInfo info)
		{
			SpriteFont font = FontManager.I.GetFont("Scream-36");
			Vector2 centre = new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT) * 0.5f;

			if (mIsPaused)
			{
				MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), new Color(10, 10, 10, 100), DrawLayer.Text);
				MonoDraw.DrawShadowStringCentred(info, font, centre, Color.White, "Paused", DrawLayer.Text);
			}
			else
			{
				if (mReadyGoTimer.GetElapsedMs() < GetReadyTime() + GO_TIME)
				{
					DrawReadyGoText(info);
				}

				if (mWinTimer.IsPlaying())
				{
					float flash = mWinTimer.GetPercentageF() % 0.5f;
					Color textColor = Color.White;
					if (flash > 0.25f)
					{
						textColor = Color.Yellow;
					}
					MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), new Color(10, 10, 10, 100), DrawLayer.Text);
					MonoDraw.DrawShadowStringCentred(info, font, centre, textColor, "Round Won", DrawLayer.Text);
				}
				else if (mLossTimer.IsPlaying())
				{
					float flash = mLossTimer.GetPercentageF() % 0.5f;
					Color textColor = Color.Orange;
					if (flash > 0.25f)
					{
						textColor = Color.Red;
					}
					MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), new Color(10, 10, 10, 100), DrawLayer.Text);
					MonoDraw.DrawShadowStringCentred(info, font, centre, textColor, "Round Lost", DrawLayer.Text);
				}
			}

			DrawHealthBar(info, new Vector2(44.0f, 18.0f));
		}

		public void DrawHealthBar(DrawInfo info, Vector2 topLeft)
		{
			int health = mPlayer.GetHealth();

			MonoDraw.DrawTextureDepth(info, mHealthBarBG, topLeft, DrawLayer.Text);

			for (int i = 0; i < health; i++)
			{
				Texture2D segTexture = mHealthBarSegMid;
				if (i == 0)
				{
					segTexture = mHealthBarSegStart;
				}
				else if (i == Player.MAX_HEALTH - 1)
				{
					segTexture = mHealthBarSegEnd;
				}

				MonoDraw.DrawTextureDepth(info, segTexture, topLeft, DrawLayer.Text);

				topLeft.X += segTexture.Width;
			}

		}


		public void DrawReadyGoText(DrawInfo info)
		{

			SpriteFont font = FontManager.I.GetFont("Scream-36");
			double time = mReadyGoTimer.GetElapsedMs();
			string text = "Ready?";
			Vector2 pos = new Vector2(SCREEN_WIDTH / 2.0f, 0.0f);

			if (time > GetReadyTime())
			{
				text = "GO!";
				pos.Y = SCREEN_HEIGHT / 2.0f;
			}
			else
			{
				float t = MathF.Min((float)(time / GetReadyTime()) * 2.0f, 1.0f);
				pos.Y = t * SCREEN_HEIGHT / 2.0f;
				MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), new Color(10, 10, 10, 100), DrawLayer.Text);
			}

			pos.Y += 25.0f;
			MonoDraw.DrawShadowStringCentred(info, font, pos, Color.Wheat, text, DrawLayer.Text);
			pos.Y -= 75.0f;
			MonoDraw.DrawShadowStringCentred(info, font, pos, Color.Wheat, "Round: " + (RunManager.I.GetRounds() + 1), DrawLayer.Text);
		}

		#endregion rDraw


		double GetReadyTime()
		{
			return RunManager.I.GetRounds() == 0 ? 4000.0 : READY_TIME;
		}


	}
}
