namespace AridArnold
{
	/// <summary>
	/// Bit map enum of all possible direct adjacencies.
	/// </summary>
	enum AdjacencyType
	{
		None = 0b0000,
		Top = 0b0001,
		Bottom = 0b0010,
		Left = 0b0100,
		Right = 0b1000,
		TopBottom = Top | Bottom,
		TopLeft = Top | Left,
		TopRight = Top | Right,
		TopBottomLeft = Top | Bottom | Left,
		TopBottomRight = Top | Bottom | Right,
		TopLeftRight = Top | Left | Right,
		BottomRight = Bottom | Right,
		BottomLeft = Bottom | Left,
		BottomLeftRight = Bottom | Left | Right,
		LeftRight = Left | Right,
		All = Top | Bottom | Left | Right,
	}





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
		protected AdjacencyType mAdjacency = AdjacencyType.None;
		protected Vector2 mPosition;
		protected Point mTileMapIndex;
		protected CardinalDirection mRotation;

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

			mTileMapIndex = TileManager.I.GetTileMapCoord(position);
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public virtual void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/blank");
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
			return SpriteEffects.None;
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get rotation of tile. E.g. platforms can be rotated
		/// </summary>
		/// <returns>Rotation in radians</returns>
		public float GetRotation()
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
			mAdjacency |= AdjacencyType.Right;

			//I'm to the left of my neighbour
			neighbour.mAdjacency |= AdjacencyType.Left;
		}



		/// <summary>
		/// Inform that a neighbour is blow this tile 
		/// </summary>
		/// <param name="neighbour">Tile that is adjacent to us</param>
		public void SetBottomAdjacent(Tile neighbour)
		{
			//My neighbour is under me
			mAdjacency |= AdjacencyType.Bottom;

			//I'm above my neighbour
			neighbour.mAdjacency |= AdjacencyType.Top;
		}



		/// <summary>
		/// How many neighbours do we have?
		/// </summary>
		public int GetNumNeighbours()
		{
			return (int)MonoMath.BitCountI32((UInt32)(mAdjacency));
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





	/// <summary>
	/// Simple tile with a square hitbox. Does nothing else.
	/// </summary>
	abstract class SquareTile : Tile
	{
		public SquareTile(Vector2 position) : base(position)
		{
		}


		/// <summary>
		/// Resolve collision with an entity
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="gameTime">Frame time</param>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), GetBounds());
		}



		/// <summary>
		/// Is this tile solid?
		/// </summary>
		/// <returns>True if a tile is solid</returns>
		public override bool IsSolid()
		{
			return true;
		}
	}





	/// <summary>
	/// A tile that has no texture or collisions.
	/// </summary>
	class AirTile : Tile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public AirTile(Vector2 position) : base(position)
		{
		}

		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/air");
		}

		/// <summary>
		/// Do not collide with the entity.
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="gameTime">Frame time</param>
		/// <returns></returns>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			// No collision.
			return CollisionResults.None;
		}



		/// <summary>
		/// Get empty bounds
		/// </summary>
		/// <returns>Zero bounds</returns>
		protected override Rect2f CalculateBounds()
		{
			return new Rect2f(Vector2.Zero, Vector2.Zero);
		}



		/// <summary>
		/// Is this tile solid?
		/// </summary>
		/// <returns>True if a tile is solid</returns>
		public override bool IsSolid()
		{
			return false;
		}
	}





	/// <summary>
	/// A tile you can interact with. Such as collectables or spikes.
	/// </summary>
	class InteractableTile : AirTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public InteractableTile(Vector2 position) : base(position)
		{
		}


		/// <summary>
		/// Get normal bounds
		/// </summary>
		/// <returns>Square bounds</returns>
		protected override Rect2f CalculateBounds()
		{
			return new Rect2f(mPosition, mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE));
		}
	}

}
