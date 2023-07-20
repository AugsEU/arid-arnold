namespace AridArnold
{
	internal class ShiftTimeCameraMove : TimedCameraMove
	{
		const float TIME_TO_ROTATE = 35.0f;
		const float ZOOM_OUT_LEVEL = 8.0f;

		float mStartRotation;
		bool mForwards;
		bool mChangedTime;

		public ShiftTimeCameraMove(bool forwards) : base(TIME_TO_ROTATE)
		{
			mStartRotation = 0.0f;
			mForwards = forwards;
			mChangedTime = false;
		}


		protected override void StartMovementInternal()
		{
			mStartRotation = mCurrentSpec.mRotation;
		}

		protected override void UpdateInternal(GameTime gameTime)
		{
			float p = GetMovementPercentage();
			float waveP = MonoMath.SmoothZeroToOne( Math.Min(2.0f - MathF.Abs(4.0f*p-2.0f),1.0f) );
			mCurrentSpec.mRotation = mStartRotation + MathF.PI * 2.0f * MonoMath.SmoothZeroToOne(p);
			mCurrentSpec.mZoom = 1.0f + ((1.0f / ZOOM_OUT_LEVEL) - 1.0f) * waveP;
			mCurrentSpec.mPosition = 20.0f * waveP * new Vector2(RandomManager.I.GetDraw().GetUnitFloat(), RandomManager.I.GetDraw().GetUnitFloat());

			if(p > 0.5f && !mChangedTime)
			{
				ChangeTime();
				mChangedTime = true;
			}

			base.UpdateInternal(gameTime);
		}

		void ChangeTime()
		{
			if(mForwards)
			{
				TimeZoneManager.I.AgePlayer();
			}
			else
			{
				TimeZoneManager.I.AntiAgePlayer();
			}
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
