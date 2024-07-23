namespace AridArnold
{
	// A collectable ID represents a type of collectable.
	// First 8 bytes are the category of collectable.
	// Next 8 bytes are the "implementation", used for sub-types.
	// Coins are all the same category but use the "implementation" for their sub-type
	using CollectableID = UInt16;

	/// <summary>
	/// Manages all items collected within a level and outside
	/// </summary>
	internal class CollectableManager : Singleton<CollectableManager>
	{
		#region rMembers

		// These are cleared at the end of the level. Mainly used to keep track of level objectives
		Dictionary<TransientCollectable, uint> mTransientCollectables = new Dictionary<TransientCollectable, uint>();

		PermanentCollectableState mCurrent = new PermanentCollectableState();
		PermanentCollectableState mStartSequence = new PermanentCollectableState();
		PermanentCollectableState mStartLevel = new PermanentCollectableState();

		#endregion rMembers


		#region rInit

		public void ResetToDefault()
		{
			mTransientCollectables = new Dictionary<TransientCollectable, uint>();

			mCurrent = new PermanentCollectableState();
			mStartSequence = new PermanentCollectableState();
			mStartLevel = new PermanentCollectableState();
		}

		#endregion rInit



		#region rCollection

		/// <summary>
		/// Collect an item of a certain type
		/// </summary>
		public void CollectSpecificItem(CollectableID type, Point pos)
		{
			int levelID = CampaignManager.I.GetCurrentLevel().GetID();
			SpecificCollectableID specificID = new SpecificCollectableID(type, pos, levelID);
			bool addedResult = mCurrent.mSpecificCollected.Add(specificID);

			if(addedResult)
			{
				// This was a new collectable, increment count.
				IncPermanentCount(type, 1);
			}
			else
			{
				MonoDebug.Log("Warning: Re-collecting permanent collectable.");
			}
		}



		/// <summary>
		/// Gain or remove from permanent count
		/// </summary>
		public void IncPermanentCount(CollectableID type, int delta)
		{
			if (mCurrent.mCollectableCounts.TryGetValue(type, out uint currentCount))
			{
				MonoDebug.Assert(delta >= 0 || currentCount >= -delta);
				mCurrent.mCollectableCounts[type] = (uint)(currentCount + delta);
			}
			else
			{
				MonoDebug.Assert(delta >= 0);
				mCurrent.mCollectableCounts.Add(type, (uint)delta);
			}
		}



		/// <summary>
		/// Set permanent count
		/// </summary>
		public void SetPermanentCount(CollectableID type, uint num)
		{
			mCurrent.mCollectableCounts[type] = num;
		}



		/// <summary>
		/// Get number of collected items of type
		/// </summary>
		public uint GetNumCollected(CollectableID type)
		{
			uint result = 0;
			mCurrent.mCollectableCounts.TryGetValue(type, out result);
			return result;
		}



		/// <summary>
		/// Get number of collected items of type
		/// </summary>
		public uint GetNumCollected(CollectableCategory type, byte impl = 0)
		{
			CollectableID key = GetCollectableID(type, impl);
			return GetNumCollected(key);
		}



		/// <summary>
		/// Do we have a specific collectable?
		/// </summary>
		public bool HasSpecific(Point pos, UInt16 type)
		{
			int levelID = CampaignManager.I.GetCurrentLevel().GetID();
			SpecificCollectableID specificID = new SpecificCollectableID(type, pos, levelID);
			return mCurrent.mSpecificCollected.Contains(specificID);
		}



		/// <summary>
		/// Generate collectable ID
		/// </summary>
		static public CollectableID GetCollectableID(CollectableCategory category, byte impl = 0)
		{
			UInt16 categoryBytes = (UInt16)category;
			return (UInt16)(categoryBytes | impl << 8);
		}

		#endregion rCollection



		#region rLevelSequence

		public void NotifySequenceBegin()
		{
			mStartSequence.CopyFrom(ref mCurrent);
		}

		public void NotifyLevelBegin()
		{
			mStartLevel.CopyFrom(ref mCurrent);
		}

		public void NotifyLevelFail()
		{
			mCurrent.CopyFrom(ref mStartLevel);
		}

		public void NotifySequenceFail()
		{
			mCurrent.CopyFrom(ref mStartSequence);
		}

		#endregion rLevelSequence



		#region rTransientItems

		/// <summary>
		/// Increment transient collectable
		/// </summary>
		public void CollectTransientItem(TransientCollectable type, uint number = 1)
		{
			if (mTransientCollectables.TryGetValue(type, out uint currentCount))
			{
				mTransientCollectables[type] = currentCount + number;
			}
			else
			{
				mTransientCollectables.Add(type, number);
			}
		}



		/// <summary>
		/// Get number of collected items of type
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns>Number of collected items of type</returns>
		public uint GetNumTransient(TransientCollectable type)
		{
			uint result = 0;
			mTransientCollectables.TryGetValue(type, out result);
			return result;
		}



		/// <summary>
		/// Destroy all transient collectables back to 0
		/// </summary>
		public void ClearTransient()
		{
			mTransientCollectables.Clear();
		}

		#endregion rTransientItems





		#region rSerial

		/// <summary>
		/// Read from a binary file
		/// </summary>
		public void ReadBinary(BinaryReader br)
		{
			ResetToDefault();

			int numCollectableCounts = br.ReadInt32();
			for(int i = 0; i < numCollectableCounts; i++)
			{
				CollectableID collectableID = br.ReadUInt16();
				uint count = br.ReadUInt32();
				mCurrent.mCollectableCounts.Add(collectableID, count);
			}

			int numSpecificCollected = br.ReadInt32();
			for (int i = 0; i < numSpecificCollected; i++)
			{
				CollectableID collectableID = br.ReadUInt16();
				UInt64 specificId= br.ReadUInt64();
				mCurrent.mSpecificCollected.Add(new SpecificCollectableID(collectableID, specificId));
			}
		}



		/// <summary>
		/// Write to a binary file
		/// </summary>
		public void WriteBinary(BinaryWriter bw)
		{
			int numCollectableCounts = mCurrent.mCollectableCounts.Count;
			bw.Write(numCollectableCounts);
			foreach(var countValuePair in mCurrent.mCollectableCounts)
			{
				bw.Write(countValuePair.Key);
				bw.Write(countValuePair.Value);
			}

			int numSpecificCollected = mCurrent.mSpecificCollected.Count;
			bw.Write(numSpecificCollected);
			foreach(var specificValuePair in mCurrent.mSpecificCollected)
			{
				bw.Write(specificValuePair.mCollectableID);
				bw.Write(specificValuePair.mSpecificID);
			}
		}

		#endregion rSerial
	}
}
