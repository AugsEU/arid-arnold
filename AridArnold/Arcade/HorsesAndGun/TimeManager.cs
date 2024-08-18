namespace HorsesAndGun
{
	class MonoTimer
	{
		bool mPlaying;
		double mElapsedTimeMs;
		double mTimeSpeed;

		public MonoTimer()
		{
			TimeManager.I.RegisterTimer(this);

			mElapsedTimeMs = 0.0;
			mPlaying = false;
			mTimeSpeed = 1.0;
		}

		~MonoTimer()
		{
			TimeManager.I.RemoveTimer(this);
		}

		public double GetElapsedMs()
		{
			return mElapsedTimeMs;
		}

		public void Start()
		{
			mPlaying = true;
		}

		public void Update(GameTime gameTime)
		{
			if (mPlaying)
			{
				mElapsedTimeMs += gameTime.ElapsedGameTime.TotalMilliseconds * mTimeSpeed;
			}
		}

		public void Stop()
		{
			mPlaying = false;
		}

		public void Reset()
		{
			mElapsedTimeMs = 0.0;
		}

		public void FullReset()
		{
			mPlaying = false;
			mElapsedTimeMs = 0.0;
			mTimeSpeed = 1.0;
		}

		public bool IsPlaying()
		{
			return mPlaying;
		}

		public void SetTimeSpeed(double timeSpeed)
		{
			mTimeSpeed = timeSpeed;
		}
	}

	internal class TimeManager : Singleton<TimeManager>
	{
		List<MonoTimer> mTimers = new List<MonoTimer>();

		public void RegisterTimer(MonoTimer timer)
		{
			mTimers.Add(timer);
		}

		public void RemoveTimer(MonoTimer timer)
		{
			mTimers.Remove(timer);
		}


		public void Update(GameTime gameTime)
		{
			foreach (MonoTimer timer in mTimers)
			{
				timer.Update(gameTime);
			}
		}

		public void Reset()
		{
			mTimers.Clear();
		}
	}
}
