namespace AridArnold
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

		public override void StartMovement(CameraSpec startSpec)
		{
			mStartRotation = startSpec.mRotation;

			while (MathF.Abs(mTargetAngle - mStartRotation) > MathF.Tau * 0.5f)
			{
				if (mTargetAngle > mStartRotation)
				{
					mTargetAngle -= MathF.Tau;
				}
				else
				{
					mTargetAngle += MathF.Tau;
				}
			}

			base.StartMovement(startSpec);
		}

		public override CameraSpec EndMovementSpec()
		{
			// Make sure we end exactly on the right angle
			mCurrentSpec.mRotation = mTargetAngle % MathF.Tau;
			
			return base.EndMovementSpec();
		}

		public override bool IsMovementOver()
		{
			return mTargetAngle == mStartRotation || base.IsMovementOver();
		}

		public override void Update(GameTime gameTime)
		{
			float p = GetMovementPercentage();

			mCurrentSpec.mRotation = mTargetAngle * p + mStartRotation * (1.0f - p);

			base.Update(gameTime);
		}

		public override bool MovementBlocksUpdate() { return true; }
	}
}
