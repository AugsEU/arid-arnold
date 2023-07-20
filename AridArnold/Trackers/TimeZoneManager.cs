namespace AridArnold
{
	class TimeZoneManager : Singleton<TimeZoneManager>
	{
		#region rMembers

		int mCurrentTimeZone;
		int mCurrentPlayerAge;

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
		/// Get the current timezone
		/// </summary>
		public int GetCurrentTimeZone()
		{
			return mCurrentTimeZone;
		}



		/// <summary>
		/// How old is the player?
		/// </summary>
		public int GetCurrentPlayerAge()
		{
			return mCurrentPlayerAge;
		}



		/// <summary>
		/// Travel forwards in time.
		/// </summary>
		public void TimeTravel()
		{
			mCurrentTimeZone++;
			EventManager.I.SendEvent(EventType.TimeChanged, new EArgs(this));
		}



		/// <summary>
		/// Travel backwards in time.
		/// </summary>
		public void AntiTimeTravel()
		{
			mCurrentTimeZone--;
			EventManager.I.SendEvent(EventType.TimeChanged, new EArgs(this));
		}



		/// <summary>
		/// Age player(also travels us in time).
		/// </summary>
		public void AgePlayer()
		{
			mCurrentTimeZone++;
			mCurrentPlayerAge++;
			EventManager.I.SendEvent(EventType.TimeChanged, new EArgs(this));
		}



		/// <summary>
		/// Anti age player(also travels backwards in time).
		/// </summary>
		public void AntiAgePlayer()
		{
			mCurrentTimeZone--;
			mCurrentPlayerAge--;
			EventManager.I.SendEvent(EventType.TimeChanged, new EArgs(this));
		}

		#endregion rTime
	}
}
