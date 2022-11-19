namespace AridArnold
{
	/// <summary>
	/// Tile for solid walls/floors
	/// </summary>
	abstract class WallTile : SquareTile
	{
		/// <summary>
		/// Wall tile
		/// </summary>
		/// <param name="position"></param>
		public WallTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static WallTile InstantiateWallTile(Vector2 pos, int param)
		{
			int worldIndex = ProgressManager.I.GetLevelPoint().mWorldIndex;
			worldIndex -= param;

			switch (worldIndex)
			{
				case 0: //Iron Works
					return new StaticWallTile(pos, "IronWorks/steel");
				case 1: //Land of mirrors
					return new StaticWallTile(pos, "Mirror/cobble");
				case 2: //Buk's Cave
					return new StaticWallTile(pos, "Buk/cave");
				case 3: //The Lab
					return new LabWall(pos);
				default:
					break;
			}

			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Wall tile with a static image.
	/// </summary>
	class StaticWallTile : WallTile
	{
		string mTexturePath;

		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public StaticWallTile(Vector2 position, string path) : base(position)
		{
			mTexturePath = path;
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/" + mTexturePath);
		}
	}





	/// <summary>
	/// Wall tile with a static image.
	/// </summary>
	class AnimWallTile : WallTile
	{
		protected Animator mAnimation;

		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public AnimWallTile(Vector2 position) : base(position)
		{
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
