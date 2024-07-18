namespace AridArnold
{
	internal class WaterDrop : FX
	{
		const float WATER_GRAVITY = 4.0f;
		const float DEW_SPEED = 1.0f;
		const float SPLASH_INTENSITY = 10.0f;

		const double DEW_TIME = 1200.0;
		const double SPLASH_TIME = 200.0;

		enum WaterState
		{
			Dew,
			FreeFall,
			Splash
		}

		Color mMainColor;
		Color mSecondColor;

		Vector2 mStartPoint;
		Vector2 mPos;
		Vector2 mPrevPos;
		Vector2 mVelocity;
		float mTotalDistanceTravelled;
		float mMaximumDistance;

		MonoTimer mTimer;
		WaterState mWaterState;

		public WaterDrop(Vector2 pos, float distance, Color mainColor, Color secondColor)
		{
			mWaterState = WaterState.Dew;
			mTimer = new MonoTimer();
			mTimer.Start();

			mStartPoint = pos;
			mPos = pos;
			mPrevPos = pos;
			mVelocity = Vector2.Zero;
			mMaximumDistance = distance;
			mTotalDistanceTravelled = 0.0f;

			mMainColor = mainColor;
			mSecondColor = secondColor;
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mPrevPos = mPos;

			switch (mWaterState)
			{
				case WaterState.Dew:
					mVelocity.Y = DEW_SPEED * dt;
					mPos += mVelocity * dt;
					break;
				case WaterState.FreeFall:
					mVelocity.Y += WATER_GRAVITY * dt;
					mPos += mVelocity * dt;
					break;
				case WaterState.Splash:
					mPos = mStartPoint;
					mPos.Y += mMaximumDistance;
					break;
			}

			mTotalDistanceTravelled += mPos.Y - mPrevPos.Y;

			DecideState();

			mTimer.Update(gameTime);
		}

		void DecideState()
		{
			switch (mWaterState)
			{
				case WaterState.Dew:
					if (mTimer.GetElapsedMs() > DEW_TIME)
					{
						mWaterState = WaterState.FreeFall;
						mTimer.Reset();
					}
					break;
				case WaterState.FreeFall:
					if (mTotalDistanceTravelled > mMaximumDistance)
					{
						mWaterState = WaterState.Splash;
						mTimer.Reset();
					}
					break;
			}
		}

		public override void Draw(DrawInfo info)
		{
			float dropSize = GetDropSize();

			Vector2 mainPos = mPos;
			Vector2 secondPos = mPrevPos;

			if (mWaterState == WaterState.Splash)
			{
				float move = (float)mTimer.GetElapsedMs() * SPLASH_INTENSITY / 1000.0f;
				mainPos.X += move;
				mainPos.Y -= move;
				secondPos.X -= move;
				secondPos.Y -= move;
			}

			mainPos.X -= dropSize / 2.0f;
			secondPos.X -= dropSize / 2.0f;

			Rectangle mainRect = new Rectangle((int)mainPos.X, (int)mainPos.Y, (int)dropSize, (int)dropSize);
			Rectangle secondRect = new Rectangle((int)secondPos.X, (int)secondPos.Y - 1, (int)dropSize, (int)dropSize);

			MonoDraw.DrawRectDepth(info, secondRect, mSecondColor, DrawLayer.BackgroundElement);
			MonoDraw.DrawRectDepth(info, mainRect, mMainColor, DrawLayer.BackgroundElement);
		}

		float GetDropSize()
		{
			switch (mWaterState)
			{
				case WaterState.Dew:
					return 2.0f * (float)mTimer.GetElapsedMs() / (float)DEW_TIME;
				case WaterState.FreeFall:
					return 2.0f;
				case WaterState.Splash:
					return MathF.Ceiling(2.0f * (1.0f - (float)mTimer.GetElapsedMs() / (float)SPLASH_TIME));
			}

			return 2.0f;
		}

		public override bool Finished()
		{
			return mWaterState == WaterState.Splash && mTimer.GetElapsedMs() > SPLASH_TIME;
		}


	}
}
