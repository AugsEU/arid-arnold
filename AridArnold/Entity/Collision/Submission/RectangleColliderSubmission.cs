
namespace AridArnold
{
	/// <summary>
	/// Submit a static rectangle for collision
	/// </summary>
	internal class RectangleColliderSubmission : ColliderSubmission
	{
		Rect2f mColliderRect;

		public RectangleColliderSubmission(Rect2f rect)
		{
			mColliderRect = rect;
		}


		public override bool CanCollideWith(MovingEntity entity)
		{
			return true;
		}

		protected override EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mColliderRect);

			//Collision!
			if (results.t.HasValue)
			{
				return new SolidEntityCollision(!mCollidedEntities.Contains(entity), results);
			}

			//No collision
			return null;
		}
	}
}
