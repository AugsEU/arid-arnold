using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    internal abstract class AIEntity : PlatformingEntity
    {
        private float mCWidthReduction;
        private float mCHeightReduction;
        protected WalkDirection mPrevDirection;
        protected Animator mRunningAnimation;
        protected Animator mStandAnimation;

        protected Texture2D mJumpUpTex;
        protected Texture2D mJumpDownTex;

        protected MonoRandom mRandom;
        int mStartSeed;
        int mFrameNum;

        public AIEntity(Vector2 pos, float walkSpeed, float jumpSpeed, float widthReduction, float heightReduction, int tileLook = 3) : base(pos, walkSpeed, jumpSpeed)
        {
            mRandom = new MonoRandom();

            mRandom.SetSeed(0);
            mRandom.ChugNumber((int)pos.X);
            mRandom.ChugNumber((int)pos.Y);

            mStartSeed = mRandom.Next();

            mPrevDirection = mRandom.PercentChance(50.0f) ? WalkDirection.Left : WalkDirection.Right;
            mWalkDirection = WalkDirection.None;

            mCWidthReduction = widthReduction;
            mCHeightReduction = heightReduction;

            mFrameNum = 0;
        }

        public override void Update(GameTime gameTime)
        {
            mFrameNum++;
            ChugRandom();

            mRunningAnimation.Update(gameTime);
            mStandAnimation.Update(gameTime);

            DecideActions();

            if(mWalkDirection != WalkDirection.None)
            {
                mPrevDirection = mWalkDirection;
            }

            base.Update(gameTime);
        }

        private void ChugRandom()
        {
            //Make random deterministic based on player movement.
            mRandom.SetSeed(mStartSeed);
            mRandom.ChugNumber((int)(mFrameNum / 15));

            int entityNum = EntityManager.I.GetEntityNum();

            for(int i = 0; i < entityNum; i++)
            {
                Entity entity = EntityManager.I.GetEntity(i);

                if(entity is Arnold)
                {
                    Vector2 pos = entity.position;

                    mRandom.ChugNumber((int)(pos.X / 128.0f));
                    mRandom.ChugNumber((int)(pos.Y / 128.0f));
                }
            }
        }

        protected Tile GetNearbyTile(int dx, int dy)
        {
            return TileManager.I.GetRelativeTile(mCentreOfMass, dx, dy);
        }

        protected bool CheckSolid(int dx, int dy)
        {
            return GetNearbyTile(dx,dy).IsSolid();
        }

        protected abstract void DecideActions();

        public override void Draw(DrawInfo info)
        {
            Texture2D texture = mTexture;

            if(mOnGround)
            {
                if (mWalkDirection == WalkDirection.None)
                {
                    texture = mStandAnimation.GetCurrentTexture();
                }
                else
                {
                    texture = mRunningAnimation.GetCurrentTexture();
                }
            }
            else
            {
                if(mVelocity.Y <= 0.0f)
                {
                    texture = mJumpUpTex;            
                }
                else
                {
                    texture = mJumpDownTex;
                }
            }

            SpriteEffects effect = mPrevDirection == WalkDirection.Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            info.spriteBatch.Draw(texture, mPosition, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, 0.0f);
        }

        public override Rect2f ColliderBounds()
        {
            float height = mTexture.Height - mCHeightReduction;
            float width = mTexture.Width - mCWidthReduction;

            Vector2 effectivePosition;
            Vector2 effectiveSize;

            if (GetGravityDir() == CardinalDirection.Left || GetGravityDir() == CardinalDirection.Right)
            {
                effectivePosition = mPosition + new Vector2(mCWidthReduction, mCHeightReduction / 2.0f);
                effectiveSize = new Vector2(height, width);
            }
            else
            {
                effectivePosition = mPosition + new Vector2(mCWidthReduction / 2.0f, mCHeightReduction);
                effectiveSize = new Vector2(width, height);
            }

            return new Rect2f(effectivePosition, effectivePosition + effectiveSize);
        }

        public override void CollideWithEntity(Entity entity)
        {
            if(entity is Arnold)
            {
                EArgs args;
                args.sender = this;

                EventManager.I.SendEvent(EventType.KillPlayer, args);
            }

            base.CollideWithEntity(entity);
        }
    }
}
