namespace AridArnold
{
	struct TileTexDrawInfo
	{
		public TileTexDrawInfo()
		{
			mTileIndex = Point.Zero;
			mRotation = 0.0f;
			mEffect = SpriteEffects.None;
		}

		public Point mTileIndex;
		public float mRotation;
		public SpriteEffects mEffect;
	}


	/// <summary>
	/// Hide away these labourious functions
	/// </summary>
	static class TileTextureHelpers
	{
		/// <summary>
		/// Find out which part of the tile atlas to draw
		/// </summary>
		public static TileTexDrawInfo GetTileDrawInfo(Tile tile)
		{
			TileTexDrawInfo returnInfo;
			Texture2D tileTexture = tile.GetTexture();
			int tileSize = (int)Tile.sTILE_SIZE;

			//Square texture, draw as is.
			if (tileTexture.Width == tileSize && tileTexture.Height == tileSize)
			{
				//Square textures can be rotated freely.
				//Others can't since they need ot be rotated to fit together.
				returnInfo = new TileTexDrawInfo();
				returnInfo.mRotation = tile.GetRotation();
				returnInfo.mEffect = tile.GetEffect();
			}
			//Otherwise, look for texture with different edge types
			else if (tileTexture.Width == 6 * tileSize && tileTexture.Height == tileSize)
			{
				//Needs rotating
				returnInfo = SetupTileWithRotation(tile.GetAdjacency());
			}
			else if (tileTexture.Width == 8 * tileSize && tileTexture.Height == 2 * tileSize)
			{
				returnInfo = SetupTileNoRotation(tile.GetAdjacency());
			}
			else
			{
				throw new Exception("Unhandled texture dimensions");
			}

			return returnInfo;
		}




		/// <summary>
		/// Used for textures that have all 16 tile types
		/// </summary>
		/// <param name="adjacency">Adjacency type</param>
		/// <param name="tileIndex">Output variable of which sub-section of the texture to use</param>
		private static TileTexDrawInfo SetupTileNoRotation(AdjacencyType adjacency)
		{
			TileTexDrawInfo ret = new TileTexDrawInfo();

			switch (adjacency)
			{
				case AdjacencyType.None:
					ret.mTileIndex.X = 7;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Top:
					ret.mTileIndex.X = 0;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Bottom:
					ret.mTileIndex.X = 2;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Left:
					ret.mTileIndex.X = 3;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Right:
					ret.mTileIndex.X = 1;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.TopBottom:
					ret.mTileIndex.X = 5;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.TopLeft:
					ret.mTileIndex.X = 0;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.TopRight:
					ret.mTileIndex.X = 1;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.TopBottomLeft:
					ret.mTileIndex.X = 5;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.TopBottomRight:
					ret.mTileIndex.X = 7;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.TopLeftRight:
					ret.mTileIndex.X = 6;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.BottomRight:
					ret.mTileIndex.X = 2;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.BottomLeft:
					ret.mTileIndex.X = 3;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.BottomLeftRight:
					ret.mTileIndex.X = 4;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.LeftRight:
					ret.mTileIndex.X = 4;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.All:
					ret.mTileIndex.X = 6;
					ret.mTileIndex.Y = 1;
					break;
			}

			return ret;
		}



		/// <summary>
		/// Used for textures that only have 5 tile types
		/// The rest are generated from rotations
		/// </summary>
		/// <param name="adjacency">Adjacency type</param>
		private static TileTexDrawInfo SetupTileWithRotation(AdjacencyType adjacency)
		{
			const float PI2 = MathHelper.PiOver2;
			const float PI = MathHelper.Pi;
			const float PI32 = MathHelper.Pi * 1.5f;

			TileTexDrawInfo ret = new TileTexDrawInfo();

			switch (adjacency)
			{
				case AdjacencyType.None:
					ret.mTileIndex.X = 0;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.Top:
					ret.mTileIndex.X = 1;
					ret.mRotation = PI32;
					break;
				case AdjacencyType.Bottom:
					ret.mTileIndex.X = 1;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.Left:
					ret.mTileIndex.X = 1;
					ret.mRotation = PI;
					break;
				case AdjacencyType.Right:
					ret.mTileIndex.X = 1;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.TopBottom:
					ret.mTileIndex.X = 2;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.TopLeft:
					ret.mTileIndex.X = 5;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.TopRight:
					ret.mTileIndex.X = 5;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.TopBottomLeft:
					ret.mTileIndex.X = 3;
					ret.mRotation = PI32;
					break;
				case AdjacencyType.TopBottomRight:
					ret.mTileIndex.X = 3;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.TopLeftRight:
					ret.mTileIndex.X = 3;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.BottomRight:
					ret.mTileIndex.X = 5;
					ret.mRotation = PI;
					break;
				case AdjacencyType.BottomLeft:
					ret.mTileIndex.X = 5;
					ret.mRotation = PI32;
					break;
				case AdjacencyType.BottomLeftRight:
					ret.mTileIndex.X = 3;
					ret.mRotation = PI;
					break;
				case AdjacencyType.LeftRight:
					ret.mTileIndex.X = 2;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.All:
					ret.mTileIndex.X = 4;
					ret.mRotation = 0.0f;
					break;
			}

			return ret;
		}
	}
}
