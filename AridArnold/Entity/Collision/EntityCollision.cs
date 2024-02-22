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



	/// <summary>
	/// Represents collision between tile and a solid hitbox.
	/// </summary>
	class SolidEntityCollision : EntityCollision
	{
		public SolidEntityCollision(bool firstTime, CollisionResults result) : base(firstTime, result)
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

		public TileEntityCollision(bool firstTime, CollisionResults result, Point tileCoord) : base(firstTime, result)
		{
			mTileCoord = tileCoord;
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			TileManager.I.GetTile(mTileCoord).OnTouch(entity, mResult);

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
		Entity mEntity;

		public EntityEntityCollision(bool firstTime, CollisionResults result, Entity entity) : base(firstTime, result)
		{
			mEntity = entity;
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			//Both react.
			entity.OnCollideEntity(mEntity);
			mEntity.OnCollideEntity(entity);

			//Only treat platforming entities as "solid" when grounded.
			// TODO: Use polymorphism to avoid this dynamic cast.
			if (mEntity is PlatformingEntity && entity is PlatformingEntity)
			{
				PlatformingEntity ourPlatEntity = (PlatformingEntity)mEntity;
				PlatformingEntity theirPlatEntity = (PlatformingEntity)entity;

				CardinalDirection ourGrav = ourPlatEntity.GetGravityDir();
				CardinalDirection theirGrav = theirPlatEntity.GetGravityDir();

				if (ourPlatEntity.IsGroundedSince(16) || ourGrav != theirGrav)
				{
					base.PostCollisionReact(entity);
				}
			}
			else
			{
				base.PostCollisionReact(entity);
			}
		}



		protected override Vector2 GetExtraVelocityInternal(MovingEntity entity)
		{
			if (entity is PlatformingEntity && mEntity is PlatformingEntity)
			{
				// Yeah I know... rtti bad
				PlatformingEntity theirPlatEntity = (PlatformingEntity)entity;
				PlatformingEntity ourPlatEntity = (PlatformingEntity)mEntity;

				CardinalDirection ourGrav = ourPlatEntity.GetGravityDir();
				CardinalDirection theirGrav = theirPlatEntity.GetGravityDir();

				if (ourGrav != theirGrav || ourPlatEntity.OnGround() == false)
				{
					return Vector2.Zero;
				}

				Vector2 gravity = theirPlatEntity.GravityVecNorm();

				if (Vector2.Dot(gravity, mResult.normal) < -0.001f)
				{
					const float DRAG_FACTOR = 0.4f;
					const float DRAG_THRESH = 5.0f;
					Vector2 dir = MonoMath.Perpendicular(gravity);
					Vector2 addedVelocity = Vector2.Dot(dir, ourPlatEntity.GetVelocity()) * dir;

					float len = addedVelocity.Length();
					if (len > DRAG_THRESH)
					{
						Vector2 dragVec = (addedVelocity / len) * DRAG_THRESH;
						addedVelocity = (addedVelocity - dragVec) * DRAG_FACTOR + dragVec;
					}

					return addedVelocity;
				}
			}

			return base.GetExtraVelocityInternal(entity);
		}
	}



	/// <summary>
	/// Represents collision between a moving platform and an entity.
	/// </summary>
	class MovingPlatformCollision : SolidEntityCollision
	{
		Vector2 mAddedVel;

		public MovingPlatformCollision(bool firstTime, CollisionResults result, Vector2 velocity) : base(firstTime, result)
		{
			mAddedVel = velocity;
		}

		protected override Vector2 GetExtraVelocityInternal(MovingEntity entity)
		{
			if (entity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;
				Vector2 gravity = platformingEntity.GravityVecNorm();

				if (Vector2.Dot(gravity, mResult.normal) < -0.001f)
				{
					return mAddedVel;
				}
			}

			return base.GetExtraVelocityInternal(entity);
		}
	}
}
