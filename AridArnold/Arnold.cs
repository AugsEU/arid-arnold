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
        const double DEATH_TIME = 500.0;
        const double FLASH_TIME = 100.0;

        EntityDirection mPrevDirection;
        Animator mRunningAnimation;

        Texture2D mJumpUpTex;
        Texture2D mJumpDownTex;

        //Various timers.
        MonoTimer mTimerSinceDeath;

        public Arnold(Vector2 pos) : base(pos)
        {
            mDirection = EntityDirection.None;
            mPrevDirection = EntityDirection.Right;

            EventManager.I.AddListener(EventType.KillPlayer, SignalPlayerDead);

            mTimerSinceDeath = new MonoTimer();
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

            //Botch position a bit. Not sure what's happening here.
            mPosition.Y -= 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            if(mTimerSinceDeath.IsPlaying())
            {
                if(mTimerSinceDeath.GetElapsedMs() > DEATH_TIME)
                {
                    SendPlayerDeathEvent();
                    mTimerSinceDeath.FullReset();
                }

                return;
            }

            mRunningAnimation.Update(gameTime);

            KeyboardState state = Keyboard.GetState();

            if (mOnGround)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    if(state.IsKeyDown(Keys.Down))
                    {
                        FallThroughPlatforms();
                    }
                    else
                    {
                        Jump();
                    }
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

            TileManager.I.ArnoldTouchTiles(this);

            if(mPosition.Y > TileManager.I.GetDrawHeight())
            {
                Kill();
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

            int xDiff = (texture.Width - mTexture.Width)/2;
            int yDiff = texture.Height - mTexture.Height;

            Color color = Color.White;

            if(mTimerSinceDeath.IsPlaying())
            {
                double timeSinceDeath = mTimerSinceDeath.GetElapsedMs();

                if((int)(timeSinceDeath / FLASH_TIME) % 2 == 0)
                {
                    color = new Color(255,51,33);
                }
                else
                {
                    color = new Color(255, 128, 79);
                }
            }

            info.spriteBatch.Draw(texture, new Rectangle((int)MathF.Round(mPosition.X) - xDiff, (int)mPosition.Y+1 - yDiff, texture.Width, texture.Height), null, color, 0.0f, Vector2.Zero, effect, 0.0f);
        }


        public override void Kill()
        {
            mTimerSinceDeath.Start();
        }

        private void SendPlayerDeathEvent()
        {
            EArgs eArgs;
            eArgs.sender = this;

            EventManager.I.SendEvent(EventType.PlayerDead, eArgs);

            base.Kill();
        }

        public virtual void SignalPlayerDead(EArgs args)
        {
            Kill();
        }
    }
}
