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

        protected float GetDeltaT(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds * 10.0f;
        }
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
            return mVelocity * GetDeltaT(gameTime);
        }

        protected void ApplyVelocity(GameTime gameTime)
        {
            mPosition += VelocityToDisplacement(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            ApplyVelocity(gameTime);
        }

        public abstract void ReactToCollision(CollisionType collisionType);
    }

    abstract class PlatformingEntity : MovingEntity
    {
        const float DEFAULT_WALK_SPEED = 10.0f;
        const float DEFAULT_GRAVITY = 5.0f;
        const float DEFAULT_JUMP_SPEED = 25.0f;

        protected bool mOnGround;
        protected EntityDirection mDirection;

        private float mWalkSpeed = DEFAULT_WALK_SPEED;
        private float mJumpSpeed = DEFAULT_JUMP_SPEED;
        private float mGravity = DEFAULT_GRAVITY;

        public PlatformingEntity()
        {
            mVelocity = Vector2.Zero;
            mDirection = EntityDirection.None;
        }

        protected void Jump()
        {
            mVelocity.Y = -mJumpSpeed;
        }

        private void ApplyGravity(GameTime gameTime)
        {
            mVelocity.Y += mGravity * GetDeltaT(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            switch (mDirection)
            {
                case EntityDirection.Left:
                    mVelocity.X = -mWalkSpeed;
                    break;
                case EntityDirection.Right:
                    mVelocity.X = mWalkSpeed;
                    break;
                case EntityDirection.None:
                    mVelocity.X = 0.0f;
                    break;
            }

            ApplyGravity(gameTime);

            ApplyCollisions(gameTime);

            base.Update(gameTime);
        }

        private void ApplyCollisions(GameTime gameTime)
        {
            mOnGround = false;

            TileManager.I.ResolveCollisions(this, gameTime);
        }

        public override void ReactToCollision(CollisionType collisionType)
        {
            switch(collisionType)
            {
                case CollisionType.Ground:
                    mOnGround = true;
                    break;
                case CollisionType.Ceiling:
                    mDirection = EntityDirection.None;
                    break;
            }
        }

        public bool grounded
        {
            get => mOnGround;
            set => mOnGround = value;
        }
    }
}
