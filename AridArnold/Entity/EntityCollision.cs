namespace AridArnold
{
	/// <summary>
	/// Comparison class to sort TileCollision results by their closeness to the entity
	/// </summary>
	class EntityCollisionSorter : IComparer<EntityCollision>
	{
		public int Compare(EntityCollision a, EntityCollision b)
		{
			return a.GetResult().t.Value.CompareTo(b.GetResult().t.Value);
		}
	}



	/// <summary>
	/// Represents a collision with an entity.
	/// </summary>
	class EntityCollision
	{
		public static EntityCollisionSorter COLLISION_SORTER = new EntityCollisionSorter();

		CollisionResults mResult;

		public EntityCollision(CollisionResults result)
		{
			mResult = result;
		}

		public CollisionResults GetResult()
		{
			return mResult;
		}

		public virtual void PostCollisionReact(MovingEntity entity)
		{

		}
	}



	/// <summary>
	/// Represents collision between tile and player.
	/// </summary>
	class TileEntityCollision : EntityCollision
	{
		Point mTileCoord;

		public TileEntityCollision(CollisionResults result, Point tileCoord) : base(result)
		{
			mTileCoord = tileCoord;
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			TileManager.I.GetTile(mTileCoord).OnTouch(entity);
		}
	}
}
