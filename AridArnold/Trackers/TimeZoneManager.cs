namespace AridArnold
{
	class TimeZoneManager : Singleton<TimeZoneManager>
	{
		#region rMembers

		int mCurrentTimeZone;

		#endregion rMembers



		#region rInit

		/// <summary>
		/// Init the time manager
		/// </summary>
		public void Init()
		{
			mCurrentTimeZone = 0;
		}

		#endregion rInit





		#region rTime

		/// <summary>
		/// Travel in time
		/// </summary>
		public void SetTimeZone(int timeZone)
		{
			mCurrentTimeZone = timeZone;
		}



		/// <summary>
		/// Get the current timezone
		/// </summary>
		public int GetCurrentTimeZone()
		{
			return mCurrentTimeZone;
		}

		#endregion rTime
	}
}
