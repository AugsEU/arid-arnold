
using System;

namespace AridArnold
{
	class WorldArcadeCabinet : Entity
	{
		ArcadeGameType mGameType;
		MonoTimer mStaticTimer;
		TextInfoBubble mPlayBubble;
		TextInfoBubble mGetTokenBubble;
		KeyItemFlagType mRequiredItem;

		public WorldArcadeCabinet(Vector2 pos, ArcadeGameType type) : base(pos)
		{
			mGameType = type;
			mPosition.Y -= 4.0f;
			mPosition.X += 2.0f;
			mStaticTimer = new MonoTimer();
			mStaticTimer.Start();

			mRequiredItem = KeyItemFlagType.kMaxKeyItems;
		}

		public override void LoadContent()
		{
			Vector2 bubblePos = mPosition + new Vector2(6.0f, -2.0f);
			string playStrID = "Arcade.PlayDeathRide";
			string getStrID = "Arcade.GetDemonToken";
			switch (mGameType)
			{
				case ArcadeGameType.DeathRide:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinetDeathRide");

					playStrID = "Arcade.PlayDeathRide";
					getStrID = "Arcade.GetDemonToken";

					mRequiredItem = KeyItemFlagType.kDemonToken;
					break;
				case ArcadeGameType.HorsesAndGun:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinetHorse");

					playStrID = "Arcade.PlayHorse";
					getStrID = "Arcade.GetHorseToken";

					mRequiredItem = KeyItemFlagType.kHorseToken;
					break;
				case ArcadeGameType.WormWarp:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinetSnake");
					
					playStrID = "Arcade.PlaySnake";
					getStrID = "Arcade.GetSerpentToken";

					mRequiredItem = KeyItemFlagType.kSerpentToken;
					break;
				default:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/WorldCabinet");
					break;
			}

			mPlayBubble = new TextInfoBubble(bubblePos, playStrID);
			mGetTokenBubble = new TextInfoBubble(bubblePos, getStrID);
		}

		public override void Update(GameTime gameTime)
		{
			mStaticTimer.Update(gameTime);

			bool hasReqItem = FlagsManager.I.CheckFlag(FlagCategory.kKeyItems, (UInt32)mRequiredItem);
			mPlayBubble.Update(gameTime, mPlayerNear && hasReqItem);
			mGetTokenBubble.Update(gameTime, mPlayerNear && !hasReqItem);
			base.Update(gameTime);
		}

		protected override void OnPlayerInteract()
		{
			bool hasReqItem = FlagsManager.I.CheckFlag(FlagCategory.kKeyItems, (UInt32)mRequiredItem);
			if (!hasReqItem)
			{
				return;
			}
			if(InputManager.I.KeyPressed(AridArnoldKeys.Confirm))
			{
				ArcadeGameScreen arcadeGameScreen = ScreenManager.I.GetScreen<ArcadeGameScreen>();
				arcadeGameScreen.ActivateGame(mGameType);
				ScreenManager.I.ActivateScreen(ScreenType.ArcadeGame);
			}
		}

		public override void Draw(DrawInfo info)
		{
			mPlayBubble.Draw(info);
			mGetTokenBubble.Draw(info);
			MonoDraw.DrawTexture(info, mTexture, mPosition);

			Color[] staticColors = { new Color(55, 55, 55), Color.DarkGray };

			int colorIdx = mStaticTimer.GetElapsedMs() % 150.0 > 75.0 ? 1 : 0;
			for(int y = 0; y < 6; y++)
			{
				MonoDraw.DrawRectDepth(info, new Rect2f(mPosition + new Vector2(2.0f, 4.0f + y), 8.0f, 1.0f), staticColors[colorIdx], DrawLayer.Default);
				colorIdx = (colorIdx + 1) % staticColors.Length;
			}
		}
	}
}
