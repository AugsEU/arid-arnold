using static AridArnold.ProfileSaveInfo;

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
			mCurrentPlayerAge = 0;
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
			if(time != mCurrentTimeZone || age != mCurrentPlayerAge)
			{
				NotifyTimeChanged();
			}
			mCurrentTimeZone = time;
			mCurrentPlayerAge = age;
		}


		/// <summary>
		/// Travel forwards in time.
		/// </summary>
		public void TimeTravel()
		{
			mCurrentTimeZone++;
			NotifyTimeChanged();
		}



		/// <summary>
		/// Travel backwards in time.
		/// </summary>
		public void AntiTimeTravel()
		{
			mCurrentTimeZone--;
			NotifyTimeChanged();
		}



		/// <summary>
		/// Age player(also travels us in time).
		/// </summary>
		public void AgePlayer()
		{
			mCurrentTimeZone++;
			mCurrentPlayerAge++;
			NotifyTimeChanged();
		}



		/// <summary>
		/// Anti age player(also travels backwards in time).
		/// </summary>
		public void AntiAgePlayer()
		{
			mCurrentTimeZone--;
			mCurrentPlayerAge--;
			NotifyTimeChanged();
		}


		/// <summary>
		/// Do stuff when time has changed.
		/// </summary>
		private void NotifyTimeChanged()
		{
			// Unlock panel.
			FlagsManager.I.SetFlag(FlagCategory.kPanelsUnlocked, (uint)PanelUnlockedType.k4DLocator, true);

			// Send event
			EventManager.I.TriggerEvent(EventType.TimeChanged);
		}

		#endregion rTime





		#region rSerial

		/// <summary>
		/// Read from a binary file
		/// </summary>
		public void ReadBinary(BinaryReader br)
		{
			mCurrentTimeZone = br.ReadInt32();
			mCurrentPlayerAge = br.ReadInt32();
		}



		/// <summary>
		/// Write to a binary file
		/// </summary>
		public void WriteBinary(BinaryWriter bw)
		{
			bw.Write(mCurrentTimeZone);
			bw.Write(mCurrentPlayerAge);
		}

		#endregion rSerial
	}
}
