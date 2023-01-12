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

		public virtual Vector2 GetExtraVelocity(MovingEntity entity)
		{
			return Vector2.Zero;
		}
	}



	/// <summary>
	/// Represents collision between tile and a solid hitbox.
	/// </summary>
	class SolidEntityCollision : EntityCollision
	{
		public SolidEntityCollision(CollisionResults result) : base(result)
		{
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			entity.ReactToCollision(mResult.normal);
		}
	}



	/// <summary>
	/// Represents collision between tile and an entity.
	/// </summary>
	class TileEntityCollision : SolidEntityCollision
	{
		Point mTileCoord;

		public TileEntityCollision(CollisionResults result, Point tileCoord) : base(result)
		{
			mTileCoord = tileCoord;
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			TileManager.I.GetTile(mTileCoord).OnTouch(entity);

			base.PostCollisionReact(entity);
		}
	}



	/// <summary>
	/// Represents collision between a entity and an entity.
	/// </summary>
	class EntityEntityCollision : SolidEntityCollision
	{
		// A bit confusing, but this represents the entity(treated like a static object) that collided with
		// the entity calling UpdateCollisionEntity
		MovingEntity mEntity;

		public EntityEntityCollision(CollisionResults result, MovingEntity entity) : base(result)
		{
			mEntity = entity;
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			//Only treat platforming entities as "solid" when grounded.
			// TODO: Use polymorphism to avoid this dynamic cast.
			if (mEntity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)mEntity;

				if (platformingEntity.pGrounded)
				{
					base.PostCollisionReact(entity);
				}
			}
			else
			{
				base.PostCollisionReact(entity);
			}
		}
	}


	/// <summary>
	/// Represents collision between a moving platform and an entity.
	/// </summary>
	class MovingPlatformCollision : SolidEntityCollision
	{
		Vector2 mAddedVel;

		public MovingPlatformCollision(CollisionResults result, Vector2 velocity) : base(result)
		{
			mAddedVel = velocity;
		}

		public override Vector2 GetExtraVelocity(MovingEntity entity)
		{
			if(entity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity =(PlatformingEntity)entity;
				Vector2 gravity = platformingEntity.GravityVecNorm();

				if(Vector2.Dot(gravity, mResult.normal) < -0.001f)
				{
					return mAddedVel;
				}
			}
			
			return base.GetExtraVelocity(entity);
		}
	}
}
