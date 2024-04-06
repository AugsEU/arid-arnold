namespace AridArnold
{
	/// <summary>
	/// An entity submits themselves to be collided with other entities.
	/// </summary>
	class EntityColliderSubmission : ColliderSubmission
	{
		#region rMembers

		Entity mEntity;

		#endregion rMembers



		#region rInitialisation

		/// <summary>
		/// Init EntityColliderSubmission with an entity.
		/// </summary>
		public EntityColliderSubmission(Entity entity)
		{
			mEntity = entity;
		}

		#endregion rInitialisation


		#region rCollision

		/// <summary>
		/// Entity can collide with any
		/// </summary>
		public override bool CanCollideWith(MovingEntity entity)
		{
			return !object.ReferenceEquals(mEntity, entity);
		}



		/// <summary>
		/// Collision check with other entity.
		/// </summary>
		private CollisionResults CollideWith(GameTime gameTime, MovingEntity entity)
		{
			return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mEntity.ColliderBounds());
		}


		/// <summary>
		/// Do collision with other entity
		/// </summary>
		protected override EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = CollideWith(gameTime, entity);

			//Collision!
			if (results.t.HasValue)
			{
				return new EntityEntityCollision(!mCollidedEntities.Contains(entity), results, mEntity);
			}

			//No collision
			return null;
		}

		#endregion rCollision
	}
}
