namespace GMTK2023
{
	internal class ShakeAndRotateTo : CameraShake
	{
		const float DEFAULT_SHAKE_SPEED = 30.2f;
		const float DEFAULT_SHAKE_AMPLITUDE = 2.0f;

		float mStartRotation;
		float mTargetAngle;

		public ShakeAndRotateTo(float time, float targetAngle, float amplitude = DEFAULT_SHAKE_AMPLITUDE, float speed = DEFAULT_SHAKE_SPEED)
			: base(time, amplitude, speed)
		{
			mTargetAngle = targetAngle;
		}

		public ShakeAndRotateTo(float time, float targetAngle, Vector2 amplitude, float speed = DEFAULT_SHAKE_SPEED) : base(time, amplitude, speed)
		{
			mTargetAngle = targetAngle;
		}

		protected override void StartMovementInternal()
		{
			mStartRotation = mCurrentSpec.mRotation;

			if (MathF.Abs(mTargetAngle - mStartRotation) > MathF.PI)
			{
				if (mTargetAngle > mStartRotation)
				{
					mTargetAngle -= MathF.PI * 2.0f;
				}
				else
				{
					mTargetAngle += MathF.PI * 2.0f;
				}
			}

			base.StartMovementInternal();
		}

		protected override void EndMovementInternal(ref CameraSpec endSpec)
		{
			// Make sure we end exactly on the right angle
			endSpec.mRotation = mTargetAngle;
			base.EndMovementInternal(ref endSpec);
		}

		protected override bool IsMovementOverInternal()
		{
			return mTargetAngle == mStartRotation || base.IsMovementOverInternal();
		}

		protected override void UpdateInternal(GameTime gameTime)
		{
			float p = GetMovementPercentage();

			mCurrentSpec.mRotation = mTargetAngle * p + mStartRotation * (1.0f - p);

			base.UpdateInternal(gameTime);
		}

		public override bool MovementBlocksUpdate() { return true; }
	}
}
