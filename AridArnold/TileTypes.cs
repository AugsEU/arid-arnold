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

    enum TileRotation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }

    //============================================
    //  Base class
    //--------------------------------------------
    abstract class Tile
    {
        protected Texture2D mTexture = null;
        protected bool mEnabled = true;
        protected AdjacencyType mAdjacency = AdjacencyType.None;

        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }

        public Tile()
        {
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

        //1 type tiles can be rotated freely.
        public virtual float GetRotation()
        {
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
            mTexture = content.Load<Texture2D>("Tiles/wall4");
        }
    }

    class PlatformTile : Tile
    {
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/platform");
        }

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            if(!entity.CollideWithPlatforms())
            {
                return CollisionResults.None;
            }

            CollisionResults results = Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), topLeft, sideLength);

            //Not a ground, ignore it.
            if(Collision2D.GetCollisionType(results.normal) != CollisionType.Ground)
            {
                results.t = null;
            }

            return results;
        }
    }

    abstract class CollectibleTile : AirTile
    {
        protected abstract CollectibleType GetCollectibleType();

        public override void OnPlayerIntersect(Arnold player) 
        {
            CollectibleManager.I.CollectItem(GetCollectibleType());
            mEnabled = false;
        }
    }

    //============================================
    //  Square types
    //--------------------------------------------
    class SteelTile : SquareTile
    {
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/steel");
        }
    }

    //============================================
    //  Collectible types
    //--------------------------------------------
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
        TileRotation mRotation;

        public SpikesTile(TileRotation rotation) : base()
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
                case TileRotation.Up:
                    topLeft.Y += smallerFactor;
                    topLeft.X += smallerFactor / 2.0f;
                    break;
                case TileRotation.Right:
                    topLeft.Y += smallerFactor / 2.0f;
                    break;
                case TileRotation.Left:
                    topLeft.X += smallerFactor;
                    topLeft.Y += smallerFactor / 2.0f;
                    break;
                case TileRotation.Down:
                    topLeft.X += smallerFactor / 2.0f;
                    break;
            }

            sideLength -= smallerFactor;

            return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
        }

        public override float GetRotation()
        {
            switch (mRotation)
            {
                case TileRotation.Up:
                    return 0.0f;
                case TileRotation.Right:
                    return MathHelper.PiOver2;
                case TileRotation.Down:
                    return MathHelper.Pi;
                case TileRotation.Left:
                    return MathHelper.PiOver2 * 3.0f;
            }

            return 0.0f;
        }
    }
}
