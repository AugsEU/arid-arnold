using System;
using System.Collections.Generic;
using System.Threading;

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
		Texture2D mDestinationCursor;
		Texture2D mSilverBulletIcon;
		Animator mShootAnim;
		MonoTimer mGunReloadTimer;
		double mGunReloadTime;
		int mGunFrenzy = 0;
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

		//Game over
		List<Vector2> mGameOverPoints;
		MonoTimer mGameOverFadeTimer;
		const double mGameOverFadeTime = 2400.0;
		bool mRequestedExit = false;



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
			mBackground = content.Load<Texture2D>("Arcade/HorsesAndGun/main_bg");
			mGunTexture = content.Load<Texture2D>("Arcade/HorsesAndGun/gun");
			mGunBarrelTexture = content.Load<Texture2D>("Arcade/HorsesAndGun/gun_barrel");
			mGameOverCross = content.Load<Texture2D>("Arcade/HorsesAndGun/dead_x");
			mDestinationCursor = content.Load<Texture2D>("Arcade/HorsesAndGun/cursor");
			mSilverBulletIcon = content.Load<Texture2D>("Arcade/HorsesAndGun/SilverBullet");

			mTopSky = new ScrollingImage(content.Load<Texture2D>("Arcade/HorsesAndGun/sky_1"), content.Load<Texture2D>("Arcade/HorsesAndGun/sky_2"), Vector2.Zero, 70);
			mTopSkyCloud1 = new ScrollingImage(content.Load<Texture2D>("Arcade/HorsesAndGun/dust_cloud_1"), content.Load<Texture2D>("Arcade/HorsesAndGun/dust_cloud_1"), Vector2.Zero, 130);
			mTopSkyCloud2 = new ScrollingImage(content.Load<Texture2D>("Arcade/HorsesAndGun/dust_cloud_2"), content.Load<Texture2D>("Arcade/HorsesAndGun/dust_cloud_2"), Vector2.Zero, 130);
			mGround = new ScrollingImage(content.Load<Texture2D>("Arcade/HorsesAndGun/ground"), content.Load<Texture2D>("Arcade/HorsesAndGun/ground"), new Vector2(84.0f, 27.0f), 200);

			mDiceTextures[0] = content.Load<Texture2D>("Arcade/HorsesAndGun/Dice1");
			mDiceTextures[1] = content.Load<Texture2D>("Arcade/HorsesAndGun/Dice2");
			mDiceTextures[2] = content.Load<Texture2D>("Arcade/HorsesAndGun/Dice3");
			mDiceTextures[3] = content.Load<Texture2D>("Arcade/HorsesAndGun/Dice4");
			mDiceTextures[4] = content.Load<Texture2D>("Arcade/HorsesAndGun/Dice5");
			mDiceTextures[5] = content.Load<Texture2D>("Arcade/HorsesAndGun/Dice6");


			mSideDiceTextures[0] = content.Load<Texture2D>("Arcade/HorsesAndGun/dice_side_1");
			mSideDiceTextures[1] = content.Load<Texture2D>("Arcade/HorsesAndGun/dice_side_2");
			mSideDiceTextures[2] = content.Load<Texture2D>("Arcade/HorsesAndGun/dice_side_3");
			mSideDiceTextures[3] = content.Load<Texture2D>("Arcade/HorsesAndGun/dice_side_4");
			mSideDiceTextures[4] = content.Load<Texture2D>("Arcade/HorsesAndGun/dice_side_5");
			mSideDiceTextures[5] = content.Load<Texture2D>("Arcade/HorsesAndGun/dice_side_6");

			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire1", 0.05f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire2", 0.05f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire3", 0.05f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire4", 0.07f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire5", 0.07f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire6", 0.07f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire7", 0.07f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire8", 0.07f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire9", 0.07f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun-fire10", 0.07f);
			mShootAnim.LoadFrame(content, "Arcade/HorsesAndGun/gun", 0.05f);

		}

		public override void OnActivate()
		{
			ScoreManager.I.ResetScore();
			mScoreTimer.FullReset();

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
			mRequestedExit = false;
			mGunFrenzy = 0;
		}

		public override void OnDeactivate()
		{
			EntityManager.I.ClearEntities();
		}

		private bool IsGameOver()
		{
			return mGameOverPoints != null && mGameOverPoints.Count > 0;
		}

		public bool IsExitRequested()
		{
			return mRequestedExit;
		}

		public void FastReload()
		{
			mGunFrenzy = Math.Max(mGunFrenzy, 0);
			mGunFrenzy += 3;
			mGunFrenzy = Math.Min(mGunFrenzy, 6); // Only 6 shots in a revolver
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
					AridArnold.SFXManager.I.PlaySFX(AridArnold.AridArnoldSFX.ArcadeGameOver, 0.7f);
					AridArnold.MusicManager.I.StopMusic();
					mGameOverFadeTimer.Start();
					mGunReloadTimer.Stop();
				}

				if (GetGameOverPercent() == 1.0f)
				{
					if (AridArnold.InputManager.I.KeyPressed(AridArnold.InputAction.Confirm))
					{
						mRequestedExit = true;
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
				AridArnold.SFXManager.I.PlaySFX(AridArnold.AridArnoldSFX.HorseReload, 0.35f);
				mGunReloadTime = NORMAL_RELOAD_TIME;
			}

			mTrackManager.Update(gameTime);
			EntityManager.I.Update(gameTime);

			HandleInput(gameTime);
		}

		private void HandleInput(GameTime gameTime)
		{
			bool fireGun = AridArnold.InputManager.I.KeyHeld(AridArnold.InputAction.UseItem);
			bool up = AridArnold.InputManager.I.KeyPressed(AridArnold.InputAction.ArnoldUp);
			bool down = AridArnold.InputManager.I.KeyPressed(AridArnold.InputAction.ArnoldDown);

			if (fireGun && GetReloadPercent() == 1.0f)
			{
				FireGun(gameTime);
			}

			if(up && !down)
			{
				mGunLane--;
			}
			else if(down && !up)
			{
				mGunLane++;
			}

			mGunLane = Math.Clamp(mGunLane, 0, NUM_LANES-1);
		}

		private void FireGun(GameTime gameTime)
		{
			AridArnold.SFXManager.I.PlaySFX(AridArnold.AridArnoldSFX.HorseGunShoot, 0.3f);
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
			if(mGunFrenzy > 0)
			{
				mGunReloadTimer.SetTimeSpeed(9.0);
			}
			mGunFrenzy--;
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
			string scoreStr = "Score: " + ScoreManager.I.GetCurrentScore();
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
		}

		private void DrawGun(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica Micro-24");
			Vector2 startPoint = new Vector2(0.0f, 29.0f);
			Vector2 spacing = new Vector2(0.0f, 50.0f);

			startPoint += spacing * mGunLane;

			Texture2D gunTex = mGunTexture;

			if (GetReloadPercent() != 1.0f)
			{
				gunTex = mShootAnim.GetCurrentTexture();
			}

			Vector2 silverBulletBasePos = startPoint + new Vector2(33.0f, 3.0f);

			for(int i = 0; i < mGunFrenzy; i++)
			{
				info.spriteBatch.Draw(mSilverBulletIcon, silverBulletBasePos, Color.White);

				silverBulletBasePos.X += 8.0f;
			}

			info.spriteBatch.Draw(gunTex, startPoint, Color.White);

			DrawGunCursor(info);
		}

		void DrawGunCursor(DrawInfo info)
		{
			if(GetReloadPercent() != 1.0f)
			{
				return;
			}

			int currDiceVal = mDiceQueue.PeekDice(0).Value;
			HorseOrder mimicOrder = new HorseOrder(HorseOrderType.moveTile, currDiceVal);

			Horse horseWeWillHit = mTrackManager.GetProjectedHorseHit(mGunLane);

			if(horseWeWillHit is null || horseWeWillHit.HasOrder())
			{
				return;
			}

			Point dest = mTrackManager.MakeHorseOrderValid(horseWeWillHit, ref mimicOrder);

			Vector2 drawPos = mTrackManager.GetTilePos(dest.Y, dest.X);
			drawPos.X += 16.0f;
			drawPos.Y += 15.0f;

			info.spriteBatch.Draw(mDestinationCursor, drawPos, Color.White * 0.5f);
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
