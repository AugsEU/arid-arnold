﻿namespace AridArnold
{
	internal class ShiftTimeCameraMove : TimedCameraMove
	{
		const float TIME_TO_ROTATE = 15.0f;
		const float ZOOM_OUT_LEVEL = 80.0f;

		float mStartRotation;
		bool mForwards;

		public ShiftTimeCameraMove(bool forwards) : base(TIME_TO_ROTATE)
		{
			mStartRotation = 0.0f;
			mForwards = forwards;
		}


		protected override void StartMovementInternal()
		{
			mStartRotation = mCurrentSpec.mRotation;
			
		}

		protected override void UpdateInternal(GameTime gameTime)
		{
			float p = GetMovementPercentage();
			float waveP = MonoMath.UnitWave(p);
			mCurrentSpec.mRotation = mStartRotation + MathF.PI * 2.0f * p;
			mCurrentSpec.mZoom = 1.0f + ((1.0f / ZOOM_OUT_LEVEL) - 1.0f) * waveP;
			mCurrentSpec.mPosition = 40.0f * waveP * new Vector2(RandomManager.I.GetDraw().GetUnitFloat(), RandomManager.I.GetDraw().GetUnitFloat());

			base.UpdateInternal(gameTime);
		}

		protected override void EndMovementInternal(ref CameraSpec endSpec)
		{
			endSpec.mPosition = Vector2.Zero;
			endSpec.mRotation = mStartRotation;
			endSpec.mZoom = 1.0f;
		}

		public override bool MovementBlocksUpdate()
		{
			return true;
		}
	}
}