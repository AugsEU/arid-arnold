namespace AridArnold
{
	abstract class TimedCameraMove : CameraMovement
	{
		float mElapsedTime;
		float mTotalTime;

		public TimedCameraMove(float time)
		{
			mTotalTime = time;
		}

		protected override bool IsMovementOverInternal()
		{
			return mElapsedTime >= mTotalTime;
		}

		protected override void UpdateInternal(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mElapsedTime += dt;
		}

		protected float GetMovementPercentage()
		{
			return Math.Clamp(mElapsedTime / mTotalTime, 0.0f, 1.0f);
		}
	}
}
