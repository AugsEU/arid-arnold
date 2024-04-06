namespace AridArnold
{
	/// <summary>
	/// Simple tile with a square hitbox. Does nothing else.
	/// </summary>
	abstract class SquareTile : Tile
	{
		public SquareTile(Vector2 position) : base(position)
		{
		}


		/// <summary>
		/// Resolve collision with an entity
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="gameTime">Frame time</param>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			Rect2f entityCol = entity.ColliderBounds();
			Vector2 entityVecDisp = entity.VelocityToDisplacement(gameTime);
			Rect2f ourBounds = GetBounds();

			return Collision2D.MovingRectVsRect(entityCol, entityVecDisp, ourBounds);
		}


		/// <summary>
		/// Is this tile solid?
		/// </summary>
		/// <returns>True if a tile is solid</returns>
		public override bool IsSolid()
		{
			return true;
		}
	}
}
