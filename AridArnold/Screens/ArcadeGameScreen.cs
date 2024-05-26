
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
		ArcadeCabinet[] mCabinets;
		int mActiveCabinet;

		public ArcadeGameScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			MediaPlayer.IsMuted = true;
			mCabinets = new ArcadeCabinet[MonoAlg.EnumLength(typeof(ArcadeGameType))];
			mCabinets[(int)ArcadeGameType.DeathRide]    = new DeathRideCabinet(graphics, Main.GetMainContentManager());
			mCabinets[(int)ArcadeGameType.HorsesAndGun] = new HorsesAndGunCabinet(graphics, Main.GetMainContentManager());
			mCabinets[(int)ArcadeGameType.WormWarp] = new WormWarpCabinet(graphics, Main.GetMainContentManager());
			mActiveCabinet = -1;
			MediaPlayer.Stop(); // Some of these games play music on launch
			MediaPlayer.IsMuted = false;
		}


		public void ActivateGame(ArcadeGameType gameType)
		{
			ArcadeCabinet cab = mCabinets[(int)gameType];
			mActiveCabinet = (int)gameType;
			cab.ResetCabinet();
		}

		public override void Update(GameTime gameTime)
		{
			if (mActiveCabinet < 0)
			{
				throw new Exception("No cabinet selected");
			}

			mCabinets[mActiveCabinet].Update(gameTime);
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			if (mActiveCabinet < 0)
			{
				throw new Exception("No cabinet selected");
			}

			mCabinets[mActiveCabinet].DrawGameToTexture(info);

			//Draw out the game area
			StartScreenSpriteBatch(info);

			mCabinets[mActiveCabinet].Draw(info);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		
	}
}
