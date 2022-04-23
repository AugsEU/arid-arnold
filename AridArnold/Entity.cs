using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{

    enum EntityDirection
    {
        Left,
        Right,
        None,
    }

    abstract class Entity
    {
        protected Vector2 mPosition;
        protected Texture2D mTexture;

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(DrawInfo info);

        public abstract void LoadContent(ContentManager content);
    }

    abstract class MovingEntity : Entity
    {
        protected Vector2 mVelocity;

        public Vector2 velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        public abstract Rect2f ColliderBounds();

        public Vector2 VelocityToDisplacement(GameTime gameTime)
        {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds * 10.0f;

            return mVelocity * deltaT;
        }

        protected void ApplyVelocity(GameTime gameTime)
        {
            mPosition += VelocityToDisplacement(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            ApplyVelocity(gameTime);
        }
    }
}
