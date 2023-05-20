using AridArnold.Tiles.Basic;

namespace AridArnold
{
    /// <summary>
    /// Manages and stores the tile map.
    /// </summary>
    internal class TileManager : Singleton<TileManager>
	{
		#region rConstants

		const int TILE_MAP_SIZE = 36;

		#endregion rConstants





		#region rMembers

		Vector2 mTileMapPos;
		float mTileSize;

		//Tile map
		Tile[,] mTileMap = new Tile[TILE_MAP_SIZE, TILE_MAP_SIZE];
		Tile mDummyTile;
		List<Point> mDeleteRequests; // Tiles we want deleted in the next update.

		//EM field
		EMField mEMField;

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
			Tile.sTILE_SIZE = tileSize;
			mDummyTile = new AirTile(Vector2.Zero);
			mDeleteRequests = new List<Point>();
		}



		/// <summary>
		/// Load a level from a name.
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		/// <param name="name">Name of the level</param>
		public void LoadLevel(string name)
		{
			mEMField = new EMField(TILE_MAP_SIZE);

			//Load tile map.
			LoadTilemap(name);
		}

		/// <summary>
		/// Load tilemap from image.
		/// </summary>
		public void LoadTilemap(string name)
		{
			Texture2D tileTexture = MonoData.I.MonoGameLoad<Texture2D>(name);

			mTileMap = new Tile[tileTexture.Width, tileTexture.Height];

			Color[] colors1D = new Color[tileTexture.Width * tileTexture.Height];
			tileTexture.GetData<Color>(colors1D);

			for (int x = 0; x < tileTexture.Width; x++)
			{
				for (int y = 0; y < tileTexture.Height; y++)
				{
					int index = x + y * tileTexture.Width;
					Color col = colors1D[index];
					Vector2 tileTopLeft = GetTileTopLeft(new Point(x,y));

					mTileMap[x, y] = GetTileFromColour(colors1D[index], tileTopLeft);
					mTileMap[x, y].LoadContent();

					AddEntityFromColour(col, tileTopLeft);
				}
			}

			CalculateTileAdjacency();

			// Do any last initialisation once everything has been calculated.
			for (int x = 0; x < tileTexture.Width; x++)
			{
				for (int y = 0; y < tileTexture.Height; y++)
				{
					mTileMap[x, y].FinishInit();
				}
			}
		}



		/// <summary>
		/// Translate from a colour to a tile instance.
		/// Creates a new instance of that tile.
		/// </summary>
		/// <param name="col">Colour to translate</param>
		/// <returns>Tile reference</returns>
		private Tile GetTileFromColour(Color col, Vector2 position)
		{
			if (col.A > 0)
			{
				//Use alpha component as a parameter.
				int param = 255 - col.A;
				uint hexCode = MonoColor.ColorToHEX(col);

				switch (hexCode)
				{
					//Basic
					case 0xFFFFFFu:
						return new AirTile(position);
					case 0x000000u:
						return new WallTile(position, "Wall");
					case 0x101010u:
						return new WallTile(position, "WallSecondary");
					case 0x202020u:
						return new OtherWorldTile(position, "OtherWorldWall");
					case 0xA9A9A9u:
						return new AnimatedPlatformTile((CardinalDirection)param, position);
					//Collectable
					case 0x0000FFu:
						return new WaterBottleTile(position);
					case 0xFF0000u:
						return new FlagTile(position);
					case 0xEA301Fu:
						return new HotDogTile(position);
					case 0xE0A021u:
						return new KeyTile(position);
					case 0xFE00FEu:
						return new CoinTile(position);
					//Special
					case 0xA51D18u:
						return new RedLockTile(position);
					case 0x404040u:
						return new SpikesTile((CardinalDirection)param, position);
					case 0xFFFF00u:
						return new MirrorTile((CardinalDirection)param, position);
					case 0x00CDF9u:
						return new MushroomTile((CardinalDirection)param, position);
					case 0x003D36u:
						return new TeleportPipe(position);
					//Electricity
					case 0x61AD65u:
						return new PermElectricButton((CardinalDirection)param, position);
					case 0xFF6C7Cu:
						return new ElectricButton((CardinalDirection)param, position);
					case 0xFFC130u:
						return new ElectricTile(position);
					case 0x0D4C92u:
						return new ElectricGate(position);
					case 0x363636u:
						return new AndroldTile(position);
					//Decoration
					case 0x2A3F50u:
						return new StalactiteTile(position);
					default:
						break;
				}
			}

			return new AirTile(position);
		}



		/// <summary>
		/// Translate from a colour to an entity
		/// Creates a new instance of the entity
		/// </summary>
		/// <param name="col">Colour you want to translate</param>
		/// <param name="pos">Starting position of entity</param>
		/// <param name="content">Monogame content manager</param>
		private void AddEntityFromColour(Color col, Vector2 pos)
		{
			if (col.A > 0)
			{
				uint hexCode = MonoColor.ColorToHEX(col);

				switch (hexCode)
				{
					//Arnold
					case 0xDC143Cu:
						EntityManager.I.RegisterEntity(new Arnold(pos));
						break;
				}
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
						if (mTileMap[x, y].IsNeighbourType(mTileMap[x + 1, y]))
						{
							mTileMap[x, y].SetRightAdjacent(mTileMap[x + 1, y]);
						}

						if(y + 1 < mTileMap.GetLength(1))
						{
							if (mTileMap[x, y].IsNeighbourType(mTileMap[x + 1, y + 1]))
							{
								mTileMap[x, y].SetBottomRightAdjacent(mTileMap[x + 1, y + 1]);
							}
						}

						if(y - 1 > 0)
						{
							if (mTileMap[x, y].IsNeighbourType(mTileMap[x + 1, y - 1]))
							{
								mTileMap[x, y].SetTopRightAdjacent(mTileMap[x + 1, y - 1]);
							}
						}

					}

					if (y + 1 < mTileMap.GetLength(1))
					{
						if (mTileMap[x, y].IsNeighbourType(mTileMap[x, y + 1]))
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

			Point ret = new Point((int)Math.Floor(pos.X), (int)MathF.Floor(pos.Y));

			MonoDebug.Assert(ret.X >= 0 && ret.Y >= 0);
			return ret;
		}


		/// <summary>
		/// Convert index to tile's top left..
		/// </summary>
		public Vector2 GetTileTopLeft(Point index)
		{
			Vector2 result = mTileMapPos;

			result.X += (index.X) * (mTileSize);
			result.Y += (index.Y) * (mTileSize);

			return result;
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
		/// Convert tile index to position.
		/// </summary>
		public Vector2 GetTileCentre(Point index)
		{
			Vector2 result = mTileMapPos;

			result.X += (index.X + 0.5f) * (mTileSize);
			result.Y += (index.Y + 0.5f) * (mTileSize);

			return result;
		}

		/// <summary>
		/// Query if tile at position is solid.
		/// </summary>
		public bool IsTileSolid(Point coord)
		{
			Tile tile = GetTile(coord);

			if (tile is null) return false;


			return tile.IsSolid();
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



		/// <summary>
		/// Get reference to EMField.
		/// </summary>
		public EMField GetEMField()
		{
			return mEMField;
		}



		/// <summary>
		/// Effectively delete a tile by making it an Air tile..
		/// </summary>
		/// <param name="point"></param>
		public void RequestDelete(Point point)
		{
			mDeleteRequests.Add(point);
		}



		/// <summary>
		/// Effectively delete a tile by making it an Air tile..
		/// </summary>
		/// <param name="point"></param>
		private void MakeTileIntoAir(Point point)
		{
			Vector2 pos = new Vector2(point.X * mTileSize, point.Y * mTileSize) + mTileMapPos;
			mTileMap[point.X, point.Y] = new AirTile(pos);
			mTileMap[point.X, point.Y].LoadContent();
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
					mTileMap[x, y].Update(gameTime);
				}
			}

			//Process delete requests
			for(int p = 0; p < mDeleteRequests.Count; p++)
			{
				MakeTileIntoAir(mDeleteRequests[p]);
			}

			mDeleteRequests.Clear();

			mEMField.ProcessUpdate();
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
					if (mTileMap[x, y].pEnabled && Collision2D.BoxVsBox(mTileMap[x, y].GetBounds(), entity.ColliderBounds()))
					{
						mTileMap[x, y].OnEntityIntersect(entity);
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

			TileTexDrawInfo tileDrawInfo = TileTextureHelpers.GetTileDrawInfo(tile);

			int tileHeight = (int)Tile.sTILE_SIZE;
			Rectangle sourceRectangle = new Rectangle(tileDrawInfo.mTileIndex.X * tileHeight, tileDrawInfo.mTileIndex.Y * tileHeight, tileHeight, tileHeight);

			MonoDraw.DrawTexture(info, tileTexture, drawDestination, sourceRectangle, Color.White, tileDrawInfo.mRotation, MonoDraw.CalcRotationOffset(tileDrawInfo.mRotation, tileHeight), tileDrawInfo.mEffect, DrawLayer.Tile);
		}

		#endregion rDraw





		#region rCollisions

		/// <summary>
		/// Resolve all collisions with an entity
		/// </summary>
		/// <param name="entity">Entity to collide</param>
		/// <param name="gameTime">Frame time</param>
		/// <returns>List of all collisions. Note: it may contain dud collisions results. Check the t parameter first</returns>
		public void GatherCollisions(GameTime gameTime, MovingEntity entity, ref List<EntityCollision> outputList)
		{
			Rect2f playerBounds = entity.ColliderBounds();
			Rect2f futurePlayerBounds = entity.ColliderBounds() + entity.VelocityToDisplacement(gameTime);

			Rectangle tileBounds = PossibleIntersectTiles(playerBounds + futurePlayerBounds);


			for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
			{
				for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
				{
					if (mTileMap[x, y].pEnabled == false)
					{
						continue;
					}

					CollisionResults collisionResults = mTileMap[x, y].Collide(entity, gameTime);

					if (collisionResults.Collided)
					{
						outputList.Add(new TileEntityCollision(true, collisionResults, new Point(x, y)));
					}
				}
			}
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
					if (mTileMap[x, y].pEnabled && mTileMap[x, y].IsSolid() && Collision2D.BoxVsBox(mTileMap[x, y].GetBounds(), rect))
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
