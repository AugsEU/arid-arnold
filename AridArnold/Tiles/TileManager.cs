namespace AridArnold
{
	/// <summary>
	/// Comparison class to sort TileCollision results by their closeness to the entity
	/// </summary>
	class TileCollisionResultsSorter : IComparer<TileCollisionResults>
	{
		public int Compare(TileCollisionResults a, TileCollisionResults b)
		{
			return a.result.t.Value.CompareTo(b.result.t.Value);
		}
	}





	/// <summary>
	/// Represents a collision point with a tile
	/// </summary>
	class TileCollisionResults
	{
		public TileCollisionResults(Point p, CollisionResults r)
		{
			coord = p;
			result = r;
		}

		public Point coord;
		public CollisionResults result;
	}





	/// <summary>
	/// Manages and stores the tile map.
	/// </summary>
	internal class TileManager : Singleton<TileManager>
	{
		#region rMembers

		Vector2 mTileMapPos;
		float mTileSize;

		Tile[,] mTileMap = new Tile[32, 32];
		Tile mDummyTile;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Initialise the tilemap at a point in space
		/// </summary>
		/// <param name="position">Top left corner of the tile map</param>
		/// <param name="tileSize">Side length of each tile, which are squares</param>
		public void Init(Vector2 position, float tileSize)
		{
			mTileMapPos = position;
			mTileSize = tileSize;
			mDummyTile = new AirTile();
		}



		/// <summary>
		/// Load a level from a name.
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		/// <param name="name">Name of the level</param>
		public void LoadLevel(ContentManager content, string name)
		{
			EntityManager.I.ClearEntities();
			CollectibleManager.I.ClearAllCollectibles();

			Texture2D tileTexture = content.Load<Texture2D>(name);

			mTileMap = new Tile[tileTexture.Width, tileTexture.Height];

			Color[] colors1D = new Color[tileTexture.Width * tileTexture.Height];
			tileTexture.GetData<Color>(colors1D);

			for (int x = 0; x < tileTexture.Width; x++)
			{
				for (int y = 0; y < tileTexture.Height; y++)
				{
					int index = x + y * tileTexture.Width;
					Color col = colors1D[index];

					mTileMap[x, y] = GetTileFromColour(colors1D[index]);
					mTileMap[x, y].LoadContent(content);

					Vector2 entityPos = new Vector2(x * mTileSize, y * mTileSize) + mTileMapPos;
					AddEntityFromColour(col, entityPos, content);
				}
			}

			CalculateTileAdjacency();
		}



		/// <summary>
		/// Translate from a colour to a tile instance.
		/// Creates a new instance of that tile.
		/// </summary>
		/// <param name="col">Colour to translate</param>
		/// <returns>Tile reference</returns>
		private Tile GetTileFromColour(Color col)
		{
			//Use alpha component as a parameter.
			int param = 255 - col.A;

			if (col.A > 0)
			{
				if (Util.CompareHEX(col, 0x000000))
				{
					return new WallTile();
				}
				else if (Util.CompareHEX(col, 0xA9A9A9))
				{
					return new PlatformTile((CardinalDirection)param);
				}
				else if (Util.CompareHEX(col, 0x0000FF))
				{
					return new WaterBottleTile();
				}
				else if (Util.CompareHEX(col, 0xFF0000))
				{
					return new FlagTile();
				}
				else if (Util.CompareHEX(col, 0xEA301F))
				{
					return new HotDogTile();
				}
				else if (Util.CompareHEX(col, 0x404040))
				{
					return new SpikesTile((CardinalDirection)param);
				}
				else if (Util.CompareHEX(col, 0x2A3F50))
				{
					return new StalactiteTile();
				}
				else if (Util.CompareHEX(col, 0xFFFF00))
				{
					return new MirrorTile((CardinalDirection)param);
				}
				else if (Util.CompareHEX(col, 0x00CDF9))
				{
					return new MushroomTile((CardinalDirection)param);
				}
			}

			return new AirTile();
		}



		/// <summary>
		/// Translate from a colour to an entity
		/// Creates a new instance of the entity
		/// </summary>
		/// <param name="col">Colour you want to translate</param>
		/// <param name="pos">Starting position of entity</param>
		/// <param name="content">Monogame content manager</param>
		private void AddEntityFromColour(Color col, Vector2 pos, ContentManager content)
		{
			if (Util.CompareHEX(col, 0xDC143C))
			{
				EntityManager.I.RegisterEntity(new Arnold(pos), content);
			}
			else if (Util.CompareHEX(col, 0x5B2C2C))
			{
				EntityManager.I.RegisterEntity(new Trundle(pos), content);
			}
		}



		/// <summary>
		/// Tell which tiles are adjacent. Used for textures.
		/// </summary>
		private void CalculateTileAdjacency()
		{
			for (int x = 0; x < mTileMap.GetLength(0); x++)
			{
				for (int y = 0; y < mTileMap.GetLength(1); y++)
				{
					if (x + 1 < mTileMap.GetLength(0))
					{
						Type type1 = mTileMap[x, y].GetType();
						Type type2 = mTileMap[x + 1, y].GetType();

						if (mTileMap[x, y].GetType() == mTileMap[x + 1, y].GetType())
						{
							mTileMap[x, y].SetRightAdjacent(mTileMap[x + 1, y]);
						}
					}

					if (y + 1 < mTileMap.GetLength(1))
					{
						if (mTileMap[x, y].GetType() == mTileMap[x, y + 1].GetType())
						{
							mTileMap[x, y].SetBottomAdjacent(mTileMap[x, y + 1]);
						}
					}
				}
			}
		}

		#endregion rInitialisation





		#region rUtility

		/// <summary>
		/// Get tile at a world position
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Tile GetTile(Vector2 pos)
		{
			return GetTile(GetTileMapCoord(pos));
		}



		/// <summary>
		/// Get a tile from a coordinate in the tile map.
		/// </summary>
		/// <param name="coord">Coordinate of tile you want.</param>
		/// <returns>Tile reference</returns>
		public Tile GetTile(Point coord)
		{
			return GetTile(coord.X, coord.Y);
		}



		/// <summary>
		/// Get a tile from a coordinate of the tile map
		/// </summary>
		/// <param name="x">Tile x-coordinate</param>
		/// <param name="y">Tile y-coordinate</param>
		/// <returns>Tile reference</returns>
		public Tile GetTile(int x, int y)
		{
			if (0 <= x && x < mTileMap.GetLength(0) &&
				0 <= y && y < mTileMap.GetLength(1))
			{
				return mTileMap[x, y];
			}

			return mDummyTile;
		}



		/// <summary>
		/// Get a tile at a world position with an offset in tiles
		/// </summary>
		/// <param name="pos">World position</param>
		/// <param name="displacement">Number of tiles to offset by</param>
		/// <returns>Tile reference</returns>
		public Tile GetRelativeTile(Vector2 pos, Point displacement)
		{
			return GetRelativeTile(pos, displacement.X, displacement.Y);
		}



		/// <summary>
		/// Get a tile at a world position with an offset in tiles
		/// </summary>
		/// <param name="pos">World position</param>
		/// <param name="dx">Number of horizontal tiles to offset</param>
		/// <param name="dy">Number of vertical tiles to offset</param>
		/// <returns></returns>
		public Tile GetRelativeTile(Vector2 pos, int dx, int dy)
		{
			Point coord = GetTileMapCoord(pos);

			return GetTile(coord.X + dx, coord.Y + dy);
		}



		/// <summary>
		/// Get an N by N square of tiles
		/// </summary>
		/// <param name="pos">World space position of the middle of the square</param>
		/// <param name="n">Side length(in tiles) of tile squares</param>
		/// <returns>Rectangle of indices to tiles</returns>
		public Rectangle GetNbyN(Vector2 pos, int n)
		{
			return GetNbyM(pos, n, n);
		}



		/// <summary>
		/// Get an N by M rectangle of tiles
		/// </summary>
		/// <param name="pos">World space position of the middle of the rectangle</param>
		/// <param name="n">Width(in tiles) of tile rectangle</param>
		/// <param name="m">Height(in tiles) of tile rectangle</param>
		/// <returns>Rectangle of indices to tiles</returns>
		public Rectangle GetNbyM(Vector2 pos, int n, int m)
		{
			Vector2 tileSpacePos = (pos - mTileMapPos) / mTileSize;

			Point point = new Point((int)tileSpacePos.X - (n - 1) / 2, (int)tileSpacePos.Y - (m - 1) / 2);
			Point size = new Point(n, m);

			if (point.X + size.X > mTileMap.GetLength(0))
			{
				size.X = mTileMap.GetLength(0) - point.X;
			}

			if (point.Y + size.Y > mTileMap.GetLength(1))
			{
				size.Y = mTileMap.GetLength(1) - point.Y;
			}

			return new Rectangle(point, size);
		}



		/// <summary>
		/// Convert world space position to tile map
		/// </summary>
		/// <param name="pos">World space position</param>
		/// <returns>Tile map index. Note that this may be out of bounds</returns>
		public Point GetTileMapCoord(Vector2 pos)
		{
			pos = pos - mTileMapPos;
			pos = pos / mTileSize;

			return new Point((int)Math.Floor(pos.X), (int)MathF.Floor(pos.Y));
		}



		/// <summary>
		/// Round a position to the centre of a tile
		/// </summary>
		/// <param name="pos">World space position</param>
		/// <returns>World space position that is in the middle of a tile</returns>
		public Vector2 RoundToTileCentre(Vector2 pos)
		{
			pos = pos - mTileMapPos;
			pos = pos / mTileSize;

			pos.X = (int)Math.Floor(pos.X) + 0.5f;
			pos.Y = (int)Math.Floor(pos.Y) + 0.5f;

			pos = pos * mTileSize;
			pos = pos + mTileMapPos;

			return pos;
		}


		/// <summary>
		/// Get pixel width drawn
		/// </summary>
		/// <returns>Width of tile map in pixels</returns>
		public int GetDrawWidth()
		{
			return (int)(mTileSize * mTileMap.GetLength(0));
		}



		/// <summary>
		/// Get pixel height drawn
		/// </summary>
		/// <returns>Width of tile mpa in pixels</returns>
		public int GetDrawHeight()
		{
			return (int)(mTileSize * mTileMap.GetLength(1));
		}



		/// <summary>
		/// Get world-space rectangle of the tile map.
		/// </summary>
		/// <returns>World-space rectangle of tile map</returns>
		public Rect2f GetTileMapRectangle()
		{
			return new Rect2f(mTileMapPos, mTileMapPos + new Vector2(mTileSize * mTileMap.GetLength(0), mTileSize * mTileMap.GetLength(1)));
		}



		/// <summary>
		/// Get side length of the tiles
		/// </summary>
		/// <returns>Side length of the tiles</returns>
		public float GetTileSize()
		{
			return mTileSize;
		}

		#endregion rUtility





		#region rUpdate

		/// <summary>
		/// Update tiles
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			Vector2 offset = new Vector2(mTileSize, mTileSize);
			for (int x = 0; x < mTileMap.GetLength(0); x++)
			{
				for (int y = 0; y < mTileMap.GetLength(1); y++)
				{
					Vector2 newMin = new Vector2(mTileSize * x, mTileSize * y);
					Rect2f tileRect = new Rect2f(newMin, newMin + offset);

					mTileMap[x, y].Update(gameTime, tileRect);
				}
			}
		}



		/// <summary>
		/// Check if this entity has touched any tiles. Call tile if they have.
		/// </summary>
		/// <param name="entity">Entity to check</param>
		public void EntityTouchTiles(Entity entity)
		{
			Rectangle tileBounds = PossibleIntersectTiles(entity.ColliderBounds());

			for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
			{
				for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
				{
					Vector2 tileTopLeft = mTileMapPos + new Vector2(x, y) * mTileSize;

					if (mTileMap[x, y].pEnabled && Collision2D.BoxVsBox(mTileMap[x, y].GetBounds(tileTopLeft, mTileSize), entity.ColliderBounds()))
					{
						Rect2f tileCollideBounds = mTileMap[x, y].GetBounds(tileTopLeft, mTileSize);
						mTileMap[x, y].OnEntityIntersect(entity, tileCollideBounds);
					}
				}
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the tile map
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public void Draw(DrawInfo info)
		{
			Point offset = new Point((int)mTileMapPos.X, (int)mTileMapPos.Y);

			for (int x = 0; x < mTileMap.GetLength(0); x++)
			{
				for (int y = 0; y < mTileMap.GetLength(1); y++)
				{
					if (mTileMap[x, y].pEnabled)
					{
						Rectangle drawRectangle = new Rectangle(offset.X + x * (int)mTileSize, offset.Y + y * (int)mTileSize, (int)mTileSize, (int)mTileSize);

						DrawTile(info, drawRectangle, mTileMap[x, y]);
					}
				}
			}
		}

		/// <summary>
		/// Draw a single tile
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <param name="drawDestination">Screen space of where to draw the tile</param>
		/// <param name="tile">Reference of tile to draw</param>
		/// <exception cref="Exception">Only textures of the correct dimensions can be used</exception>
		public void DrawTile(DrawInfo info, Rectangle drawDestination, Tile tile)
		{
			Texture2D tileTexture = tile.GetTexture();

			int tileHeight = tileTexture.Height;

			//Tile index that we will pick from the texture.
			Point tileIndex = new Point(0, 0);

			//Rotation amount so we can fit tiles together. Should be multiples of 90.
			float rotation = 0.0f;

			SpriteEffects effect = SpriteEffects.None;

			//Square texture, draw as is.
			if (tileTexture.Width == tileTexture.Height)
			{
				//Square textures can be rotated freely.
				//Others can't since they need ot be rotated to fit together.
				rotation = tile.GetRotation();
				effect = tile.GetEffect();
			}
			//Otherwise, look for texture with different edge types
			else if (tileTexture.Width == 6 * tileTexture.Height) //Needs rotating
			{
				SetupTileWithRotation(tile.GetAdjacency(), ref rotation, ref tileIndex);
			}
			else if (tileTexture.Width == 4 * tileTexture.Height)
			{
				tileHeight = tileHeight / 2;
				SetupTileNoRotation(tile.GetAdjacency(), ref tileIndex);
			}
			//What is this?
			else
			{
				throw new Exception("Unhandled texture dimensions");
			}


			if (tileHeight % drawDestination.Height != 0)
			{
				throw new Exception("Tile size doesn't match tile map, stretching may be happening. Must be an integer multiple.");
			}

			Rectangle sourceRectangle = new Rectangle(tileIndex.X * tileHeight, tileIndex.Y * tileHeight, tileHeight, tileHeight);

			info.spriteBatch.Draw(tileTexture, drawDestination, sourceRectangle, Color.White, rotation, Util.CalcRotationOffset(rotation, tileHeight), effect, 1.0f);
		}



		/// <summary>
		/// Used for textures that have all 16 tile types
		/// </summary>
		/// <param name="adjacency">Adjacency type</param>
		/// <param name="tileIndex">Output variable of which sub-section of the texture to use</param>
		private void SetupTileNoRotation(AdjacencyType adjacency, ref Point tileIndex)
		{
			switch (adjacency)
			{
				case AdjacencyType.None:
					tileIndex.X = 7;
					tileIndex.Y = 1;
					break;
				case AdjacencyType.Top:
					tileIndex.X = 0;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.Bottom:
					tileIndex.X = 2;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.Left:
					tileIndex.X = 3;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.Right:
					tileIndex.X = 1;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.TopBottom:
					tileIndex.X = 5;
					tileIndex.Y = 1;
					break;
				case AdjacencyType.TopLeft:
					tileIndex.X = 0;
					tileIndex.Y = 1;
					break;
				case AdjacencyType.TopRight:
					tileIndex.X = 1;
					tileIndex.Y = 1;
					break;
				case AdjacencyType.TopBottomLeft:
					tileIndex.X = 5;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.TopBottomRight:
					tileIndex.X = 7;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.TopLeftRight:
					tileIndex.X = 6;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.BottomRight:
					tileIndex.X = 2;
					tileIndex.Y = 1;
					break;
				case AdjacencyType.BottomLeft:
					tileIndex.X = 3;
					tileIndex.Y = 1;
					break;
				case AdjacencyType.BottomLeftRight:
					tileIndex.X = 4;
					tileIndex.Y = 0;
					break;
				case AdjacencyType.LeftRight:
					tileIndex.X = 4;
					tileIndex.Y = 1;
					break;
				case AdjacencyType.All:
					tileIndex.X = 6;
					tileIndex.Y = 1;
					break;
			}
		}



		/// <summary>
		/// Used for textures that only have 5 tile types
		/// The rest are generated from rotations
		/// </summary>
		/// <param name="adjacency">Adjacency type</param>
		/// <param name="rotation">Output variable of how far to rotate the texture</param>
		/// <param name="tileIndex">Output variable of which sub-section of the texture to use</param>
		private void SetupTileWithRotation(AdjacencyType adjacency, ref float rotation, ref Point tileIndex)
		{
			const float PI2 = MathHelper.PiOver2;
			const float PI = MathHelper.Pi;
			const float PI32 = MathHelper.Pi * 1.5f;

			switch (adjacency)
			{
				case AdjacencyType.None:
					tileIndex.X = 0;
					rotation = 0.0f;
					break;
				case AdjacencyType.Top:
					tileIndex.X = 1;
					rotation = PI32;
					break;
				case AdjacencyType.Bottom:
					tileIndex.X = 1;
					rotation = PI2;
					break;
				case AdjacencyType.Left:
					tileIndex.X = 1;
					rotation = PI;
					break;
				case AdjacencyType.Right:
					tileIndex.X = 1;
					rotation = 0.0f;
					break;
				case AdjacencyType.TopBottom:
					tileIndex.X = 2;
					rotation = PI2;
					break;
				case AdjacencyType.TopLeft:
					tileIndex.X = 5;
					rotation = 0.0f;
					break;
				case AdjacencyType.TopRight:
					tileIndex.X = 5;
					rotation = PI2;
					break;
				case AdjacencyType.TopBottomLeft:
					tileIndex.X = 3;
					rotation = PI32;
					break;
				case AdjacencyType.TopBottomRight:
					tileIndex.X = 3;
					rotation = PI2;
					break;
				case AdjacencyType.TopLeftRight:
					tileIndex.X = 3;
					rotation = 0.0f;
					break;
				case AdjacencyType.BottomRight:
					tileIndex.X = 5;
					rotation = PI;
					break;
				case AdjacencyType.BottomLeft:
					tileIndex.X = 5;
					rotation = PI32;
					break;
				case AdjacencyType.BottomLeftRight:
					tileIndex.X = 3;
					rotation = PI;
					break;
				case AdjacencyType.LeftRight:
					tileIndex.X = 2;
					rotation = 0.0f;
					break;
				case AdjacencyType.All:
					tileIndex.X = 4;
					rotation = 0.0f;
					break;
			}
		}



		/// <summary>
		/// Centre the tile map in the screen.
		/// </summary>
		/// <param name="screenWidth">Width of the screen to centre it</param>
		public void CentreX(float screenWidth)
		{
			float ourWidth = GetDrawWidth();
			float xOffset = (screenWidth - ourWidth) / 2.0f;

			mTileMapPos.X = xOffset;
		}

		#endregion rDraw





		#region rCollisions

		/// <summary>
		/// Resolve all collisions with an entity
		/// </summary>
		/// <param name="entity">Entity to collide</param>
		/// <param name="gameTime">Frame time</param>
		/// <returns>List of all collisions. Note: it may contain dud collisions results. Check the t parameter first</returns>
		public List<TileCollisionResults> ResolveCollisions(MovingEntity entity, GameTime gameTime)
		{
			List<TileCollisionResults> results = new List<TileCollisionResults>();

			Util.Log("==Resolving==");

			Rect2f playerBounds = entity.ColliderBounds();
			Rect2f futurePlayerBounds = entity.ColliderBounds() + entity.VelocityToDisplacement(gameTime);

			Rectangle tileBounds = PossibleIntersectTiles(playerBounds + futurePlayerBounds);

			Util.Log(" Starting vel " + entity.pVelocity.ToString());

			for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
			{
				for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
				{
					if (mTileMap[x, y].pEnabled == false)
					{
						continue;
					}

					Vector2 tileTopLeft = mTileMapPos + new Vector2(x, y) * mTileSize;

					CollisionResults collisionResults = mTileMap[x, y].Collide(entity, tileTopLeft, mTileSize, gameTime);

					if (collisionResults.Collided)
					{
						results.Add(new TileCollisionResults(new Point(x, y), collisionResults));
					}
				}
			}
			results.Sort(new TileCollisionResultsSorter());

			Util.Log(" Resolving all " + results.Count + " collisions");

			for (int i = 0; i < results.Count; i++)
			{
				Point point = results[i].coord;
				Vector2 tileTopLeft = mTileMapPos + new Vector2(point.X, point.Y) * mTileSize;

				Tile tile = mTileMap[point.X, point.Y];

				CollisionResults collisionResults = tile.Collide(entity, tileTopLeft, mTileSize, gameTime);

				if (collisionResults.Collided)
				{
					Vector2 pushVec = collisionResults.normal * new Vector2(Math.Abs(entity.pVelocity.X), Math.Abs(entity.pVelocity.Y)) * (1.0f - collisionResults.t.Value) * 1.02f;

					Util.Log("   " + point.ToString() + "Pushing by normal " + collisionResults.normal.ToString() + "(" + collisionResults.t.Value + ")");

					entity.pVelocity += pushVec;
				}

				results[i].result = collisionResults;
			}

			Util.Log(" Final vel " + entity.pVelocity.X + ", " + entity.pVelocity.Y);

			return results;
		}



		/// <summary>
		/// Find rectangle of tile coordinates that a box will lie in
		/// </summary>
		/// <param name="box">Box to check</param>
		/// <returns>Rectangle of indices to tiles</returns>
		public Rectangle PossibleIntersectTiles(Rect2f box)
		{
			box.min = (box.min - mTileMapPos) / mTileSize;
			box.max = (box.max - mTileMapPos) / mTileSize;

			Point rMin = new Point(Math.Max((int)box.min.X - 1, 0), Math.Max((int)box.min.Y - 1, 0));
			Point rMax = new Point(Math.Min((int)box.max.X + 2, mTileMap.GetLength(0) - 1), Math.Min((int)box.max.Y + 2, mTileMap.GetLength(1) - 1));

			return new Rectangle(rMin, rMax - rMin);
		}



		/// <summary>
		/// Check if a rectangle touches an solid tiles
		/// </summary>
		/// <param name="rect">Rectangle to check</param>
		/// <returns>True if the rectangle touches any tiles marked as solid</returns>
		public bool DoesRectTouchTiles(Rect2f rect)
		{
			Rectangle tileBounds = PossibleIntersectTiles(rect);

			for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
			{
				for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
				{
					Vector2 tileTopLeft = mTileMapPos + new Vector2(x, y) * mTileSize;

					if (mTileMap[x, y].pEnabled && mTileMap[x, y].IsSolid() && Collision2D.BoxVsBox(mTileMap[x, y].GetBounds(tileTopLeft, mTileSize), rect))
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion rCollision
	}
}
