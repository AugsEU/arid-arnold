namespace AridArnold
{
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

    //============================================
    //  Base class
    //--------------------------------------------
    abstract class Tile
    {
        protected Texture2D mTexture = null;
        protected bool mEnabled = true;
        protected AdjacencyType mAdjacency = AdjacencyType.None;
        protected CardinalDirection mRotation;

        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }

        public Tile()
        {
            mRotation = CardinalDirection.Up;
        }

        public abstract CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime);

        public virtual void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/blank");
        }

        public virtual void Update(GameTime gameTime, Rect2f bounds) { }

        public virtual void OnTouch(MovingEntity entity) { }

        public virtual void OnEntityIntersect(Entity entity, Rect2f bounds) { }

        public virtual Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
        }

        public virtual bool IsSolid()
        {
            return false;
        }

        public virtual Texture2D GetTexture()
        {
            return mTexture;
        }

        public AdjacencyType GetAdjacency()
        {
            return mAdjacency;
        }

        public void SetRightAdjacent(Tile neighbour)
        {
            //My neighbour is to the right of me
            mAdjacency |= AdjacencyType.Right;

            //I'm to the left of my neighbour
            neighbour.mAdjacency |= AdjacencyType.Left;
        }

        public void SetBottomAdjacent(Tile neighbour)
        {
            //My neighbour is under me
            mAdjacency |= AdjacencyType.Bottom;

            //I'm above my neighbour
            neighbour.mAdjacency |= AdjacencyType.Top;
        }

        //Tiles can be rotated.
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

        public virtual SpriteEffects GetEffect()
        {
            return SpriteEffects.None;
        }
    }

    //============================================
    //  Basic types
    //--------------------------------------------
    abstract class SquareTile : Tile
    {
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/blank");
        }

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), GetBounds(topLeft, sideLength));
        }

        public override bool IsSolid()
        {
            return true;
        }
    }

    class AirTile : Tile
    {
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/air");
        }

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            // No collision.
            return CollisionResults.None;
        }

        public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            return new Rect2f(Vector2.Zero, Vector2.Zero);
        }

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

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/" + ProgressManager.I.GetWorldData().platformTexture);
        }

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
                    FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.OliveDrab, entity.position, "+1 Life");
                }
                else
                {
                    FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.White, entity.position, "+0 Lives");
                }
            }
            base.OnEntityIntersect(entity, bounds);
        }
    }

    class WaterTile : CollectibleTile
    {
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

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), topLeft, sideLength, mRotation);
        }

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
                            if (platformingEntity.grounded == false)
                            {
                                if (platformingEntity.velocity.Y > minVel)
                                {
                                    platformingEntity.velocity = new Vector2(platformingEntity.velocity.X, -platformingEntity.velocity.Y * alpha);
                                    didBounce = true;
                                }
                                else if (platformingEntity.velocity.Y > 0.0f)
                                {
                                    platformingEntity.velocity = new Vector2(platformingEntity.velocity.X, -minVel * alpha);
                                    didBounce = true;
                                }

                                if (didBounce)
                                {
                                    float newY = bounds.min.Y - entityBounds.Height;
                                    platformingEntity.position = new Vector2(platformingEntity.position.X, newY);
                                    platformingEntity.grounded = true;
                                }
                            }
                        }
                        break;
                    case CardinalDirection.Left:
                    case CardinalDirection.Right:
                        {
                            bool valid = (CardinalDirection.Left == mRotation) != (platformingEntity.velocity.X < 0.0f);

                            if (valid)
                            {
                                if (platformingEntity.grounded == false)
                                {
                                    platformingEntity.velocity = new Vector2(-platformingEntity.velocity.X, platformingEntity.velocity.Y);
                                    platformingEntity.ReverseWalkDirection();
                                }
                                else
                                {
                                    platformingEntity.velocity = new Vector2(-platformingEntity.velocity.X, -minVel * alpha);
                                    platformingEntity.ReverseWalkDirection();
                                }

                                didBounce = true;
                            }
                        }
                        break;
                    case CardinalDirection.Down:
                        {
                            if (platformingEntity.grounded == false)
                            {
                                if (platformingEntity.velocity.Y < 0.0f)
                                {
                                    platformingEntity.velocity = new Vector2(platformingEntity.velocity.X, -platformingEntity.velocity.Y * alpha);
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
