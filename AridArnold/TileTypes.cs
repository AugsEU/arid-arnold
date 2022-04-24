using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    abstract class Tile
    {
        protected Texture2D mTexture = null;

        public Tile()
        {
        }

        public abstract void LoadContent(ContentManager content);

        public abstract CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime);

        public Texture2D GetTexture(DrawInfo info)
        {
            if (mTexture == null)
            {
                mTexture = new Texture2D(info.graphics.GraphicsDevice, 1, 1);

                Color[] data = new Color[1];
                data[0] = Color.White;
                mTexture.SetData(data);
            }

            return mTexture;
        }
    }

    abstract class SquareTile : Tile
    {
        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            Rect2f rect2F = new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
            return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), rect2F);
        }
    }

    class AirTile : Tile
    {
        public AirTile()
        {
        }

        public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
        {
            // No collision.
            CollisionResults results;
            results.t = null;
            results.normal = Vector2.Zero;

            return results;
        }

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/air");
        }
    }

    class WallTile : SquareTile
    {
        public WallTile()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/wall");
        }
    }
}
