namespace AridArnold
{
	/// <summary>
	/// Different categories for flags.
	///    First 32 bytes of flag is it's category. 
	///    The 32bits after are the "implementation", up to whoever is settting
	///    the flags to differentiate between flags in the same category.
	/// </summary>
	enum FlagCategory : UInt32
	{
		kWaterCollected = 0,
		kCurses,
		kMaxFlagCategory
	}

	/// <summary>
	/// Different types of curses/blessings
	/// </summary>
	enum CurseFlagTypes : UInt32
	{
		kCurseGiven = 0,
		kBlessingLives,
		kBlessingMoney,
		kCurseLives,
		kCurseMoney,
		kMaxCurse
	}



	/// <summary>
	/// Stores various random flags. Basically used as a big bag of random data that needs to be storred by the NPCs and such.
	/// </summary>
	class FlagsManager : Singleton<FlagsManager>
	{
		#region rMembers

		// Save a bunch of flags in a pot
		HashSet<UInt64> mFlagPot;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create flag manager
		/// </summary>
		public FlagsManager()
		{
			Init();
		}



		/// <summary>
		/// Init with no flags
		/// </summary>
		public void Init()
		{
			mFlagPot = new HashSet<UInt64>();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update flag manager.
		/// </summary>
		public void Update(GameTime gameTime)
		{
			// Does nothing? Might delete
		}

		#endregion rUpdate





		#region rFlags

		/// <summary>
		/// Set raw flag value to true/false
		/// </summary>
		public void SetFlag(UInt64 flag, bool value)
		{
			if(value)
			{
				mFlagPot.Add(flag);
			}
			else
			{
				mFlagPot.Remove(flag);
			}
		}



		/// <summary>
		/// Set flag to tre/false
		/// </summary>
		public void SetFlag(FlagCategory category, UInt32 impl, bool value)
		{
			UInt64 rawFlag = GetRawFlag(category, impl);
			SetFlag(rawFlag, value);
		}



		/// <summary>
		/// Get raw flag from category + impl data
		/// </summary>
		public UInt64 GetRawFlag(FlagCategory category, UInt32 impl)
		{
			UInt64 categoryBytes = (UInt64)category;
			UInt64 implExpanded = (UInt64)impl;

			return (implExpanded << 32) | categoryBytes;
		}



		/// <summary>
		/// Is this flag set?
		/// </summary>
		public bool CheckFlag(UInt64 flag)
		{
			return mFlagPot.Contains(flag);
		}



		/// <summary>
		/// Is this flag set?
		/// </summary>
		public bool CheckFlag(FlagCategory flag, UInt32 impl)
		{
			UInt64 rawFlag = (UInt64)GetRawFlag(flag, impl);
			return CheckFlag(rawFlag);
		}

		#endregion rFlags
	}
}
