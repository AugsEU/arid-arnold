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

        public Entity(Vector2 pos)
        {
            mPosition = pos;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(DrawInfo info);

        public abstract void LoadContent(ContentManager content);

        protected float GetDeltaT(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds * 10.0f;
        }

        public virtual void Kill()
        {

        }
    }

    abstract class MovingEntity : Entity
    {
        protected Vector2 mVelocity;
        protected bool mFallthrough;

        public MovingEntity(Vector2 pos) : base(pos)
        {
        }

        public bool CollideWithPlatforms()
        {
            return !mFallthrough;
        }

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

        private void ApplyCollisions(GameTime gameTime)
        {
            List<TileCollisionResults> tileResults = TileManager.I.ResolveCollisions(this, gameTime);

            ApplyVelocity(gameTime);

            //Post collision effects.
            foreach (var res in tileResults)
            {
                if(res.result.Collided)
                {
                    ReactToCollision(Collision2D.GetCollisionType(res.result.normal));
                }

                //mTileMap[res.coord.X, res.coord.Y].OnTouch(this);
            }
        }

        protected void ApplyVelocity(GameTime gameTime)
        {
            mPosition += VelocityToDisplacement(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            ApplyCollisions(gameTime);

            mFallthrough = false;
        }

        protected abstract void ReactToCollision(CollisionType collisionType);
    }

    abstract class PlatformingEntity : MovingEntity
    {
        const float DEFAULT_WALK_SPEED = 9.0f;
        const float DEFAULT_GRAVITY = 4.35f;
        const float DEFAULT_JUMP_SPEED = 25.0f;

        protected bool mOnGround;
        protected EntityDirection mDirection;

        private float mWalkSpeed = DEFAULT_WALK_SPEED;
        private float mJumpSpeed = DEFAULT_JUMP_SPEED;
        private float mGravity = DEFAULT_GRAVITY;

        public PlatformingEntity(Vector2 pos) : base(pos)
        {
            mVelocity = Vector2.Zero;
            mDirection = EntityDirection.None;
        }

        protected void Jump()
        {
            mVelocity.Y = -mJumpSpeed;
        }

        protected void FallThroughPlatforms()
        {
            mFallthrough = true;
        }

        private void ApplyGravity(GameTime gameTime)
        {
            float mod = 1.0f;
            if(mVelocity.Y < 0.0f)
            {
                mod = 2.0f;
            }

            float delta = mGravity * GetDeltaT(gameTime) * mod;

            if(mVelocity.Y < 0.0f && delta > -mVelocity.Y)
            {
                delta = delta / 3.5f;
            }

            mVelocity.Y += delta;

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

            mOnGround = false;
            base.Update(gameTime);
        }

        protected override void ReactToCollision(CollisionType collisionType)
        {
            switch(collisionType)
            {
                case CollisionType.Ground:
                    mOnGround = true;
                    break;
                case CollisionType.Ceiling:
                    mVelocity.Y = mGravity;
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
