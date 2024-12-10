namespace AridArnold
{
	/// <summary>
	/// Represents a tile in the game world.
	/// </summary>
	abstract class Tile
	{
		#region rConstants

		public static float sTILE_SIZE;

		#endregion rConstants





		#region rMembers

		protected Texture2D mTexture = null;
		protected bool mEnabled = true;
		protected AdjacencyType mAdjacency = AdjacencyType.Ad0;
		protected Vector2 mPosition;
		protected Point mTileMapIndex;
		protected CardinalDirection mRotation;
		protected Vector2 mDrawOffset;

		private Rect2f? mBoundsCache;

		#endregion rMembers





		#region rInitialisation 

		/// <summary>
		/// Position
		/// </summary>
		/// <param name="position">mPosition</param>
		public Tile(Vector2 position)
		{
			mRotation = CardinalDirection.Up;
			mPosition = position;
			mBoundsCache = null;

			mDrawOffset = Vector2.Zero;

			mTileMapIndex = TileManager.I.GetTileMapCoord(position);
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public virtual void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Blank");
		}


		/// <summary>
		/// Called once all initialisation has been completed.
		/// </summary>
		public virtual void FinishInit()
		{

		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update the tile
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public virtual void Update(GameTime gameTime) { }

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture for this tile
		/// </summary>
		/// <returns>Texture reference</returns>
		public virtual Texture2D GetTexture()
		{
			return mTexture;
		}



		/// <summary>
		/// Sprite effects can mirror or flip a tile when drawing.
		/// </summary>
		/// <returns>Sprite effect</returns>
		public virtual SpriteEffects GetEffect()
		{
			// Flipping the tile keeps light direction consistent
			switch (mRotation)
			{
				case CardinalDirection.Down:
				case CardinalDirection.Left:
					return SpriteEffects.FlipHorizontally;
				default:
					break;
			}
			return SpriteEffects.None;
		}

		public virtual void DrawExtra(DrawInfo info)
		{
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get rotation of tile. E.g. platforms can be rotated
		/// </summary>
		/// <returns>Rotation in radians</returns>
		public virtual float GetRotation()
		{
			return Util.GetRotation(mRotation);
		}



		/// <summary>
		/// Get type of tile adjacency.
		/// </summary>
		/// <returns>Tile adjacency</returns>
		public AdjacencyType GetAdjacency()
		{
			return mAdjacency;
		}



		/// <summary>
		/// Inform that a neighbour is to the right of this tile
		/// </summary>
		/// <param name="neighbour">Tile that is adjacent to us</param>
		public void SetRightAdjacent(Tile neighbour)
		{
			//My neighbour is to the right of me
			mAdjacency |= AdjacencyType.Ad6;

			//I'm to the left of my neighbour
			neighbour.mAdjacency |= AdjacencyType.Ad4;
		}



		/// <summary>
		/// Inform that a neighbour is to the right of this tile
		/// </summary>
		/// <param name="neighbour">Tile that is adjacent to us</param>
		public void SetTopRightAdjacent(Tile neighbour)
		{
			//My neighbour is to the top right of me
			mAdjacency |= AdjacencyType.Ad9;

			//I'm to the bottom left of my neighbour
			neighbour.mAdjacency |= AdjacencyType.Ad1;
		}



		/// <summary>
		/// Inform that a neighbour is blow this tile 
		/// </summary>
		/// <param name="neighbour">Tile that is adjacent to us</param>
		public void SetBottomAdjacent(Tile neighbour)
		{
			//My neighbour is under me
			mAdjacency |= AdjacencyType.Ad2;

			//I'm above my neighbour
			neighbour.mAdjacency |= AdjacencyType.Ad8;
		}



		/// <summary>
		/// Inform that a neighbour is blow this tile 
		/// </summary>
		/// <param name="neighbour">Tile that is adjacent to us</param>
		public void SetBottomRightAdjacent(Tile neighbour)
		{
			//My neighbour is bottom right of me
			mAdjacency |= AdjacencyType.Ad3;

			//I'm to the top left of my neighbour 
			neighbour.mAdjacency |= AdjacencyType.Ad7;
		}



		/// <summary>
		/// How many neighbours do we have?
		/// </summary>
		public int GetNumDirectlyAdjacenct()
		{
			return (int)MonoMath.BitCountI32((uint)AdjacencyHelper.GetDirectlyAdjacent(mAdjacency));
		}



		/// <summary>
		/// Is the other tile a type capable of being a neighbour.
		/// </summary>
		public virtual bool IsNeighbourType(Tile other)
		{
			return other.GetType() == GetType();
		}



		/// <summary>
		/// Get the center of the tile
		/// </summary>
		public Vector2 GetCentre()
		{
			return mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE) * 0.5f;
		}



		/// <summary>
		/// Gets the index of the tile.
		/// </summary>
		public Point GetIndex()
		{
			return mTileMapIndex;
		}




		/// <summary>
		/// Get draw offset
		/// </summary>
		public Vector2 GetDrawOffset()
		{
			return mDrawOffset;
		}



		/// <summary>
		/// Is this tile enabled?
		/// </summary>
		public bool pEnabled
		{
			get { return mEnabled; }
			set { mEnabled = value; }
		}

		#endregion rUtility





		#region rCollision

		/// <summary>
		/// Resolve collision with an entity
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="gameTime">Frame time</param>
		/// <returns></returns>
		public abstract CollisionResults Collide(MovingEntity entity, GameTime gameTime);



		/// <summary>
		/// Called when an entity touches us. E.g. Arnold is standing on a tile
		/// </summary>
		/// <param name="entity">Entity that touched us</param>
		public virtual void OnTouch(MovingEntity entity, CollisionResults collisionResults) { }



		/// <summary>
		/// Called when an entity intersects us. E.g. Arnold passes through a water bottle
		/// </summary>
		/// <param name="entity">Entity that intersected us</param>
		public virtual void OnEntityIntersect(Entity entity) { }



		/// <summary>
		/// Get bounds of this tile.
		/// </summary>
		/// <returns>Collision rectangle</returns>
		public Rect2f GetBounds()
		{
			if (mBoundsCache == null)
			{
				mBoundsCache = CalculateBounds();
			}

			return mBoundsCache.Value;
		}



		/// <summary>
		/// Get bounds of this tile.
		/// </summary>
		/// <returns>Collision rectangle</returns>
		protected virtual Rect2f CalculateBounds()
		{
			return new Rect2f(mPosition, mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE));
		}



		/// <summary>
		/// Is this tile solid?
		/// </summary>
		/// <returns>True if a tile is solid</returns>
		public virtual bool IsSolid()
		{
			return false;
		}

		#endregion rCollision

	}
}
