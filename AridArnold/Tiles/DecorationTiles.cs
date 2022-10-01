namespace AridArnold
{
	class StalactiteTile : AirTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public StalactiteTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/stalag-tile");
		}
	}
}
