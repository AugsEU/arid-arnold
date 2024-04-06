
namespace AridArnold
{
	internal class ReflectedTileSubmission : ColliderSubmission
	{
		Tile mTargetTile;
		PlatformingEntity mTargetEntity;
		Vector2 mReflectionNormal;
		Vector2 mReflectedPosition;

		public ReflectedTileSubmission(Tile targetTile, PlatformingEntity targetEntity, Vector2 reflection, Vector2 pos)
		{
			mTargetEntity = targetEntity;
			mTargetTile = targetTile;
			mReflectionNormal = reflection;
			mReflectedPosition = pos;
		}

		public override bool CanCollideWith(MovingEntity entity)
		{
			return object.ReferenceEquals(mTargetEntity, entity);
		}

		protected override EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity)
		{
			Vector2 originalPos = entity.GetPos();
			Vector2 originalVel = entity.GetVelocity();

			Vector2 reflectedVel = MonoMath.Reflect(originalVel, mReflectionNormal);

			entity.SetPos(mReflectedPosition);
			entity.SetVelocity(reflectedVel);

			CollisionResults collisionResults = mTargetTile.Collide(entity, gameTime);

			collisionResults.normal = MonoMath.Reflect(collisionResults.normal, mReflectionNormal);

			EntityCollision result = null;

			if (collisionResults.Collided)
			{
				result = new TileEntityCollision(true, collisionResults, mTargetTile);
			}

			entity.SetPos(originalPos);
			entity.SetVelocity(originalVel);

			return result;
		}
	}
}
