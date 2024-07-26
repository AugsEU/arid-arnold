namespace AridArnold
{
	internal class ShiftTimeCameraMove : TimedCameraMove
	{
		public const float TIME_TO_ROTATE = 60.0f;
		const float ZOOM_OUT_LEVEL = 8.0f;

		float mStartRotation;
		bool mForwards;
		int mUnblockUpdateHack = 0;
		TimeShiftFaderFX mTextureFader;

		public ShiftTimeCameraMove(bool forwards, TimeShiftFaderFX textureFader) : base(TIME_TO_ROTATE)
		{
			mStartRotation = 0.0f;
			mForwards = forwards;
			mTextureFader = textureFader;
		}


		protected override void StartMovementInternal()
		{
			mStartRotation = mCurrentSpec.mRotation;
		}

		protected override void UpdateInternal(GameTime gameTime)
		{
			float p = GetMovementPercentage();
			float waveP = MonoMath.SmoothZeroToOne(Math.Min(2.0f - MathF.Abs(4.0f * p - 2.0f), 1.0f));
			mCurrentSpec.mRotation = mStartRotation + MathF.PI * 2.0f * MonoMath.SmoothZeroToOne(p);

			if (!mForwards)
			{
				mCurrentSpec.mRotation = -mCurrentSpec.mRotation;
			}

			mCurrentSpec.mZoom = 1.0f + ((1.0f / ZOOM_OUT_LEVEL) - 1.0f) * waveP;
			mCurrentSpec.mPosition = 20.0f * waveP * new Vector2(RandomManager.I.GetDraw().GetUnitFloat(), RandomManager.I.GetDraw().GetUnitFloat());

			mUnblockUpdateHack++;

			mTextureFader.Update(gameTime);

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
			return mUnblockUpdateHack > 1;
		}
	}
}
