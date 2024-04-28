namespace AridArnold
{
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

				if (ourPlatEntity.IsGroundedSince(1) || ourGrav != theirGrav)
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
					const float DRAG_THRESH = 20.0f;
					Vector2 dir = MonoMath.Perpendicular(gravity);
					Vector2 addedVelocity = Vector2.Dot(dir, ourPlatEntity.GetVelocity()) * dir;

					float len = addedVelocity.Length();
					if (len > DRAG_THRESH)
					{
						Vector2 dragVec = addedVelocity / len * DRAG_THRESH;
						addedVelocity = (addedVelocity - dragVec) * DRAG_FACTOR + dragVec;
					}

					return addedVelocity;
				}
			}

			return base.GetExtraVelocityInternal(entity);
		}
	}
}
