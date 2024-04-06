namespace AridArnold
{
	/// <summary>
	/// Platform submits itself for collision
	/// </summary>
	class PlatformColliderSubmission : ColliderSubmission
	{
		#region rMembers

		Vector2 mVelocity;
		Vector2 mPosition;
		CardinalDirection mRotation;
		float mWidth;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init EntityColliderSubmission with a top left coord and the width of the platform.
		/// </summary>
		public PlatformColliderSubmission(Vector2 velocity, Vector2 leftPos, float width, CardinalDirection rotation)
		{
			mVelocity = velocity;
			mPosition = leftPos;
			mWidth = width;
			mRotation = rotation;
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Platforms can collide with all entities.
		/// </summary>
		public override bool CanCollideWith(MovingEntity entity)
		{
			return true;
		}



		/// <summary>
		/// Collision check with other entity.
		/// </summary>
		private CollisionResults CollideWith(GameTime gameTime, MovingEntity entity)
		{
			if (!entity.CollideWithPlatforms())
			{
				if (entity is PlatformingEntity)
				{
					PlatformingEntity platformingEntity = (PlatformingEntity)entity;

					if (mRotation == Util.InvertDirection(platformingEntity.GetGravityDir()))
					{
						return CollisionResults.None;
					}
				}
				else
				{
					return CollisionResults.None;
				}
			}

			return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mPosition, mWidth, mRotation);
		}

		protected override EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = CollideWith(gameTime, entity);

			//Collision!
			if (results.t.HasValue)
			{
				return new MovingPlatformEntityCollision(!mCollidedEntities.Contains(entity), results, mVelocity);
			}

			//No collision
			return null;
		}

		#endregion rCollision
	}
}
