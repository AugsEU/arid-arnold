using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{

    enum WalkDirection
    {
        Left,
        Right,
        None,
    }

    enum GravityDirection
    {
        Down = 0,
        Up,
        Left,
        Right,
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
                    ReactToCollision(res.result.normal);
                }
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

        protected abstract void ReactToCollision(Vector2 collisionNormal);
    }

    abstract class PlatformingEntity : MovingEntity
    {
        const float DEFAULT_WALK_SPEED = 9.0f;
        const float DEFAULT_GRAVITY = 4.35f;
        const float DEFAULT_JUMP_SPEED = 25.0f;

        protected bool mOnGround;

        //Magniture of motion vectors
        private float mWalkSpeed;
        private float mJumpSpeed;
        private float mGravity; 

        private GravityDirection mGravityDirection;
        protected WalkDirection mWalkDirection;

        public PlatformingEntity(Vector2 pos) : base(pos)
        {
            mVelocity = Vector2.Zero;
            mWalkDirection = WalkDirection.None;

            mWalkSpeed = DEFAULT_WALK_SPEED;
            mJumpSpeed = DEFAULT_JUMP_SPEED;
            mGravity = DEFAULT_GRAVITY;

            mGravityDirection = GravityDirection.Down;
        }

        public void SetGravity(GravityDirection dir)
        {
            mGravityDirection = dir;
        }

        protected GravityDirection GetGravityDir()
        {
            return mGravityDirection;
        }

        protected Vector2 GravityVecNorm()
        {
            switch (GetGravityDir())
            {
                case AridArnold.GravityDirection.Down:
                    return new Vector2(0.0f, 1.0f);
                case AridArnold.GravityDirection.Up:
                    return new Vector2(0.0f, -1.0f);
                case AridArnold.GravityDirection.Left:
                    return new Vector2(-1.0f, 0.0f);
                case AridArnold.GravityDirection.Right:
                    return new Vector2(1.0f, 0.0f);
            }

            throw new NotImplementedException();
        }

        protected void Jump()
        {
            mVelocity = -mJumpSpeed * GravityVecNorm();
        }

        protected void FallThroughPlatforms()
        {
            mFallthrough = true;
        }

        private void ApplyGravity(GameTime gameTime)
        {
            float vecAlongGrav = Vector2.Dot(GravityVecNorm(), mVelocity);

            float mod = 1.0f;
            if(vecAlongGrav < 0.0f)
            {
                mod = 2.0f;
            }

            float delta = mGravity * GetDeltaT(gameTime) * mod;
            Vector2 deltaVec = GravityVecNorm() * delta;

            if(-delta < vecAlongGrav && vecAlongGrav < 0.0f )
            {
                deltaVec = deltaVec / 3.5f;
            }

            mVelocity += deltaVec;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 downVec = GravityVecNorm();

            Vector2 sideVec = new Vector2(MathF.Abs(downVec.Y), MathF.Abs(downVec.X));

            float component = Vector2.Dot(mVelocity, sideVec);

            mVelocity = mVelocity - component * sideVec;

            switch (mWalkDirection)
            {
                case WalkDirection.Left:
                    mVelocity -= mWalkSpeed * sideVec;
                    break;
                case WalkDirection.Right:
                    mVelocity += mWalkSpeed * sideVec;
                    break;
                case WalkDirection.None:
                    break;
            }

            ApplyGravity(gameTime);

            mOnGround = false;
            base.Update(gameTime);
        }

        protected override void ReactToCollision(Vector2 normal)
        {
            CollisionType collisionType;

            float dp = Vector2.Dot(normal, GravityVecNorm());

            if(dp >= 0.95f)
            {
                collisionType = CollisionType.Ceiling;
            }
            else if(dp <= -0.95f)
            {
                collisionType = CollisionType.Ground;
            }
            else
            {
                collisionType= CollisionType.Wall;
            }

            switch (collisionType)
            {
                case CollisionType.Ground:
                    mOnGround = true;
                    break;
                case CollisionType.Ceiling:
                    mVelocity = GravityVecNorm() * mGravity;
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
