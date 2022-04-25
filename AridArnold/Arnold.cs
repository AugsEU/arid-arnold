using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    class Arnold : PlatformingEntity
    {
        EntityDirection mPrevDirection;

        Animator mRunningAnimation;

        Texture2D mJumpUpTex;
        Texture2D mJumpDownTex;

        public Arnold()
        {
            mPosition = new Vector2(400, 190);
            mVelocity = new Vector2(0, 0);
        }

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Arnold/arnold-stand");
            mJumpUpTex = content.Load<Texture2D>("Arnold/arnold-jump-up");
            mJumpDownTex = content.Load<Texture2D>("Arnold/arnold-jump-down");

            mRunningAnimation = new Animator();
            mRunningAnimation.LoadFrame(content, "Arnold/arnold-run0", 0.1f);
            mRunningAnimation.LoadFrame(content, "Arnold/arnold-run1", 0.1f);
            mRunningAnimation.LoadFrame(content, "Arnold/arnold-run2", 0.1f);
            mRunningAnimation.LoadFrame(content, "Arnold/arnold-run3", 0.15f);

            mRunningAnimation.Play();
        }

        public override void Update(GameTime gameTime)
        {
            mRunningAnimation.Update(gameTime);

            KeyboardState state = Keyboard.GetState();

            if (mOnGround)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    Jump();
                }

                if(state.IsKeyDown(Keys.Left))
                {
                    mDirection = EntityDirection.Left;
                    mPrevDirection = mDirection;
                }
                else if(state.IsKeyDown(Keys.Right))
                {
                    mDirection = EntityDirection.Right;
                    mPrevDirection = mDirection;
                }
                else
                {
                    mDirection = EntityDirection.None;
                }
            }

            base.Update(gameTime);
        }

        public override Rect2f ColliderBounds()
        {
            return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Width, mTexture.Height));
        }

        public override void Draw(DrawInfo info)
        {
            SpriteEffects effect = SpriteEffects.None;

            if(mPrevDirection == EntityDirection.Left)
            {
                effect = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = mTexture;

            if(mOnGround)
            {
                if (mDirection != EntityDirection.None)
                {
                    texture = mRunningAnimation.GetCurrentTexture();
                }
            }
            else
            {
                if (mVelocity.Y > 0.0f)
                {
                    texture = mJumpDownTex;
                }
                else
                {
                    texture = mJumpUpTex;
                }
            }

            info.spriteBatch.Draw(texture, new Rectangle((int)mPosition.X, (int)mPosition.Y+1, mTexture.Width, mTexture.Height), null, Color.White, 0.0f, Vector2.Zero, effect, 0.0f);
        }

    }
}
