using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    class Arnold : MovingEntity
    {
        const float WALK_SPEED = 10.0f;
        const float GRAVITY = 15.0f;
        const float JUMP_SPEED = 25.0f;

        bool mOnGround;
        EntityDirection mDirection;

        public Arnold()
        {
            mDirection = EntityDirection.None;

            mPosition = new Vector2(400, 190);
            mVelocity = new Vector2(0, 0);
        }

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("arnold");
        }

        public override void Update(GameTime gameTime)
        {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds * 10.0f;

            KeyboardState state = Keyboard.GetState();

            mVelocity.Y += GRAVITY * deltaT;

            mOnGround = true;

            if (mOnGround)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    mVelocity.Y = -JUMP_SPEED;
                }

                if(state.IsKeyDown(Keys.Left))
                {
                    mDirection = EntityDirection.Left;
                }
                else if(state.IsKeyDown(Keys.Right))
                {
                    mDirection = EntityDirection.Right;
                }
                else
                {
                    mDirection = EntityDirection.None;
                }
            }


            switch(mDirection)
            {
                case EntityDirection.Left:
                    mVelocity.X = -WALK_SPEED;
                    break;
                case EntityDirection.Right:
                    mVelocity.X = WALK_SPEED;
                    break;
                default:
                    mVelocity.X = 0.0f;
                    break;
            }

            ApplyCollisions(gameTime);

            base.Update(gameTime);
        }

        private void ApplyCollisions(GameTime gameTime)
        {
            TileManager.I.ResolveCollisions(this, gameTime);
        }

        public override Rect2f ColliderBounds()
        {
            return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Width, mTexture.Height));
        }

        public override void Draw(DrawInfo info)
        {
            info.spriteBatch.Draw(mTexture, mPosition, Color.White);
        }

    }
}
