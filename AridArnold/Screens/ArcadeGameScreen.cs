
namespace AridArnold
{
	public enum ArcadeGameType : int
	{
		DeathRide = 0,
		HorsesAndGun,
		WormWarp
	}

	internal class ArcadeGameScreen : Screen
	{
		ArcadeGame[] mGames;
		ArcadeGame mActiveGame;

		public ArcadeGameScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mGames = new ArcadeGame[MonoAlg.EnumLength(typeof(ArcadeGameType))];
			mGames[(int)ArcadeGameType.DeathRide]    = new DeathRide.DeathRide(graphics, Main.GetMainContentManager());
			mGames[(int)ArcadeGameType.HorsesAndGun] = new HorsesAndGun.HorsesAndGun(graphics, Main.GetMainContentManager());
			mGames[(int)ArcadeGameType.WormWarp] = new WormWarp.SnakeGameArcade(graphics, Main.GetMainContentManager());
			mActiveGame = null;
		}


		public void ActivateGame(ArcadeGameType gameType)
		{
			ArcadeGame game = mGames[(int)gameType];
			mActiveGame = game;
			game.ResetGame();
		}

		public override void Update(GameTime gameTime)
		{
			if(mActiveGame != null)
			{
				mActiveGame.Update(gameTime);
			}
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			MonoDebug.Assert(mActiveGame != null);

			// Render the arcade game
			RenderTarget2D arcadeGameRender = null;
			if (mActiveGame != null)
			{
				arcadeGameRender = mActiveGame.DrawToRenderTarget(info);
			}

			//Draw out the game area
			StartScreenSpriteBatch(info);

			if (arcadeGameRender is not null)
			{
				MonoDraw.DrawTexture(info, arcadeGameRender, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, DrawLayer.Default);
			}

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		
	}
}
