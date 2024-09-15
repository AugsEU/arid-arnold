namespace AridArnold
{
	/// <summary>
	/// Stores various random flags. Basically used as a big bag of random data that needs to be storred by the NPCs and such.
	/// </summary>
	class FlagsManager : Singleton<FlagsManager>
	{
		#region rMembers

		// Save a bunch of flags in a pot
		HashSet<UInt64> mFlagPot;
		HashSet<UInt64> mStartSequenceFlagPot;

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
		/// Set flag to tre/false
		/// </summary>
		public void SetFlag(FlagCategory category, bool value)
		{
			UInt64 rawFlag = GetRawFlag(category, 0);
			SetFlag(rawFlag, value);
		}



		/// <summary>
		/// Get raw flag from category + impl data
		/// </summary>
		public UInt64 GetRawFlag(FlagCategory category, UInt32 impl = 0)
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
		public bool CheckFlag(FlagCategory flag, UInt32 impl = 0)
		{
			UInt64 rawFlag = (UInt64)GetRawFlag(flag, impl);
			return CheckFlag(rawFlag);
		}

		#endregion rFlags





		#region rSequence

		/// <summary>
		/// Call at start of sequence
		/// </summary>
		public void NotifySequenceStart()
		{
			mStartSequenceFlagPot = new HashSet<UInt64>(mFlagPot);
		}



		/// <summary>
		/// Compare two flags.
		/// -1 Was set now isn't
		/// 0 Same as before
		/// 1 Wasn't set and now is
		/// </summary>
		public int GetFlagSeqDiff(FlagCategory flag, UInt32 impl = 0)
		{
			UInt64 rawFlag = GetRawFlag(flag, impl);
			bool wasSet = mStartSequenceFlagPot.Contains(rawFlag);
			bool isSet = mFlagPot.Contains(rawFlag);

			if(wasSet && !isSet)
			{
				return -1;
			}
			else if(!wasSet && isSet)
			{
				return 1;
			}

			return 0;
		}

		#endregion rSequence





		#region rSerial

		/// <summary>
		/// Read from a binary file
		/// </summary>
		public void ReadBinary(BinaryReader br)
		{
			mFlagPot.Clear();
			int numFlags = br.ReadInt32();
			for(int i = 0; i < numFlags; i++)
			{
				UInt64 flagID = br.ReadUInt64();
				mFlagPot.Add(flagID);
			}
		}



		/// <summary>
		/// Write to a binary file
		/// </summary>
		public void WriteBinary(BinaryWriter bw)
		{
			bw.Write((int)mFlagPot.Count);
			foreach(UInt64 flagID in mFlagPot)
			{
				bw.Write(flagID);
			}
		}

		#endregion rSerial
	}
}
