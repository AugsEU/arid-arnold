﻿
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
			mCabinets = new ArcadeCabinet[MonoEnum.EnumLength(typeof(ArcadeGameType))];
			mCabinets[(int)ArcadeGameType.DeathRide]    = new DeathRideCabinet(graphics, Main.GetMainContentManager());
			mCabinets[(int)ArcadeGameType.HorsesAndGun] = new HorsesAndGunCabinet(graphics, Main.GetMainContentManager());
			mCabinets[(int)ArcadeGameType.WormWarp] = new WormWarpCabinet(graphics, Main.GetMainContentManager());
			mActiveCabinet = -1;

			for (int i = 0; i < mCabinets.Length; i++)
			{
				mCabinets[i].ResetScores();
			}
		}


		public void ActivateGame(ArcadeGameType gameType)
		{
			ArcadeCabinet cab = mCabinets[(int)gameType];
			mActiveCabinet = (int)gameType;
			cab.ResetCabinet();
		}

		public override void OnActivate()
		{
			MusicManager.I.StopMusic(1100.0);
			SFXManager.I.EndAllSFX(40.0f);
			base.OnActivate();
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

		public void ResetScores()
		{
			for(int i = 0; i < mCabinets.Length; i++)
			{
				mCabinets[i].ResetScores();
			}
		}

		public void ReadBinary(BinaryReader br)
		{
			int numCabs = br.ReadInt32();
			for(int i = 0; i < numCabs && i < mCabinets.Length; i++)
			{
				mCabinets[i].ReadBinary(br);
			}
		}

		public void WriteBinary(BinaryWriter bw)
		{
			bw.Write(mCabinets.Length);
			for (int i = 0; i < mCabinets.Length; i++)
			{
				mCabinets[i].WriteBinary(bw);
			}
		}
	}
}
