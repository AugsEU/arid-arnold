namespace GMTK2023
{
	/// <summary>
	/// Simple timer/stopwatch class.
	/// It is updated automatically.
	/// </summary>
	class MonoTimer
	{
		#region rMembers

		bool mPlaying;
		protected double mElapsedTimeMs;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Constrcut monotimer
		/// </summary>
		public MonoTimer()
		{
			TimeManager.I.RegisterTimer(this);

			mElapsedTimeMs = 0.0;
			mPlaying = false;
		}



		/// <summary>
		/// Remove timer
		/// </summary>
		~MonoTimer()
		{
			TimeManager.I.RemoveTimer(this);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update timer
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			if (mPlaying)
			{
				mElapsedTimeMs += gameTime.ElapsedGameTime.TotalMilliseconds;
			}
		}



		/// <summary>
		/// Start the timer
		/// </summary>
		public void Start()
		{
			mPlaying = true;
		}



		/// <summary>
		/// Stop the timer
		/// </summary>
		public void Stop()
		{
			mPlaying = false;
		}



		/// <summary>
		/// Reset the timer. Doesn't stop it playing.
		/// </summary>
		public void Reset()
		{
			mElapsedTimeMs = 0.0;
		}



		/// <summary>
		/// Stop the timer and reset it.
		/// </summary>
		public void FullReset()
		{
			mPlaying = false;
			mElapsedTimeMs = 0.0;
		}

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Is the timer playing?
		/// </summary>
		/// <returns>True if the timer is playing</returns>
		public bool IsPlaying()
		{
			return mPlaying;
		}



		/// <summary>
		/// Get's elapsed time
		/// </summary>
		/// <returns>Time in MS</returns>
		public double GetElapsedMs()
		{
			return mElapsedTimeMs;
		}



		/// <summary>
		/// Set elapsed milliseconds
		/// </summary>
		public void SetElapsedMs(double elapsed)
		{
			mElapsedTimeMs = elapsed;
		}

		#endregion rUtility
	}






	/// <summary>
	/// Timer that runs on a percentage of time
	/// </summary>
	class PercentageTimer : MonoTimer
	{
		#region rMembers

		double mTotalTime;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct percentage timer
		/// </summary>
		/// <param name="totalTime">Time for it to reach 100%</param>
		public PercentageTimer(double totalTime) : base()
		{
			mTotalTime = totalTime;
		}

		#endregion rInitialisation





		#region rUtility

		/// <summary>
		/// Get percentage as a fraction of 1.0
		/// </summary>
		/// <returns>Number in the range [0.0,1.0]</returns>
		public double GetPercentage()
		{
			if (GetElapsedMs() < mTotalTime)
			{
				return GetElapsedMs() / mTotalTime;
			}

			//Return 1.0 after exceeding the total time
			return 1.0;
		}



		/// <summary>
		/// Get percentage as float.
		/// </summary>
		/// <returns>Number in the range [0.0f, 1.0f]</returns>
		public float GetPercentageF()
		{
			return (float)GetPercentage();
		}


		/// <summary>
		/// Immediately complete a timer.
		/// </summary>
		public void SetComplete()
		{
			mElapsedTimeMs = mTotalTime;
		}



		/// <summary>
		/// Set time as a percentage
		/// </summary>
		public void SetPercentTime(float timePercent)
		{
			mElapsedTimeMs = mTotalTime * timePercent;
		}

		#endregion rUtility
	}





	/// <summary>
	/// Time manager that updates all timers automatically
	/// </summary>
	internal class TimeManager : Singleton<TimeManager>
	{
		#region rMembers

		List<MonoTimer> mTimers = new List<MonoTimer>();

		#endregion rMembers





		#region rUpdate

		/// <summary>
		/// Update all timers
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			foreach (MonoTimer timer in mTimers)
			{
				timer.Update(gameTime);
			}
		}

		#endregion rUpdate





		#region rRegistry

		/// <summary>
		/// Register a timer to be updated automatically.
		/// </summary>
		/// <param name="timer">Timer to add</param>
		public void RegisterTimer(MonoTimer timer)
		{
			mTimers.Add(timer);
		}



		/// <summary>
		/// Remove timer from registry.
		/// </summary>
		/// <param name="timer">Timer to remove</param>
		public void RemoveTimer(MonoTimer timer)
		{
			mTimers.Remove(timer);
		}

		#endregion rRegistry

	}
}
