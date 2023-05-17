using AridArnold.Tiles.Basic;

namespace AridArnold
{
    /// <summary>
    /// Wall tile.
    /// </summary>
    class WallTile : SquareTile
	{
		Animator mAnimation;
		string mAnimName;

		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public WallTile(Vector2 position, string animName) : base(position)
		{
			mAnimName = animName;
		}


		/// <summary>
		/// Load content for a wall tile.
		/// </summary>
		/// <param name="content"></param>
		public override void LoadContent()
		{
			mAnimation = MonoData.I.LoadAnimator(mAnimName);
			mAnimation.Play(RandomManager.I.GetDraw().GetUnitFloat());

			base.LoadContent();
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
