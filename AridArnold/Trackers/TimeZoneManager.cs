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
		/// 
		/// </summary>
		/// <param name="time"></param>
		public void SetCurrentTimeZoneAndAge(int time, int age)
		{
			mCurrentTimeZone = time;
			mCurrentPlayerAge = age;
			EventManager.I.TriggerEvent(EventType.TimeChanged);
		}


		/// <summary>
		/// Travel forwards in time.
		/// </summary>
		public void TimeTravel()
		{
			mCurrentTimeZone++;
			EventManager.I.TriggerEvent(EventType.TimeChanged);
		}



		/// <summary>
		/// Travel backwards in time.
		/// </summary>
		public void AntiTimeTravel()
		{
			mCurrentTimeZone--;
			EventManager.I.TriggerEvent(EventType.TimeChanged);
		}



		/// <summary>
		/// Age player(also travels us in time).
		/// </summary>
		public void AgePlayer()
		{
			mCurrentTimeZone++;
			mCurrentPlayerAge++;
			EventManager.I.TriggerEvent(EventType.TimeChanged);
		}



		/// <summary>
		/// Anti age player(also travels backwards in time).
		/// </summary>
		public void AntiAgePlayer()
		{
			mCurrentTimeZone--;
			mCurrentPlayerAge--;
			EventManager.I.TriggerEvent(EventType.TimeChanged);
		}

		#endregion rTime
	}
}
