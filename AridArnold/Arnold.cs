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
        public Arnold()
        {
            mPosition = new Vector2(400, 190);
            mVelocity = new Vector2(0, 0);
        }

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("arnold");
        }

        public override void Update(GameTime gameTime)
        {
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

            base.Update(gameTime);
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
