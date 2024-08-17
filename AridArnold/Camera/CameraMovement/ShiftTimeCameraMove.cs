namespace AridArnold
{
	internal class ShiftTimeCameraMove : TimedCameraMove
	{
		public const float TIME_TO_ROTATE = 40.0f;
		const float ZOOM_OUT_LEVEL = 8.0f;

		bool mForwards;
		int mUnblockUpdateHack = 0;
		TimeShiftFaderFX mTextureFader;

		public ShiftTimeCameraMove(bool forwards, TimeShiftFaderFX textureFader) : base(TIME_TO_ROTATE)
		{
			mForwards = forwards;
			mTextureFader = textureFader;

			if(mForwards)
			{
				SFXManager.I.PlaySFX(AridArnoldSFX.AgeForward, 0.2f);
			}
			else
			{
				SFXManager.I.PlaySFX(AridArnoldSFX.AgeBackward, 0.2f);
			}
		}

		public override void Update(GameTime gameTime)
		{
			float p = GetMovementPercentage();
			float waveP = MonoMath.SmoothZeroToOne(Math.Min(2.0f - MathF.Abs(4.0f * p - 2.0f), 1.0f));
			mCurrentSpec.mRotation = mStartSpec.mRotation + MathF.PI * 2.0f * MonoMath.SmoothZeroToOne(p);

			if (!mForwards)
			{
				mCurrentSpec.mRotation = -mCurrentSpec.mRotation;
			}

			mCurrentSpec.mZoom = 1.0f + ((1.0f / ZOOM_OUT_LEVEL) - 1.0f) * waveP;
			mCurrentSpec.mPosition = 20.0f * waveP * new Vector2(RandomManager.I.GetDraw().GetUnitFloat(), RandomManager.I.GetDraw().GetUnitFloat());

			mUnblockUpdateHack++;

			mTextureFader.Update(gameTime);

			base.Update(gameTime);
		}

		public override CameraSpec EndMovementSpec()
		{
			mCurrentSpec.mPosition = Vector2.Zero;
			mCurrentSpec.mRotation = mStartSpec.mRotation;
			mCurrentSpec.mZoom = 1.0f;

			return mCurrentSpec;
		}

		public override bool MovementBlocksUpdate()
		{
			return mUnblockUpdateHack > 1;
		}
	}
}
