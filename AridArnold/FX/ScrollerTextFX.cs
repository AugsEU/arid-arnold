using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AridArnold
{
    internal class ScrollerTextFX : FX
    {
        SpriteFont mFont;
        Color mColor;
        Vector2 mStartPos;
        Vector2 mPos;
        float mSpeed;
        float mMaxHeight;
        float mTime;

        string mText;

        public ScrollerTextFX(SpriteFont font, Color col, Vector2 pos, string text, float upSpeed, float maxHeight, float time)
        {
            mFont = font;
            mColor = col;
            mPos = pos;
            mSpeed = upSpeed;
            mText = text;
            mMaxHeight = maxHeight;
            mTime = time;

            mStartPos = pos;
        }

        public override void Update(GameTime gameTime)
        {
            mPos.Y -= mSpeed * Util.GetDeltaT(gameTime);
        }

        public override void Draw(DrawInfo info)
        {
            Vector2 drawPos = mPos;

            drawPos.Y = Math.Max(drawPos.Y, mStartPos.Y - mMaxHeight);
            Util.DrawStringCentred(info.spriteBatch, mFont, drawPos, mColor, mText);
        }

        public override bool Finished()
        {
            return Math.Abs(mStartPos.Y - mPos.Y) > mTime + mMaxHeight;
        }
    }
}
