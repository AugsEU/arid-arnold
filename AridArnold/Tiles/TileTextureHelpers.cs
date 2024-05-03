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
			else if (tileTexture.Width == tileSize && tileTexture.Height == 47 * tileSize)
			{
				returnInfo = SetupTileForBorderFill(tile.GetAdjacency());
			}
			else if (tileTexture.Width == tileSize && tileTexture.Height == 2 * tileSize)
			{
				returnInfo = SetupTileForUpDown(tile.GetAdjacency());
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

			switch (AdjacencyHelper.GetDirectlyAdjacent(adjacency))
			{
				case AdjacencyType.Ad0:
					ret.mTileIndex.X = 7;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad8:
					ret.mTileIndex.X = 0;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad2:
					ret.mTileIndex.X = 2;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad4:
					ret.mTileIndex.X = 3;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad6:
					ret.mTileIndex.X = 1;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad28:
					ret.mTileIndex.X = 5;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad48:
					ret.mTileIndex.X = 0;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad68:
					ret.mTileIndex.X = 1;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad248:
					ret.mTileIndex.X = 5;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad268:
					ret.mTileIndex.X = 7;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad468:
					ret.mTileIndex.X = 6;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad26:
					ret.mTileIndex.X = 2;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad24:
					ret.mTileIndex.X = 3;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad246:
					ret.mTileIndex.X = 4;
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad46:
					ret.mTileIndex.X = 4;
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad2468:
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

			switch (AdjacencyHelper.GetDirectlyAdjacent(adjacency))
			{
				case AdjacencyType.Ad0:
					ret.mTileIndex.X = 0;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.Ad8:
					ret.mTileIndex.X = 1;
					ret.mRotation = PI32;
					break;
				case AdjacencyType.Ad2:
					ret.mTileIndex.X = 1;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.Ad4:
					ret.mTileIndex.X = 1;
					ret.mRotation = PI;
					break;
				case AdjacencyType.Ad6:
					ret.mTileIndex.X = 1;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.Ad28:
					ret.mTileIndex.X = 2;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.Ad48:
					ret.mTileIndex.X = 5;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.Ad68:
					ret.mTileIndex.X = 5;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.Ad248:
					ret.mTileIndex.X = 3;
					ret.mRotation = PI32;
					break;
				case AdjacencyType.Ad268:
					ret.mTileIndex.X = 3;
					ret.mRotation = PI2;
					break;
				case AdjacencyType.Ad468:
					ret.mTileIndex.X = 3;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.Ad26:
					ret.mTileIndex.X = 5;
					ret.mRotation = PI;
					break;
				case AdjacencyType.Ad24:
					ret.mTileIndex.X = 5;
					ret.mRotation = PI32;
					break;
				case AdjacencyType.Ad246:
					ret.mTileIndex.X = 3;
					ret.mRotation = PI;
					break;
				case AdjacencyType.Ad46:
					ret.mTileIndex.X = 2;
					ret.mRotation = 0.0f;
					break;
				case AdjacencyType.Ad2468:
					ret.mTileIndex.X = 4;
					ret.mRotation = 0.0f;
					break;
			}

			return ret;
		}




		/// <summary>
		/// Sets up tile that are of the "border + fill" type
		/// </summary>
		/// <param name="adjacency">Adjacency type</param>
		private static TileTexDrawInfo SetupTileForBorderFill(AdjacencyType adjacency)
		{
			TileTexDrawInfo ret = new TileTexDrawInfo();

			AdjacencyType essentialAdjacency = AdjacencyHelper.RemoveRedundantTiles(adjacency);
			switch (essentialAdjacency)
			{
				case AdjacencyType.Ad12346789:
					ret.mTileIndex.Y = 0;
					break;
				case AdjacencyType.Ad1234689:
					ret.mTileIndex.Y = 1;
					break;
				case AdjacencyType.Ad1234678:
					ret.mTileIndex.Y = 2;
					break;
				case AdjacencyType.Ad1246789:
					ret.mTileIndex.Y = 3;
					break;
				case AdjacencyType.Ad2346789:
					ret.mTileIndex.Y = 4;
					break;
				case AdjacencyType.Ad123468:
					ret.mTileIndex.Y = 5;
					break;
				case AdjacencyType.Ad124689:
					ret.mTileIndex.Y = 6;
					break;
				case AdjacencyType.Ad234689:
					ret.mTileIndex.Y = 7;
					break;
				case AdjacencyType.Ad124678:
					ret.mTileIndex.Y = 8;
					break;
				case AdjacencyType.Ad234678:
					ret.mTileIndex.Y = 9;
					break;
				case AdjacencyType.Ad246789:
					ret.mTileIndex.Y = 10;
					break;
				case AdjacencyType.Ad12468:
					ret.mTileIndex.Y = 11;
					break;
				case AdjacencyType.Ad23468:
					ret.mTileIndex.Y = 12;
					break;
				case AdjacencyType.Ad24678:
					ret.mTileIndex.Y = 13;
					break;
				case AdjacencyType.Ad24689:
					ret.mTileIndex.Y = 14;
					break;
				case AdjacencyType.Ad2468:
					ret.mTileIndex.Y = 15;
					break;
				case AdjacencyType.Ad12346:
					ret.mTileIndex.Y = 16;
					break;
				case AdjacencyType.Ad1246:
					ret.mTileIndex.Y = 17;
					break;
				case AdjacencyType.Ad2346:
					ret.mTileIndex.Y = 18;
					break;
				case AdjacencyType.Ad246:
					ret.mTileIndex.Y = 19;
					break;
				case AdjacencyType.Ad12478:
					ret.mTileIndex.Y = 20;
					break;
				case AdjacencyType.Ad2478:
					ret.mTileIndex.Y = 21;
					break;
				case AdjacencyType.Ad1248:
					ret.mTileIndex.Y = 22;
					break;
				case AdjacencyType.Ad248:
					ret.mTileIndex.Y = 23;
					break;
				case AdjacencyType.Ad46789:
					ret.mTileIndex.Y = 24;
					break;
				case AdjacencyType.Ad4689:
					ret.mTileIndex.Y = 25;
					break;
				case AdjacencyType.Ad4678:
					ret.mTileIndex.Y = 26;
					break;
				case AdjacencyType.Ad468:
					ret.mTileIndex.Y = 27;
					break;
				case AdjacencyType.Ad23689:
					ret.mTileIndex.Y = 28;
					break;
				case AdjacencyType.Ad2368:
					ret.mTileIndex.Y = 29;
					break;
				case AdjacencyType.Ad2689:
					ret.mTileIndex.Y = 30;
					break;
				case AdjacencyType.Ad268:
					ret.mTileIndex.Y = 31;
					break;
				case AdjacencyType.Ad28:
					ret.mTileIndex.Y = 32;
					break;
				case AdjacencyType.Ad46:
					ret.mTileIndex.Y = 33;
					break;
				case AdjacencyType.Ad124:
					ret.mTileIndex.Y = 34;
					break;
				case AdjacencyType.Ad24:
					ret.mTileIndex.Y = 35;
					break;
				case AdjacencyType.Ad478:
					ret.mTileIndex.Y = 36;
					break;
				case AdjacencyType.Ad48:
					ret.mTileIndex.Y = 37;
					break;
				case AdjacencyType.Ad689:
					ret.mTileIndex.Y = 38;
					break;
				case AdjacencyType.Ad68:
					ret.mTileIndex.Y = 39;
					break;
				case AdjacencyType.Ad236:
					ret.mTileIndex.Y = 40;
					break;
				case AdjacencyType.Ad26:
					ret.mTileIndex.Y = 41;
					break;
				case AdjacencyType.Ad2:
					ret.mTileIndex.Y = 42;
					break;
				case AdjacencyType.Ad4:
					ret.mTileIndex.Y = 43;
					break;
				case AdjacencyType.Ad8:
					ret.mTileIndex.Y = 44;
					break;
				case AdjacencyType.Ad6:
					ret.mTileIndex.Y = 45;
					break;
				case AdjacencyType.Ad0:
					ret.mTileIndex.Y = 46;
					break;
				default:
					throw new Exception("This tile type doesn't work!");
			}

			return ret;
		}


		/// <summary>
		/// Setup basic up/down tile
		/// </summary>
		private static TileTexDrawInfo SetupTileForUpDown(AdjacencyType adjacency)
		{
			TileTexDrawInfo ret = new TileTexDrawInfo();

			if(adjacency.HasFlag(AdjacencyType.Ad8))
			{
				ret.mTileIndex.Y = 1;
			}

			return ret;
		}
	}
}
