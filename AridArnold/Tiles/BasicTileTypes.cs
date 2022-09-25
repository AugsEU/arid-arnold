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
		#region rMembers

		protected Texture2D mTexture = null;
        protected bool mEnabled = true;
        protected AdjacencyType mAdjacency = AdjacencyType.None;
        protected CardinalDirection mRotation;

        #endregion rMembers





        #region rInitialisation 

        /// <summary>
        /// Tile constructor
        /// </summary>
        public Tile()
        {
            mRotation = CardinalDirection.Up;
        }



        /// <summary>
        /// Load all textures and assets
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public virtual void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/blank");
        }

        #endregion rInitialisation





        #region rUpdate

        /// <summary>
        /// Update the tile
        /// </summary>
        /// <param name="gameTime">Frame time</param>
        /// <param name="bounds">Bounds of the tile. TO DO: Store this in the tile</param>
        public virtual void Update(GameTime gameTime, Rect2f bounds) { }

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
            switch (mRotation)
            {
                case CardinalDirection.Up:
                    return 0.0f;
                case CardinalDirection.Right:
                    return MathHelper.PiOver2;
                case CardinalDirection.Down:
                    return MathHelper.Pi;
                case CardinalDirection.Left:
                    return MathHelper.PiOver2 * 3.0f;
            }

            return 0.0f;
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
        /// <param name="topLeft">Top left position of this tile</param>
        /// <param name="sideLength">Side length of this tile</param>
        /// <param name="gameTime">Frame time</param>
        /// <returns></returns>
        public abstract CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime);



        /// <summary>
        /// Called when an entity touches us. E.g. Arnold is standing on a tile
        /// </summary>
        /// <param name="entity">Entity that touched us</param>
        public virtual void OnTouch(MovingEntity entity) { }



        /// <summary>
        /// Called when an entity intersects us. E.g. Arnold passes through a water bottle
        /// </summary>
        /// <param name="entity">Entity that intersected us</param>
        /// <param name="bounds">Our tile bounds</param>
        public virtual void OnEntityIntersect(Entity entity, Rect2f bounds) { }



        /// <summary>
        /// Get bounds of this tile.
        /// </summary>
        /// <param name="topLeft">Top left position of tile.</param>
        /// <param name="sideLength">Side length of tile</param>
        /// <returns>Collision rectangle</returns>
        public virtual Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
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
        /// <summary>
        /// Resolve collision with an entity
        /// </summary>
        /// <param name="entity">Entity that is colliding with us</param>
        /// <param name="topLeft">Top left position of this tile</param>
        /// <param name="sideLength">Side length of this tile</param>
        /// <param name="gameTime">Frame time</param>
        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), GetBounds(topLeft, sideLength));
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
        /// <param name="topLeft">Top left position of this tile</param>
        /// <param name="sideLength">Side length of this tile</param>
        /// <param name="gameTime">Frame time</param>
        /// <returns></returns>
        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            // No collision.
            return CollisionResults.None;
        }

        public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
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
        public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
        }
    }





    /// <summary>
    /// Tile for solid walls/floors
    /// </summary>
    class WallTile : SquareTile
    {
        /// <summary>
        /// Load all textures and assets
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/" + ProgressManager.I.GetWorldData().wallTexture);
        }
    }
}
