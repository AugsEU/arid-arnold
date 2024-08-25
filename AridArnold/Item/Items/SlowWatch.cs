
namespace AridArnold
{
	internal class SlowWatch : Item
	{
		const double SLOW_TIME = 3800.0;

		PercentageTimer mSlowTimer;

		public SlowWatch() : base("Items.SlowWatchTitle", "Items.SlowWatchDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/SlowWatch/Watch");
			mSlowTimer = new PercentageTimer(SLOW_TIME);
		}

		public override int GetPrice()
		{
			return 1;
		}

		public override void Begin()
		{
			mSlowTimer.ResetStart();
			SFXManager.I.PlaySFX(AridArnoldSFX.AgeForward, 0.4f, -0.8f, -0.8f);
			base.Begin();
		}

		public override void ActOnArnold(GameTime gameTime, Arnold arnold)
		{
			mSlowTimer.Update(gameTime);

			if(mSlowTimer.IsPlaying() && mSlowTimer.GetElapsedMsF() >= 0.1f)
			{
				Main.SetTimeSlowDown(2);
				if (mSlowTimer.GetPercentageF() >= 1.0f)
				{
					Main.SetTimeSlowDown(1);
					mSlowTimer.Stop();
					EndItem();
				}
			}

			base.ActOnArnold(gameTime, arnold);
		}
	}
}
