namespace AridArnold
{
	/// <summary>
	/// Wall tile.
	/// </summary>
	class WallTile : SquareTile
	{
		Animator mAnimation;
		int mWorldIdx;

		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public WallTile(int param, Vector2 position) : base(position)
		{
			mWorldIdx = ProgressManager.I.GetLevelPoint().mWorldIndex - param;
		}


		/// <summary>
		/// Load content for a wall tile.
		/// </summary>
		/// <param name="content"></param>
		public override void LoadContent(ContentManager content)
		{
			mAnimation = ProgressManager.I.GetWorld(mWorldIdx).GetTheme().GenerateWallAnimation(content);

			base.LoadContent(content);
		}



		/// <summary>
		/// Update wall animations
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			mAnimation.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Get texture for this tile
		/// </summary>
		public override Texture2D GetTexture()
		{
			return mAnimation.GetCurrentTexture();
		}
	}
}
