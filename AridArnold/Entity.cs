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

    abstract class Entity
    {
        protected Vector2 mPosition;
        protected Texture2D mTexture;

        public Entity(Vector2 pos)
        {
            mPosition = pos;
        }

        public virtual void Update(GameTime gameTime)
        {
            TileManager.I.EntityTouchTiles(this);
        }

        public abstract void Draw(DrawInfo info);

        public abstract void LoadContent(ContentManager content);

        public abstract Rect2f ColliderBounds();

        public void ShiftPosition(Vector2 shift)
        {
            mPosition += shift;
        }
        public Vector2 position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public virtual void CollideWithEntity(Entity entity)
        {

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

        public Vector2 VelocityToDisplacement(GameTime gameTime)
        {
            return mVelocity * Util.GetDeltaT(gameTime);
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

                TileManager.I.GetTile(res.coord).OnTouch(this);
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

            base.Update(gameTime);
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
        protected float mWalkSpeed;
        protected float mJumpSpeed;
        protected float mGravity; 

        private CardinalDirection mCardinalDirection;
        protected WalkDirection mWalkDirection;

        public PlatformingEntity(Vector2 pos, float walkSpeed = DEFAULT_WALK_SPEED, float jumpSpeed = DEFAULT_JUMP_SPEED, float gravity = DEFAULT_GRAVITY) : base(pos)
        {
            mVelocity = Vector2.Zero;
            mWalkDirection = WalkDirection.None;

            mWalkSpeed = walkSpeed;
            mJumpSpeed = jumpSpeed;
            mGravity = gravity;

            mCardinalDirection = CardinalDirection.Down;
        }

        public void SetGravity(CardinalDirection dir)
        {
            mCardinalDirection = dir;
        }

        public void SetWalkDirection(WalkDirection dir)
        {
            mWalkDirection=dir;
        }

        public CardinalDirection GetGravityDir()
        {
            return mCardinalDirection;
        }

        protected Vector2 GetForwardVec()
        {
            if(mWalkDirection == WalkDirection.Left)
            {
                return new Vector2(-1.0f, 0.0f);
            }

            return new Vector2(1.0f, 0.0f);
        }

        protected Vector2 GravityVecNorm()
        {
            switch (GetGravityDir())
            {
                case AridArnold.CardinalDirection.Down:
                    return new Vector2(0.0f, 1.0f);
                case AridArnold.CardinalDirection.Up:
                    return new Vector2(0.0f, -1.0f);
                case AridArnold.CardinalDirection.Left:
                    return new Vector2(-1.0f, 0.0f);
                case AridArnold.CardinalDirection.Right:
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

            float delta = mGravity * Util.GetDeltaT(gameTime) * mod;
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
                    if(mOnGround == false)
                    {
                        mVelocity += component * sideVec;
                    }
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
