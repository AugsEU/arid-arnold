using System;
using System.Collections.Generic;

namespace HorsesAndGun
{
	internal class MainGameScreen : Screen
	{
		const int NUM_LANES = 5;

		//Textures
		Texture2D mBackground;
		Texture2D mGameOverCross;
		Texture2D[] mDiceTextures;
		Texture2D[] mSideDiceTextures;
		ScrollingImage mTopSky;
		ScrollingImage mTopSkyCloud1;
		ScrollingImage mTopSkyCloud2;
		ScrollingImage mGround;

		//Gun
		int mGunLane;
		Texture2D mGunTexture;
		Texture2D mGunBarrelTexture;
		Animator mShootAnim;
		MonoTimer mGunReloadTimer;
		double mGunReloadTime;
		const double NORMAL_RELOAD_TIME = 3000.0;

		//Dice
		DiceQueue mDiceQueue;

		//Tiles
		TrackManager mTrackManager;

		//Game Start
		MonoTimer mReadyTimer;
		const double mReadyTime = 2000.0;

		//Score
		MonoTimer mScoreTimer;
		bool mNewHighScore;

		//Game over
		List<Vector2> mGameOverPoints;
		MonoTimer mGameOverFadeTimer;
		const double mGameOverFadeTime = 2400.0;



		public MainGameScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
		{
			mDiceQueue = new DiceQueue();
			mDiceTextures = new Texture2D[6];
			mSideDiceTextures = new Texture2D[6];
			mTrackManager = new TrackManager(content);
			mGunReloadTimer = new MonoTimer();
			mShootAnim = new Animator(Animator.PlayType.OneShot);
			mGameOverFadeTimer = new MonoTimer();
			mReadyTimer = new MonoTimer();
			mScoreTimer = new MonoTimer();
			mGunLane = 0;
		}

		public override void LoadContent(ContentManager content)
		{
			mBackground = content.Load<Texture2D>("main_bg");
			mGunTexture = content.Load<Texture2D>("gun");
			mGunBarrelTexture = content.Load<Texture2D>("gun_barrel");
			mGameOverCross = content.Load<Texture2D>("dead_x");

			mTopSky = new ScrollingImage(content.Load<Texture2D>("sky_1"), content.Load<Texture2D>("sky_2"), Vector2.Zero, 70);
			mTopSkyCloud1 = new ScrollingImage(content.Load<Texture2D>("dust_cloud_1"), content.Load<Texture2D>("dust_cloud_1"), Vector2.Zero, 130);
			mTopSkyCloud2 = new ScrollingImage(content.Load<Texture2D>("dust_cloud_2"), content.Load<Texture2D>("dust_cloud_2"), Vector2.Zero, 130);
			mGround = new ScrollingImage(content.Load<Texture2D>("ground"), content.Load<Texture2D>("ground"), new Vector2(84.0f, 27.0f), 200);

			mDiceTextures[0] = content.Load<Texture2D>("Dice1");
			mDiceTextures[1] = content.Load<Texture2D>("Dice2");
			mDiceTextures[2] = content.Load<Texture2D>("Dice3");
			mDiceTextures[3] = content.Load<Texture2D>("Dice4");
			mDiceTextures[4] = content.Load<Texture2D>("Dice5");
			mDiceTextures[5] = content.Load<Texture2D>("Dice6");


			mSideDiceTextures[0] = content.Load<Texture2D>("dice_side_1");
			mSideDiceTextures[1] = content.Load<Texture2D>("dice_side_2");
			mSideDiceTextures[2] = content.Load<Texture2D>("dice_side_3");
			mSideDiceTextures[3] = content.Load<Texture2D>("dice_side_4");
			mSideDiceTextures[4] = content.Load<Texture2D>("dice_side_5");
			mSideDiceTextures[5] = content.Load<Texture2D>("dice_side_6");

			mShootAnim.LoadFrame(content, "gun-fire1", 0.05f);
			mShootAnim.LoadFrame(content, "gun-fire2", 0.05f);
			mShootAnim.LoadFrame(content, "gun-fire3", 0.05f);
			mShootAnim.LoadFrame(content, "gun-fire4", 0.07f);
			mShootAnim.LoadFrame(content, "gun-fire5", 0.07f);
			mShootAnim.LoadFrame(content, "gun-fire6", 0.07f);
			mShootAnim.LoadFrame(content, "gun-fire7", 0.07f);
			mShootAnim.LoadFrame(content, "gun-fire8", 0.07f);
			mShootAnim.LoadFrame(content, "gun-fire9", 0.07f);
			mShootAnim.LoadFrame(content, "gun-fire10", 0.07f);
			mShootAnim.LoadFrame(content, "gun", 0.05f);

		}

		public override void OnActivate()
		{
			mNewHighScore = false;
			ScoreManager.I.ResetScore();
			mScoreTimer.FullReset();

			SoundManager.I.PlayMusic(SoundManager.MusicType.MainGame, 0.5f);
			mDiceQueue = new DiceQueue();
			EntityManager.I.ClearEntities();

			mGunReloadTime = NORMAL_RELOAD_TIME;
			mGunLane = 0;
			mGameOverPoints = null;

			mGunReloadTimer.FullReset();
			mGameOverFadeTimer.FullReset();
			mReadyTimer.FullReset();
			mReadyTimer.Start();

			mTrackManager.Init();
		}

		public override void OnDeactivate()
		{
			EntityManager.I.ClearEntities();
		}

		private bool IsGameOver()
		{
			return mGameOverPoints != null && mGameOverPoints.Count > 0;
		}

		public void FastReload()
		{
			if (mGunReloadTimer.IsPlaying())
			{
				mGunReloadTimer.SetTimeSpeed(9.0);
			}
		}

		public override void Update(GameTime gameTime)
		{
			//Ready...
			if (mReadyTimer.IsPlaying())
			{
				if (mReadyTimer.GetElapsedMs() > mReadyTime)
				{
					mReadyTimer.Stop();
				}

				return;
			}

			//Check for game over
			mGameOverPoints = mTrackManager.GetGameOverPoints();

			if (IsGameOver())
			{
				if (mGameOverFadeTimer.IsPlaying() == false)
				{
					SoundManager.I.StopMusic();
					SoundManager.I.PlaySFX(SoundManager.SFXType.GameOver, 0.5f);
					mNewHighScore = ScoreManager.I.CheckHighScore();
					mGameOverFadeTimer.Start();
					mGunReloadTimer.Stop();
				}

				if (GetGameOverPercent() == 1.0f)
				{
					if (InputManager.I.LClick())
					{
						ScreenManager.I.ActivateScreen(ScreenType.RossButtonsScreen);
					}
				}
				return;
			}

			//Normal update
			//Scrolling images
			mTopSky.Update(gameTime);
			mTopSkyCloud1.Update(gameTime);
			mTopSkyCloud2.Update(gameTime);
			mGround.Update(gameTime);

			//Score
			if (mScoreTimer.IsPlaying() == false)
			{
				mScoreTimer.Start();
			}

			if (mScoreTimer.GetElapsedMs() > 1000.0)
			{
				ScoreManager.I.AddCurrentScore(5);
				mScoreTimer.FullReset();
			}

			mShootAnim.Update(gameTime);

			//Reload timer stuff
			if (mGunReloadTimer.GetElapsedMs() >= mGunReloadTime)
			{
				mGunReloadTimer.Stop();
				mGunReloadTimer.FullReset();
				SoundManager.I.PlaySFX(SoundManager.SFXType.GunReload, 0.35f);
				mGunReloadTime = NORMAL_RELOAD_TIME;
			}

			mTrackManager.Update(gameTime);
			EntityManager.I.Update(gameTime);

			HandleInput(gameTime);

			DecideGunPosition();
		}

		private void HandleInput(GameTime gameTime)
		{
			if (InputManager.I.LClick() && GetReloadPercent() == 1.0f)
			{
				FireGun(gameTime);
			}
		}

		private void FireGun(GameTime gameTime)
		{
			SoundManager.I.PlaySFX(SoundManager.SFXType.GunShoot, 0.2f);
			//Shoot dice
			Dice diceToShoot = mDiceQueue.PopDice();
			Texture2D diceTex = GetDiceTexture(diceToShoot);
			Vector2 speed = new Vector2(20.0f, 0.0f);
			Vector2 startPos = new Vector2(65.0f, 180.0f);
			startPos.Y = mGunLane * 50.0f + 43.0f;
			EntityManager.I.RegisterEntity(new MovingDie(startPos, speed, diceToShoot, diceTex), mContentManager);

			//Timer stuff
			mGunReloadTimer.FullReset();
			mGunReloadTimer.Start();
			mShootAnim.Play();
		}

		private float GetReloadPercent()
		{
			if (mGunReloadTimer.IsPlaying())
			{
				return (float)(mGunReloadTimer.GetElapsedMs() / mGunReloadTime);
			}

			return 1.0f;
		}

		private float GetGameOverPercent()
		{
			if (mGameOverFadeTimer.IsPlaying())
			{
				return Math.Min((float)(mGameOverFadeTimer.GetElapsedMs() / mGameOverFadeTime), 1.0f);
			}

			return 1.0f;
		}

		public void DecideGunPosition()
		{
			Point mousePos = InputManager.I.GetMousePos();

			mGunLane = Math.Max(0, (mousePos.Y - 29) / 50);

			mGunLane = Math.Min(mGunLane, NUM_LANES - 1);
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica Micro-24");

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(Color.SaddleBrown);

			info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);

			mGround.Draw(info);

			info.spriteBatch.Draw(mBackground, Vector2.Zero, Color.White);

			//Draw score
			mTopSky.Draw(info);
			//mTopSkyCloud1.Draw(info);
			mTopSkyCloud2.Draw(info);
			string scoreStr = "Score: " + ScoreManager.I.GetCurrentScore() + "    High score:" + ScoreManager.I.GetHighScore();
			Util.DrawStringCentred(info.spriteBatch, pixelFont, new Vector2(SCREEN_WIDTH / 2.0f + 1.0f, 15.0f + 1.0f), new Color(50, 25, 0), scoreStr);
			Util.DrawStringCentred(info.spriteBatch, pixelFont, new Vector2(SCREEN_WIDTH / 2.0f, 15.0f), Color.SaddleBrown, scoreStr);


			mTrackManager.Draw(info);
			EntityManager.I.Draw(info);

			DrawDice(info);

			DrawGun(info);

			if (IsGameOver())
			{
				DrawGameOver(info);
			}

			if (mReadyTimer.IsPlaying())
			{
				DrawGameStart(info);
			}

			info.spriteBatch.End();

			return mScreenTarget;
		}

		private void DrawGameStart(DrawInfo info)
		{
			Rect2f screenBG = new Rect2f(Vector2.Zero, new Vector2(Screen.SCREEN_WIDTH, Screen.SCREEN_HEIGHT));
			Util.DrawRect(info, screenBG, new Color(0, 0, 0, 128));
			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			Util.DrawStringCentred(info.spriteBatch, FontManager.I.GetFont("Pixica-24"), centre, Color.Wheat, "Get ready...");
		}

		private void DrawGameOver(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");
			SpriteFont smallPixelFont = FontManager.I.GetFont("Pixica Micro-24");

			foreach (Vector2 pos in mGameOverPoints)
			{
				info.spriteBatch.Draw(mGameOverCross, pos, Color.White);
			}

			int alpha = (int)(255.0f * GetGameOverPercent());

			Rect2f screenBG = new Rect2f(Vector2.Zero, new Vector2(Screen.SCREEN_WIDTH, Screen.SCREEN_HEIGHT));

			Util.DrawRect(info, screenBG, new Color(0, 0, 0, alpha));

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			float falpha = GetGameOverPercent();
			Color textColor = new Color(Color.Wheat, falpha);

			Util.DrawStringCentred(info.spriteBatch, pixelFont, centre + new Vector2(0.0f, -130.0f), textColor, "GAME OVER!");

			Util.DrawStringCentred(info.spriteBatch, pixelFont, centre, textColor, "Score: " + ScoreManager.I.GetCurrentScore());

			if (falpha == 1.0f)
			{
				Util.DrawStringCentred(info.spriteBatch, smallPixelFont, centre + new Vector2(0.0f, 130.0f), textColor, "Click to continue...");
			}

			if (mNewHighScore)
			{
				textColor = new Color(Color.Salmon, falpha);
				Util.DrawStringCentred(info.spriteBatch, pixelFont, centre + new Vector2(0.0f, 30.0f), textColor, "New high score: " + ScoreManager.I.GetHighScore() + "!");
			}
			else
			{
				Util.DrawStringCentred(info.spriteBatch, pixelFont, centre + new Vector2(0.0f, 30.0f), textColor, "High score: " + ScoreManager.I.GetHighScore());
			}
		}

		private void DrawGun(DrawInfo info)
		{
			Vector2 startPoint = new Vector2(0.0f, 29.0f);
			Vector2 spacing = new Vector2(0.0f, 50.0f);

			startPoint += spacing * mGunLane;

			Texture2D gunTex = mGunTexture;

			if (GetReloadPercent() != 1.0f)
			{
				gunTex = mShootAnim.GetCurrentTexture();
			}

			info.spriteBatch.Draw(gunTex, startPoint, Color.White);
		}

		private void DrawDice(DrawInfo info)
		{
			Vector2 reloadPoint = new Vector2(9.0f, 287.0f);
			Vector2 startPoint = new Vector2(110.5f, 287.0f);

			float p = 1.0f - GetReloadPercent();

			//Speical dice
			reloadPoint = Util.LerpVec(startPoint, reloadPoint, 1.0f - p);
			Texture2D texture = GetSideDiceTexture(mDiceQueue.PeekDice(0));
			info.spriteBatch.Draw(texture, reloadPoint, Color.White);



			Vector2 spacing = new Vector2(73.0f, 0.0f);

			startPoint += spacing * (p);

			for (int i = 1; i < mDiceQueue.GetDiceNum() - 1; i++)
			{
				texture = GetSideDiceTexture(mDiceQueue.PeekDice(i));

				info.spriteBatch.Draw(texture, startPoint, Color.White);

				startPoint += spacing;
			}

			startPoint += spacing * (p);

			texture = GetSideDiceTexture(mDiceQueue.PeekDice(mDiceQueue.GetDiceNum() - 1));

			info.spriteBatch.Draw(texture, startPoint, Color.White);

			info.spriteBatch.Draw(mGunBarrelTexture, new Vector2(0.0f, 276.0f), Color.White);

			if (GetReloadPercent() == 1.0f)
			{
				startPoint = new Vector2(7.0f, 289.0f);
				texture = GetDiceTexture(mDiceQueue.PeekDice(0));
				info.spriteBatch.Draw(texture, startPoint, Color.White);
			}
		}

		Texture2D GetDiceTexture(Dice die)
		{
			return GetDiceTexture(die.Value);
		}

		Texture2D GetDiceTexture(int value)
		{
			return mDiceTextures[value - 1];
		}

		Texture2D GetSideDiceTexture(Dice die)
		{
			return GetSideDiceTexture(die.Value);
		}

		Texture2D GetSideDiceTexture(int value)
		{
			return mSideDiceTextures[value - 1];
		}

	}
}
