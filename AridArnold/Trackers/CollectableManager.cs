namespace AridArnold
{
	/// <summary>
	/// Types of objects we can collect that are gone at the end of the level
	/// </summary>
	enum TransientCollectable
	{
		WaterBottle,
		Flag
	}

	enum PermanentCollectable : byte
	{
		Key,
		Door,
		LevelLock,
		Coin
	}





	/// <summary>
	/// Manages all items collected within a level and outside
	/// </summary>
	internal class CollectableManager : Singleton<CollectableManager>
	{
		#region rMembers

		Dictionary<TransientCollectable, uint> mTransientCollectables = new Dictionary<TransientCollectable, uint>();

		// Note: UInt16 == PermaenetCollectable type but C# has no type def >:| very angry bad language why
		Dictionary<UInt16, uint> mPermanentCollectables = new Dictionary<UInt16, uint>();
		HashSet<UInt64> mSpecificCollected = new HashSet<UInt64>();

		#endregion rMembers





		#region rCollection

		/// <summary>
		/// Collect an item of a certain type
		/// </summary>
		/// <param name="type">Type of collectable</param>
		/// <param name="number"></param>
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
		/// Collect an item of a certain type
		/// </summary>
		public void CollectPermanentItem(Point pos, UInt16 type)
		{
			if (mPermanentCollectables.TryGetValue(type, out uint currentCount))
			{
				mPermanentCollectables[type] = currentCount + 1;
			}
			else
			{
				mPermanentCollectables.Add(type, 1);
			}

			mSpecificCollected.Add(CalculateSpecificID(pos, type));
		}



		/// <summary>
		/// Gain or remove from permanent count
		/// </summary>
		public void ChangePermanentItem(UInt16 type, int delta)
		{
			if (mPermanentCollectables.TryGetValue(type, out uint currentCount))
			{
				MonoDebug.Assert(currentCount >= -delta);
				mPermanentCollectables[type] = (uint)(currentCount + delta);
			}
			else
			{
				MonoDebug.Assert(delta >= 0);
				mPermanentCollectables.Add(type, (uint)delta);
			}
		}



		/// <summary>
		/// Get number of collected items of type
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns>Number of collected items of type</returns>
		public uint GetCollected(TransientCollectable type)
		{
			uint result = 0;
			mTransientCollectables.TryGetValue(type, out result);
			return result;
		}



		/// <summary>
		/// Get number of collected items of type
		/// </summary>
		public uint GetCollected(UInt16 type)
		{
			uint result = 0;
			mPermanentCollectables.TryGetValue(type, out result);
			return result;
		}



		/// <summary>
		/// Do we have a specific collectable?
		/// </summary>
		public bool HasSpecific(Point pos, UInt16 type)
		{
			return mSpecificCollected.Contains(CalculateSpecificID(pos, type));
		}



		/// <summary>
		/// Destroy all transient collectables back to 0
		/// </summary>
		public void ClearTransient()
		{
			mTransientCollectables.Clear();
		}



		/// <summary>
		/// Calculate hash of sorts.
		/// </summary>
		UInt64 CalculateSpecificID(Point pos, UInt16 type)
		{
			byte xPos = (byte)pos.X;
			byte yPos = (byte)pos.Y;
			UInt32 levelID = (UInt32)(CampaignManager.I.GetCurrentLevel().GetID());

			UInt64 ret = ((UInt64)xPos << 56) |
						 ((UInt64)yPos << 48) |
						 ((UInt64)levelID << 16) |
						 type;

			return ret;
		}

		#endregion rCollection
	}
}
