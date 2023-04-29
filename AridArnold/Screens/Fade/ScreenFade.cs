namespace AridArnold
{
	abstract class ScreenFade : FX
	{
		float mTime;
		float mSpeed;
		bool mForwards;

		public ScreenFade(float speed, bool forwards)
		{
			mTime = forwards ? 0.0f : 1.0f;
			mSpeed = speed;
			mForwards = forwards;
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			mTime += (mForwards ? dt : -dt) * mSpeed;
		}


		public override bool Finished()
		{
			if (!mForwards) return mTime <= 0.0f;

			return mTime >= 1.0f;
		}

		public override void Draw(DrawInfo info)
		{
			DrawAtTime(info, mTime);
		}

		protected abstract void DrawAtTime(DrawInfo info, float time);
	}
}
