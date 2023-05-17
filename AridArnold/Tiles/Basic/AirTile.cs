namespace AridArnold
{
	/// <summary>
	/// A tile that has no texture or collisions.
	/// </summary>
	class AirTile : Tile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public AirTile(Vector2 position) : base(position)
		{
		}

		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Air");
		}

		/// <summary>
		/// Do not collide with the entity.
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="gameTime">Frame time</param>
		/// <returns></returns>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			// No collision.
			return CollisionResults.None;
		}



		/// <summary>
		/// Get empty bounds
		/// </summary>
		/// <returns>Zero bounds</returns>
		protected override Rect2f CalculateBounds()
		{
			return new Rect2f(Vector2.Zero, Vector2.Zero);
		}



		/// <summary>
		/// Is this tile solid?
		/// </summary>
		/// <returns>True if a tile is solid</returns>
		public override bool IsSolid()
		{
			return false;
		}
	}
}
