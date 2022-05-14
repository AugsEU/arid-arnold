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

        WalkDirection mPrevDirection;
        Animator mRunningAnimation;

        Texture2D mJumpUpTex;
        Texture2D mJumpDownTex;

        //Various timers.
        MonoTimer mTimerSinceDeath;

        public Arnold(Vector2 pos) : base(pos)
        {
            mPrevDirection = WalkDirection.Right;

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
                    if(state.IsKeyDown(GetFallthroughKey()))
                    {
                        FallThroughPlatforms();
                    }
                    else
                    {
                        Jump();
                    }
                }

                HandleWalkInput(state);
            }

            //DEBUG
            if (state.IsKeyDown(Keys.W))
            {
                SetGravity(GravityDirection.Up);
            }
            else if (state.IsKeyDown(Keys.S))
            {
                SetGravity(GravityDirection.Down);
            }
            else if (state.IsKeyDown(Keys.A))
            {
                SetGravity(GravityDirection.Left);
            }
            else if (state.IsKeyDown(Keys.D))
            {
                SetGravity(GravityDirection.Right);
            }
            //DEBUG

            TileManager.I.ArnoldTouchTiles(this);

            if(CheckOffScreenDeath())
            {
                Kill();
            }

            base.Update(gameTime);
        }

        private Keys GetFallthroughKey()
        {
            switch (GetGravityDir())
            {
                case GravityDirection.Down:
                    return Keys.Down;
                case GravityDirection.Up:
                    return Keys.Up;
                case GravityDirection.Left:
                    return Keys.Left;
                case GravityDirection.Right:
                    return Keys.Right;
            }

            throw new NotImplementedException();
        }

        private void HandleWalkInput(KeyboardState state)
        {
            GravityDirection gravDir = GetGravityDir();

            if(gravDir == GravityDirection.Down || gravDir == GravityDirection.Up)
            {
                if (state.IsKeyDown(Keys.Left))
                {
                    mWalkDirection = WalkDirection.Left;
                    mPrevDirection = mWalkDirection;
                }
                else if (state.IsKeyDown(Keys.Right))
                {
                    mWalkDirection = WalkDirection.Right;
                    mPrevDirection = mWalkDirection;
                }
                else
                {
                    mWalkDirection = WalkDirection.None;
                }
            }
            else if(gravDir == GravityDirection.Left || gravDir == GravityDirection.Right)
            {
                if (state.IsKeyDown(Keys.Up))
                {
                    mWalkDirection = WalkDirection.Left;
                    mPrevDirection = mWalkDirection;

                }
                else if (state.IsKeyDown(Keys.Down))
                {
                    mWalkDirection = WalkDirection.Right;
                    mPrevDirection = mWalkDirection;
                }
                else
                {
                    mWalkDirection = WalkDirection.None;
                }
            }
        }

        private bool CheckOffScreenDeath()
        {
            //if (IsGravityInverted())
            //{
            //    return mPosition.Y < mTexture.Height/2.0f;
            //}

            return mPosition.Y > TileManager.I.GetDrawHeight();
        }

        public override Rect2f ColliderBounds()
        {
            if(GetGravityDir() == GravityDirection.Left || GetGravityDir() == GravityDirection.Right)
            {
                return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Height, mTexture.Width));
            }

            return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Width, mTexture.Height));
        }

        public override void Draw(DrawInfo info)
        {
            SpriteEffects effect = SpriteEffects.None;

            if (mPrevDirection == WalkDirection.Left)
            {
                effect = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = mTexture;

            if (mOnGround)
            {
                if (mWalkDirection != WalkDirection.None)
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

            int xDiff = texture.Width - mTexture.Width;
            int yDiff = texture.Height - mTexture.Height;

            float rotation = 0.0f;

            switch (GetGravityDir())
            {
                case GravityDirection.Down:
                    rotation = 0.0f;
                    xDiff = xDiff / 2;
                    break;
                case GravityDirection.Up:
                    rotation = MathHelper.Pi;
                    yDiff = 1;
                    xDiff = xDiff / 2;
                    effect = effect ^ SpriteEffects.FlipHorizontally;
                    break;
                case GravityDirection.Left:
                    Util.Swap(ref xDiff, ref yDiff);

                    xDiff = -xDiff/2;

                    yDiff += 2;

                    rotation = MathHelper.PiOver2;
                    break;
                case GravityDirection.Right:
                    rotation = MathHelper.PiOver2 * 3.0f;
                    effect = effect ^ SpriteEffects.FlipHorizontally;

                    
                    xDiff--;
                    yDiff++;

                    break;
            }

            Vector2 rotationOffset = Util.CalcRotationOffset(rotation, texture.Width, texture.Height);

            Util.DrawRect(info, ColliderBounds(), Color.Blue);
            info.spriteBatch.Draw(texture, new Rectangle((int)MathF.Round(mPosition.X) - xDiff, (int)mPosition.Y + 1 - yDiff, texture.Width, texture.Height), null, GetDrawColour(), rotation, rotationOffset, effect, 0.0f);

            
        }

        private Color GetDrawColour()
        {
            if (mTimerSinceDeath.IsPlaying())
            {
                double timeSinceDeath = mTimerSinceDeath.GetElapsedMs();

                if ((int)(timeSinceDeath / FLASH_TIME) % 2 == 0)
                {
                    return new Color(255, 51, 33);
                }
                else
                {
                    return new Color(255, 128, 79);
                }
            }

            return Color.White;
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
