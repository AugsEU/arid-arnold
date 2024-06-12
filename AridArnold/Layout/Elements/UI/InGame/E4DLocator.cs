namespace AridArnold
{
	class E4DLocator : UIPanelBase
	{
		PercentageTimer mFlashTimer;
		PercentageTimer mTravelTimer;

		int mPrevTimezone;
		int mPrevAge;

		public E4DLocator(XmlNode rootNode) : base(rootNode, "UI/InGame/4DLocatorBG")
		{
			mFlashTimer = new PercentageTimer(200.0);
			mTravelTimer = new PercentageTimer(1500.0);

			mPrevTimezone = TimeZoneManager.I.GetCurrentTimeZone();
			mPrevAge = TimeZoneManager.I.GetCurrentPlayerAge();

			mFlashTimer.Start();
		}


		public override void Update(GameTime gameTime)
		{
			int timeNow = TimeZoneManager.I.GetCurrentTimeZone();
			int ageNow = TimeZoneManager.I.GetCurrentPlayerAge();

			if(mTravelTimer.IsPlaying())
			{
				if(mTravelTimer.GetPercentageF() >= 1.0f)
				{
					mPrevAge = ageNow;
					mPrevTimezone = timeNow;
					mTravelTimer.FullReset();
				}
			}
			else if (timeNow != mPrevTimezone || ageNow != mPrevAge)
			{
				mTravelTimer.FullReset();
				mTravelTimer.Start();
			}

			if (mFlashTimer.GetPercentageF() >= 1.0f)
			{
				mFlashTimer.Reset();
			}

			

			base.Update(gameTime);
		}

		private Vector2 GetDotPosition(int time, int age)
		{
			Vector2 basePos = GetPosition() + new Vector2(35.0f, 87.0f);

			basePos.Y -= age * 42.0f;
			basePos.X += (time+1) * 38.0f;

			return basePos;
		}

		public override void Draw(DrawInfo info)
		{
			//Draw BG first
			base.Draw(info);

			Vector2 dotPos = GetDotDrawPosition();
			DrawTimeDot(info, dotPos);
		}

		private Vector2 GetDotDrawPosition()
		{
			Vector2 basePos = GetDotPosition(mPrevTimezone, mPrevAge);

			if(mTravelTimer.IsPlaying())
			{
				int timeNow = TimeZoneManager.I.GetCurrentTimeZone();
				int ageNow = TimeZoneManager.I.GetCurrentPlayerAge();

				Vector2 newVec = GetDotPosition(timeNow, ageNow);
				return MonoMath.Lerp(basePos, newVec, mTravelTimer.GetPercentageF());
			}

			return basePos;
		}

		private void DrawTimeDot(DrawInfo info, Vector2 point)
		{
			Color darkCol = new Color(160, 76, 16);
			Color brightCol = new Color(239, 178, 35);

			Color drawCol = MonoMath.Lerp(darkCol, brightCol, MonoMath.UnitWave(mFlashTimer.GetPercentageF()));

			Rect2f rectangle = new Rect2f(point, 3.0f, 3.0f);

			MonoDraw.DrawRectDepth(info, rectangle, drawCol, GetDepth());
		}
	}
}
