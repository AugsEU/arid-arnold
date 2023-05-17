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
			return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), GetBounds());
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
