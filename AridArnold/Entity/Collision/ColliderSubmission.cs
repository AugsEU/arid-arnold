namespace AridArnold
{
	/// <summary>
	/// Objects can submit auxillary hitboxes to the entity manager for review.
	/// </summary>
	abstract class ColliderSubmission
	{
		/// <summary>
		/// Can we collide with this entity?
		/// </summary>
		public abstract bool CanCollideWith(MovingEntity entity);



		/// <summary>
		/// Do collision check and get entity collision result. Can return null
		/// </summary>
		public abstract EntityCollision GetEntityCollision(GameTime gameTime, MovingEntity entity);
	}



	/// <summary>
	/// An entity submits themselves to be collided with other entities.
	/// </summary>
	class EntityColliderSubmission : ColliderSubmission
	{
		#region rMembers

		MovingEntity mEntity;

		#endregion rMembers



		#region rInitialisation

		/// <summary>
		/// Init EntityColliderSubmission with an entity.
		/// </summary>
		public EntityColliderSubmission(MovingEntity entity)
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
		public override EntityCollision GetEntityCollision(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = CollideWith(gameTime, entity);

			//Collision!
			if(results.t.HasValue)
			{
				return new EntityEntityCollision(results, mEntity);
			}

			//No collision
			return null;
		}

		#endregion rCollision
	}
}
