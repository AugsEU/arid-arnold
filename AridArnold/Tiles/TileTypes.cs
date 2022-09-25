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
        /// Load a textures
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
        /// <returns></returns>
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

    class AirTile : Tile
    {
        /// <summary>
        /// Load a textures
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

    class InteractableTile : AirTile
    {
        public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
        }
    }

    class WallTile : SquareTile
    {
        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/" + ProgressManager.I.GetWorldData().wallTexture);
        }
    }

    class PlatformTile : Tile
    {
        public PlatformTile(CardinalDirection rotation) : base()
        {
            mRotation = rotation;
        }


        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/" + ProgressManager.I.GetWorldData().platformTexture);
        }



        /// <summary>
        /// Resolve collision with an entity. Note: Some entities can pass through us.
        /// </summary>
        /// <param name="entity">Entity that is colliding with us</param>
        /// <param name="topLeft">Top left position of this tile</param>
        /// <param name="sideLength">Side length of this tile</param>
        /// <param name="gameTime">Frame time</param>
        /// <returns></returns>
        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            if (!entity.CollideWithPlatforms())
            {
                if (entity is PlatformingEntity)
                {
                    PlatformingEntity platformingEntity = (PlatformingEntity)entity;

                    if (mRotation == Util.InvertDirection(platformingEntity.GetGravityDir()))
                    {
                        return CollisionResults.None;
                    }
                }
                else
                {
                    return CollisionResults.None;
                }
            }

            return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), topLeft, sideLength, mRotation);
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

    //============================================
    //  Collectible types
    //--------------------------------------------
    abstract class CollectibleTile : InteractableTile
    {
        protected abstract CollectibleType GetCollectibleType();

        public override void OnEntityIntersect(Entity entity, Rect2f bounds)
        {
            if (entity is Arnold)
            {
                CollectibleManager.I.CollectItem(GetCollectibleType());
                mEnabled = false;
            }
        }
    }

    class FlagTile : CollectibleTile
    {
        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/flag");
        }

        public override void OnEntityIntersect(Entity entity, Rect2f bounds)
        {
            if (entity is Arnold)
            {
                ProgressManager.I.ReportCheckpoint();
            }
            base.OnEntityIntersect(entity, bounds);
        }

        protected override CollectibleType GetCollectibleType()
        {
            return CollectibleType.Flag;
        }
    }

    class HotDogTile : InteractableTile
    {
        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/hotdog");
        }

        public override void OnEntityIntersect(Entity entity, Rect2f bounds)
        {
            if (entity is Arnold)
            {
                int livesBefore = ProgressManager.I.Lives;
                ProgressManager.I.GiveLife();
                mEnabled = false;

                //If there is actually a life increase
                if (livesBefore < ProgressManager.I.Lives)
                {
                    FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.OliveDrab, entity.pPosition, "+1 Life");
                }
                else
                {
                    FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.White, entity.pPosition, "+0 Lives");
                }
            }
            base.OnEntityIntersect(entity, bounds);
        }
    }

    class WaterTile : CollectibleTile
    {
        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/bottle");
        }

        protected override CollectibleType GetCollectibleType()
        {
            return CollectibleType.WaterBottle;
        }
    }

    //============================================
    //  Special types
    //--------------------------------------------
    class SpikesTile : InteractableTile
    {
        public SpikesTile(CardinalDirection rotation) : base()
        {
            mRotation = rotation;
        }

        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/spikes");
        }

        public override void OnEntityIntersect(Entity entity, Rect2f bounds)
        {
            if (entity is Arnold)
            {
                EArgs eArgs;
                eArgs.sender = this;

                EventManager.I.SendEvent(EventType.KillPlayer, eArgs);
            }
        }

        //Make bounds a bit smaller to make it fairer
        public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            const float smallerFactor = 7.0f;

            switch (mRotation)
            {
                case CardinalDirection.Up:
                    topLeft.Y += smallerFactor;
                    topLeft.X += smallerFactor / 2.0f;
                    break;
                case CardinalDirection.Right:
                    topLeft.Y += smallerFactor / 2.0f;
                    break;
                case CardinalDirection.Left:
                    topLeft.X += smallerFactor;
                    topLeft.Y += smallerFactor / 2.0f;
                    break;
                case CardinalDirection.Down:
                    topLeft.X += smallerFactor / 2.0f;
                    break;
            }

            sideLength -= smallerFactor;

            return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
        }
    }

    class MirrorTile : PlatformTile
    {
        public MirrorTile(CardinalDirection rotation) : base(rotation)
        {

        }

        public override void OnEntityIntersect(Entity entity, Rect2f bounds)
        {
            if (entity is PlatformingEntity)
            {
                PlatformingEntity platformingEntity = (PlatformingEntity)entity;

                switch (mRotation)
                {
                    case CardinalDirection.Up:
                        if (platformingEntity.GetGravityDir() != CardinalDirection.Down)
                        {
                            if (platformingEntity.GetGravityDir() == CardinalDirection.Left || platformingEntity.GetGravityDir() == CardinalDirection.Right)
                            {
                                platformingEntity.SetWalkDirection(WalkDirection.None);
                            }
                            platformingEntity.SetGravity(CardinalDirection.Down);
                            platformingEntity.ShiftPosition(new Vector2(0.0f, -(bounds.Height + 16.0f)));
                        }
                        break;
                    case CardinalDirection.Right:
                        if (platformingEntity.GetGravityDir() != CardinalDirection.Left)
                        {
                            if (platformingEntity.GetGravityDir() == CardinalDirection.Up || platformingEntity.GetGravityDir() == CardinalDirection.Down)
                            {
                                platformingEntity.SetWalkDirection(WalkDirection.None);
                            }
                            platformingEntity.SetGravity(CardinalDirection.Left);
                            platformingEntity.ShiftPosition(new Vector2(bounds.Width + 16.0f, 0.0f));
                        }
                        break;
                    case CardinalDirection.Down:
                        if (platformingEntity.GetGravityDir() != CardinalDirection.Up)
                        {
                            if (platformingEntity.GetGravityDir() == CardinalDirection.Left || platformingEntity.GetGravityDir() == CardinalDirection.Right)
                            {
                                platformingEntity.SetWalkDirection(WalkDirection.None);
                            }
                            platformingEntity.SetGravity(CardinalDirection.Up);
                            platformingEntity.ShiftPosition(new Vector2(0.0f, bounds.Height + 16.0f));
                        }
                        break;
                    case CardinalDirection.Left:
                        if (platformingEntity.GetGravityDir() != CardinalDirection.Right)
                        {
                            if (platformingEntity.GetGravityDir() == CardinalDirection.Up || platformingEntity.GetGravityDir() == CardinalDirection.Down)
                            {
                                platformingEntity.SetWalkDirection(WalkDirection.None);
                            }
                            platformingEntity.SetGravity(CardinalDirection.Right);
                            platformingEntity.ShiftPosition(new Vector2(-(bounds.Width + 16.0f), 0.0f));
                        }
                        break;
                }


            }
        }

        /// <summary>
        /// Resolve collision with an entity. Note: Only the top side of the mirror is solid.
        /// </summary>
        /// <param name="entity">Entity that is colliding with us</param>
        /// <param name="topLeft">Top left position of this tile</param>
        /// <param name="sideLength">Side length of this tile</param>
        /// <param name="gameTime">Frame time</param>
        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), topLeft, sideLength, mRotation);
        }

        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/mirror");
        }
    }

    class StalactiteTile : AirTile
    {
        public StalactiteTile() : base()
        {
        }



        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/stalag-tile");
        }
    }

    class MushroomTile : InteractableTile
    {
        Animator mBounceAnim;

        public MushroomTile(CardinalDirection rotation) : base()
        {
            mRotation = rotation;
        }

        /// <summary>
        /// Load a textures
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/mushroom");
            mBounceAnim = new Animator(Animator.PlayType.OneShot);

            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce1", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce2", 0.1f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce1", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce3", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce4", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce3", 0.05f);
        }

        public override void OnEntityIntersect(Entity entity, Rect2f bounds)
        {
            const float alpha = 1.4f;
            const float minVel = 19.5f;
            
            if (entity is PlatformingEntity)
            {
                PlatformingEntity platformingEntity = (PlatformingEntity)entity;
                Rect2f entityBounds = platformingEntity.ColliderBounds();

                bool didBounce = false;

                switch (mRotation)
                {
                    case CardinalDirection.Up:
                        {
                            if (platformingEntity.pGrounded == false)
                            {
                                if (platformingEntity.pVelocity.Y > minVel)
                                {
                                    platformingEntity.pVelocity = new Vector2(platformingEntity.pVelocity.X, -platformingEntity.pVelocity.Y * alpha);
                                    didBounce = true;
                                }
                                else if (platformingEntity.pVelocity.Y > 0.0f)
                                {
                                    platformingEntity.pVelocity = new Vector2(platformingEntity.pVelocity.X, -minVel * alpha);
                                    didBounce = true;
                                }

                                if (didBounce)
                                {
                                    float newY = bounds.min.Y - entityBounds.Height;
                                    platformingEntity.pPosition = new Vector2(platformingEntity.pPosition.X, newY);
                                    platformingEntity.pGrounded = true;
                                }
                            }
                        }
                        break;
                    case CardinalDirection.Left:
                    case CardinalDirection.Right:
                        {
                            bool valid = (CardinalDirection.Left == mRotation) != (platformingEntity.pVelocity.X < 0.0f);

                            if (valid)
                            {
                                if (platformingEntity.pGrounded == false)
                                {
                                    platformingEntity.pVelocity = new Vector2(-platformingEntity.pVelocity.X, platformingEntity.pVelocity.Y);
                                    platformingEntity.ReverseWalkDirection();
                                }
                                else
                                {
                                    platformingEntity.pVelocity = new Vector2(-platformingEntity.pVelocity.X, -minVel * alpha);
                                    platformingEntity.ReverseWalkDirection();
                                }

                                didBounce = true;
                            }
                        }
                        break;
                    case CardinalDirection.Down:
                        {
                            if (platformingEntity.pGrounded == false)
                            {
                                if (platformingEntity.pVelocity.Y < 0.0f)
                                {
                                    platformingEntity.pVelocity = new Vector2(platformingEntity.pVelocity.X, -platformingEntity.pVelocity.Y * alpha);
                                    didBounce = true;
                                }
                            }
                        }

                        break;
                }

                if (didBounce)
                {
                    mBounceAnim.Play();
                }
            }
        }



        public override void Update(GameTime gameTime, Rect2f bounds) 
        {
            mBounceAnim.Update(gameTime);
        }

        public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            float heightReduction = 6.0f;

            switch (mRotation)
            {
                case CardinalDirection.Up:
                    return new Rect2f(topLeft + new Vector2(0.0f, heightReduction), topLeft + new Vector2(sideLength, sideLength));
                case CardinalDirection.Right:
                    return new Rect2f(topLeft, topLeft + new Vector2(sideLength - heightReduction, sideLength));
                case CardinalDirection.Down:
                    return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength - heightReduction));
                case CardinalDirection.Left:
                    return new Rect2f(topLeft + new Vector2(heightReduction, 0.0f), topLeft + new Vector2(sideLength, sideLength));
            }

            throw new NotImplementedException();
        }

        public override Texture2D GetTexture()
        {
            if(mBounceAnim.IsPlaying())
            {
                return mBounceAnim.GetCurrentTexture();
            }

            return mTexture;
        }
    }
}
