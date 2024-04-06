
namespace AridArnold
{
	internal class ReflectedTileSubmission : ColliderSubmission
	{
		Tile mTargetTile;
		PlatformingEntity mTargetEntity;
		Vector2 mReflectionNormal;
		Vector2 mReflectionCentre;

		public ReflectedTileSubmission(Tile targetTile, PlatformingEntity targetEntity, Vector2 refNormal, Vector2 refCentre)
		{
			mTargetEntity = targetEntity;
			mTargetTile = targetTile;
			mReflectionNormal = refNormal;
			mReflectionCentre = refCentre;
		}

		public override bool CanCollideWith(MovingEntity entity)
		{
			return object.ReferenceEquals(mTargetEntity, entity);
		}

		protected override EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity)
		{
			Vector2 originalPos = entity.GetPos();
			Vector2 originalVel = entity.GetVelocity();
			CardinalDirection originalGravity = CardinalDirection.Down;
			if(entity is PlatformingEntity platformingEntity)
			{
				originalGravity = platformingEntity.GetGravityDir();
				platformingEntity.SetGravity(Util.ReflectCardinalDirection(originalGravity, mReflectionNormal));
			}

			Vector2 reflectedVel = MonoMath.Reflect(originalVel, mReflectionNormal);

			entity.SetPos(GetReflectedPosition(mTargetEntity));
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
			if (entity is PlatformingEntity platformingEntity2)// BAD I KNOW
			{
				platformingEntity2.SetGravity(originalGravity);
			}

			return result;
		}

		public Vector2 GetReflectedPosition(MovingEntity entity)
		{
			Rect2f entityCol = entity.ColliderBounds();
			Vector2 reflectedPos = MonoMath.Reflect(entity.GetPos(), mReflectionNormal, mReflectionCentre);

			Vector2 colliderSizeVec = new Vector2(entityCol.Width, entityCol.Height);
			Vector2 offset = MonoMath.CompMult(colliderSizeVec, mReflectionNormal);

			reflectedPos += offset;

			return reflectedPos;
		}
	}
}
