namespace GMTK2023
{
	internal class CameraShake : TimedCameraMove
	{
		Vector2 mShakeDisplacement;
		Vector2 mCentrePos;
		Vector2 mAmplitude;
		float mAngularSpeed;
		float mCurrAngle;

		public CameraShake(float time, float amplitude, float speed) : base(time)
		{
			mShakeDisplacement = Vector2.Zero;
			mAmplitude = new Vector2(amplitude);
			mAngularSpeed = speed;
			mCurrAngle = 0.0f;
		}

		public CameraShake(float time, Vector2 amplitude, float speed) : base(time)
		{
			mShakeDisplacement = Vector2.Zero;
			mAmplitude = amplitude;
			mAngularSpeed = speed;
		}



		protected override void StartMovementInternal()
		{
			mCentrePos = mCurrentSpec.mPosition;
		}

		protected override void EndMovementInternal(ref CameraSpec endSpec)
		{
			// Make sure we end exactly where we started
			endSpec.mPosition = mCentrePos;
		}

		protected override void UpdateInternal(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mCurrAngle += dt * mAngularSpeed;

			mShakeDisplacement.X = MathF.Sin(mCurrAngle) * mAmplitude.X;
			mShakeDisplacement.Y = MathF.Cos(mCurrAngle) * mAmplitude.Y;
			mShakeDisplacement *= GetDropOff();

			mCurrentSpec.mPosition = mCentrePos + mShakeDisplacement;

			base.UpdateInternal(gameTime);
		}

		protected virtual float GetDropOff()
		{
			return 1.0f;
		}
	}

	internal class DiminishCameraShake : CameraShake
	{
		public DiminishCameraShake(float time, float amplitude, float speed) : base(time, amplitude, speed)
		{
		}

		public DiminishCameraShake(float time, Vector2 amplitude, float speed) : base(time, amplitude, speed)
		{
		}

		protected override float GetDropOff()
		{
			return (1.0f - GetMovementPercentage());
		}
	}
}
