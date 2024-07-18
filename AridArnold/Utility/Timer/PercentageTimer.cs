
namespace AridArnold
{
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
}
