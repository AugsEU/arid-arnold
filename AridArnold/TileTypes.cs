using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    enum AdjacencyType
    {
        None =              0b0000,
        Top =               0b0001,
        Bottom =            0b0010,
        Left =              0b0100,
        Right =             0b1000,
        TopBottom =         Top | Bottom,
        TopLeft =           Top | Left,
        TopRight =          Top | Right,
        TopBottomLeft =     Top | Bottom | Left,
        TopBottomRight =    Top | Bottom | Right,
        TopLeftRight =      Top | Left | Right,
        BottomRight =       Bottom | Right,
        BottomLeft =        Bottom | Left,
        BottomLeftRight =   Bottom | Left | Right,
        LeftRight =         Left | Right,
        All =               Top | Bottom | Left | Right,
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

        public virtual void Update(GameTime gameTime) { }

        public virtual void OnTouch(MovingEntity entity) { }

        public virtual void OnPlayerIntersect(Arnold player) { }

        public virtual Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
        }

        public Texture2D GetTexture()
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
            if(!entity.CollideWithPlatforms())
            {
                return CollisionResults.None;
            }

            return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), topLeft, sideLength, mRotation);
        }
    }

    //============================================
    //  Collectible types
    //--------------------------------------------
    abstract class CollectibleTile : AirTile
    {
        protected abstract CollectibleType GetCollectibleType();

        public override void OnPlayerIntersect(Arnold player) 
        {
            CollectibleManager.I.CollectItem(GetCollectibleType());
            mEnabled = false;
        }
    }

    class FlagTile : CollectibleTile
    {
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/flag");
        }

        public override void OnPlayerIntersect(Arnold player)
        {
            ProgressManager.I.ReportCheckpoint();
            base.OnPlayerIntersect(player);
        }

        protected override CollectibleType GetCollectibleType()
        {
            return CollectibleType.Flag;
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
    class SpikesTile : AirTile
    {
        public SpikesTile(CardinalDirection rotation) : base()
        {
            mRotation = rotation;
        }

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/spikes");
        }

        public override void OnPlayerIntersect(Arnold player)
        {
            EArgs eArgs;
            eArgs.sender = this;

            EventManager.I.SendEvent(EventType.KillPlayer, eArgs);
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

        public override void OnPlayerIntersect(Arnold entity) 
        {
            Rect2f bounds = entity.ColliderBounds();

            switch (mRotation)
            {
                case CardinalDirection.Up:
                    if (entity.GetGravityDir() != CardinalDirection.Down)
                    {
                        entity.SetGravity(CardinalDirection.Down);
                        entity.ShiftPosition(new Vector2(0.0f, -(bounds.Height + 16.0f)));
                    }
                    break;
                case CardinalDirection.Right:
                    if (entity.GetGravityDir() != CardinalDirection.Left)
                    {
                        entity.SetGravity(CardinalDirection.Left);
                        entity.ShiftPosition(new Vector2(bounds.Width + 16.0f, 0.0f));
                    }
                    break;
                case CardinalDirection.Down:
                    if (entity.GetGravityDir() != CardinalDirection.Up)
                    {
                        entity.SetGravity(CardinalDirection.Up);
                        entity.ShiftPosition(new Vector2(0.0f, bounds.Height+16.0f));
                    }
                    break;
                case CardinalDirection.Left:
                    if (entity.GetGravityDir() != CardinalDirection.Right)
                    {
                        entity.SetGravity(CardinalDirection.Right);
                        entity.ShiftPosition(new Vector2(-(bounds.Width + 16.0f), 0.0f));
                    }
                    break;
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
}
