namespace AridArnold
{
	/// <summary>
	/// Types of objects we can collect
	/// </summary>
	enum CollectableType
	{
		WaterBottle,
		Flag
	}





	/// <summary>
	/// Manages all items collected within a level and outside
	/// </summary>
	internal class CollectableManager : Singleton<CollectableManager>
	{
		#region rMembers

		Dictionary<CollectableType, uint> mCurrentCollectables = new Dictionary<CollectableType, uint>();

		#endregion rMembers





		#region rCollection

		/// <summary>
		/// Collect an item of a certain type
		/// </summary>
		/// <param name="type">Type of collectable</param>
		/// <param name="number"></param>
		public void CollectItem(CollectableType type, uint number = 1)
		{
			if (mCurrentCollectables.ContainsKey(type))
			{
				mCurrentCollectables[type] += number;
			}
			else
			{
				mCurrentCollectables.Add(type, number);
			}
		}



		/// <summary>
		/// Get number of collected items of type
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns>Number of collected items of type</returns>
		public uint GetCollected(CollectableType type)
		{
			if (!mCurrentCollectables.ContainsKey(type))
			{
				mCurrentCollectables.Add(type, 0);
			}

			return mCurrentCollectables[type];
		}



		/// <summary>
		/// Destroy all collectables back to 0
		/// </summary>
		public void ClearAllCollectables()
		{
			mCurrentCollectables.Clear();
		}

		#endregion rCollection
	}
}
