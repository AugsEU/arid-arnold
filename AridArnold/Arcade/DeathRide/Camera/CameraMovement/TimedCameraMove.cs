namespace GMTK2023
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
			return mElapsedTime / mTotalTime;
		}
	}
}
