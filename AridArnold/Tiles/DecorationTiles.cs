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
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Buk/stalag-tile");
		}
	}
}
