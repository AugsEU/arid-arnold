
using System;

namespace AridArnold
{
	class WorldArcadeCabinet : Entity
	{
		ArcadeGameType mGameType;
		MonoTimer mStaticTimer;
		TextInfoBubble mBubble;

		public WorldArcadeCabinet(Vector2 pos, ArcadeGameType type) : base(pos)
		{
			mGameType = type;
			mPosition.Y -= 4.0f;
			mStaticTimer = new MonoTimer();
			mStaticTimer.Start();
		}

		public override void LoadContent()
		{
			SpriteFont bubbleFont = FontManager.I.GetFont("Pixica-12");
			Vector2 bubblePos = mPosition;
			string strID = "Arcade.PlayDeathRide";
			switch (mGameType)
			{
				case ArcadeGameType.DeathRide:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinetDeathRide");
					strID = "Arcade.PlayDeathRide";
					break;
				case ArcadeGameType.HorsesAndGun:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinetHorse");
					strID = "Arcade.PlayHorse";
					break;
				case ArcadeGameType.WormWarp:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinetSnake");
					strID = "Arcade.PlaySnake";
					break;
				default:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinet");
					break;
			}

			mBubble = new TextInfoBubble(bubblePos, BubbleStyle.DefaultPrompt, bubbleFont, strID, Color.White);
		}

		public override void Update(GameTime gameTime)
		{
			mBubble.Update(gameTime);
			if(mPlayerNear)
			{
				mBubble.Open();
			}
			else
			{
				mBubble.Close();
			}
			base.Update(gameTime);
		}

		protected override void OnPlayerInteract()
		{
			if(InputManager.I.KeyPressed(AridArnoldKeys.Confirm))
			{
				ArcadeGameScreen arcadeGameScreen = ScreenManager.I.GetScreen<ArcadeGameScreen>();
				arcadeGameScreen.ActivateGame(mGameType);
				ScreenManager.I.ActivateScreen(ScreenType.ArcadeGame);
			}
		}

		public override void Draw(DrawInfo info)
		{
			mBubble.Draw(info);
			MonoDraw.DrawTexture(info, mTexture, mPosition);

			Color[] staticColors = { new Color(38, 38, 38), Color.DarkGray };

			int colorIdx = mStaticTimer.GetElapsedMs() % 150.0 > 75.0 ? 1 : 0;
			for(int y = 0; y < 6; y++)
			{
				MonoDraw.DrawRectDepth(info, new Rect2f(mPosition + new Vector2(2.0f, 4.0f + y), 8.0f, 1.0f), staticColors[colorIdx], DrawLayer.Default);
				colorIdx = (colorIdx + 1) % staticColors.Length;
			}
		}
	}
}
