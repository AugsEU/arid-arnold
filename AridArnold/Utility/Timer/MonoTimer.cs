namespace AridArnold
{
	/// <summary>
	/// Simple timer/stopwatch class.
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
			mElapsedTimeMs = 0.0;
			mPlaying = false;
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



		/// <summary>
		/// Play timer from 0
		/// </summary>
		public void ResetStart()
		{
			Reset();
			Start();
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
}
