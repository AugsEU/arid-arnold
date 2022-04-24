using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    enum TileType
    {
        Square,
        Air,
        Wall,
        Platform
    }

    abstract class Tile
    {
        protected Texture2D mTexture = null;

        public Tile()
        {
        }

        public abstract TileType GetTileType(); 
        public abstract CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime);
    }

    abstract class SquareTile : Tile
    {
        public override TileType GetTileType()
        {
            return TileType.Square;
        }

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            Rect2f rect2F = new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
            return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), rect2F);
        }
    }

    class AirTile : Tile
    {
        public override TileType GetTileType()
        {
            return TileType.Air;
        }

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            // No collision.
            CollisionResults results;
            results.t = null;
            results.normal = Vector2.Zero;

            return results;
        }
    }

    class WallTile : SquareTile
    {
        public override TileType GetTileType()
        {
            return TileType.Wall;
        }
    }

    class PlatformTile : Tile
    {
        public override TileType GetTileType()
        {
            return TileType.Platform;
        }

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            Rect2f rect2F = new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
            CollisionResults results = Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), rect2F);

            //Not a ground, ignore it.
            if(Collision2D.GetCollisionType(results.normal) != CollisionType.Ground)
            {
                results.t = null;
            }

            return results;
        }
    }
}
