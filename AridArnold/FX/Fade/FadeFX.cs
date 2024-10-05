
namespace AridArnold
{
	/// <summary>
	/// An FX object that takes in a fade and automatically updates it.
	/// </summary>
	class FadeFX : FX
	{
		float mTime;
		float mSpeed;
		bool mForwards;
		Fade mFadeObj;

		public FadeFX(Fade fadeObj, float speed = 0.1f, bool forwards = false)
		{
			float speedMult = CampaignManager.I.IsSpeedrunMode() ? 3.0f : 1.0f;

			mTime = 0.0f;
			mSpeed = speed * speedMult;
			mForwards = forwards;
			mFadeObj = fadeObj;
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			mTime += dt * mSpeed;
			mTime = Math.Clamp(mTime, 0.0f, 1.0f);
		}

		public override void Draw(DrawInfo info)
		{
			float calcTime = mForwards ? mTime : 1.0f - mTime;
			mFadeObj.DrawAtTime(info, calcTime);
		}

		public override bool Finished()
		{
			return mTime >= 1.0f;
		}
	}
}
