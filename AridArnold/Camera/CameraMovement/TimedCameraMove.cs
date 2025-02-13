﻿using Microsoft.VisualBasic;

namespace AridArnold
{
	abstract class TimedCameraMove : CameraMovement
	{
		float mElapsedTime;
		float mTotalTime;

		GameSFX mTravelSound = null;
		GameSFX mFinishSound = null;

		public TimedCameraMove(float time)
		{
			mTotalTime = time;
		}

		public void LoadSFX(GameSFX traveSFX, GameSFX finishSFX)
		{
			mTravelSound = traveSFX;
			mFinishSound = finishSFX;

			if (mTravelSound is not null)
			{
				mTravelSound.GetBuffer().SetLoop(true);
				SFXManager.I.PlaySFX(mTravelSound);
			}
		}

		public override bool IsMovementOver()
		{
			return mElapsedTime >= mTotalTime;
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mElapsedTime += dt;


			if (IsMovementOver())
			{
				mTravelSound?.Stop(60.0f);

				if (mFinishSound is not null)
				{
					SFXManager.I.PlaySFX(mFinishSound);
				}

				mTravelSound = null;
				mFinishSound = null;
			}

			base.Update(gameTime);
		}

		protected float GetMovementPercentage()
		{
			return Math.Clamp(mElapsedTime / mTotalTime, 0.0f, 1.0f);
		}
	}
}
