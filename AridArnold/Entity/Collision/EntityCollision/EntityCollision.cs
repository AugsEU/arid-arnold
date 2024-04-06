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
		protected CollisionResults mResult;
		protected bool mFirstTime;

		public EntityCollision(bool firstTime, CollisionResults result)
		{
			mResult = result;
			mFirstTime = firstTime;
		}

		public CollisionResults GetResult()
		{
			return mResult;
		}

		public virtual void PostCollisionReact(MovingEntity entity)
		{

		}

		public Vector2 GetExtraVelocity(MovingEntity entity)
		{
			if (mFirstTime)
			{
				return GetExtraVelocityInternal(entity);
			}

			return Vector2.Zero;
		}

		protected virtual Vector2 GetExtraVelocityInternal(MovingEntity entity)
		{
			return Vector2.Zero;
		}
	}
}
