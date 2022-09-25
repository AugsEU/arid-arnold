namespace AridArnold
{
	class StalactiteTile : AirTile
	{
		/// <summary>
		/// StalactiteTile constructor
		/// </summary>
		public StalactiteTile() : base()
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
