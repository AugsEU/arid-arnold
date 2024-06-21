namespace AridArnold
{
	using System.Data.Common;
	using System.Net;
	// A collectable ID represents a type of collectable.
	// First 8 bytes are the category of collectable.
	// Next 8 bytes are the "implementation", used for sub-types.
	// Coins are all the same category but use the "implementation" for their sub-type
	using CollectableID = UInt16;



	/// <summary>
	/// Types of objects we can collect that are gone at the end of the level
	/// </summary>
	enum TransientCollectable
	{
		WaterBottle,
		Flag
	}



	/// <summary>
	/// Types of collectable
	/// </summary>
	enum CollectableCategory : byte
	{
		Key,
		Door,
		LevelLock,
		Coin,
		WaterBottle
	}



	/// <summary>
	/// ID for a specific collectable.
	/// E.g. the key specifically in level 1
	/// </summary>
	struct SpecificCollectableID : IEquatable<SpecificCollectableID>
	{
		public CollectableID mCollectableID; // Type of collectable
		public UInt64 mSpecificID;

		public SpecificCollectableID(CollectableID collectableID, UInt64 specificID)
		{
			mCollectableID = collectableID;
			mSpecificID = specificID;
		}

		public SpecificCollectableID(CollectableID collectableID, Point pos, int levelID)
		{
			UInt64 xByte = (UInt64)pos.X;
			UInt64 yByte = (UInt64)pos.Y;
			UInt64 levelIDExpanded = (UInt64)(levelID);

			mSpecificID = levelIDExpanded | (xByte << 32) | (yByte << 48);
			mCollectableID = collectableID;
		}

		public bool Equals(SpecificCollectableID other)
		{
			return other.mCollectableID == mCollectableID &&
				other.mSpecificID == mSpecificID;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(mCollectableID.GetHashCode(), mSpecificID.GetHashCode());
		}
	}


	/// <summary>
	/// Stores the state of the permanent collectables
	/// </summary>
	struct PermanentCollectableState
	{
		// Store how many of each collectable we have.
		public Dictionary<CollectableID, uint> mCollectableCounts;

		// Store which collectables we have already gotten.
		public HashSet<SpecificCollectableID> mSpecificCollected;

		public PermanentCollectableState()
		{
			mCollectableCounts = new Dictionary<CollectableID, uint>();
			mSpecificCollected = new HashSet<SpecificCollectableID>();
		}

		/// <summary>
		/// Set this to a deep copy of other
		/// </summary>
		public void CopyFrom(ref PermanentCollectableState other)
		{
			mCollectableCounts = new Dictionary<CollectableID, uint>(other.mCollectableCounts);
			mSpecificCollected = new HashSet<SpecificCollectableID>(other.mSpecificCollected);
		}
	}
}
