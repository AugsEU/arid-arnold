using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AridArnold
{
    internal class FXManager : Singleton<FXManager>
    {
        List<FX> mFXList = new List<FX>();

        public void AddTextScroller(SpriteFont font, Color col, Vector2 pos, string text, float upSpeed = 4.1f, float maxHeight = 10.0f, float time = 20.0f)
        {
            mFXList.Add(new ScrollerTextFX(font, col, pos, text, upSpeed, maxHeight, time));
        }

        public void Update(GameTime gameTime)
        {
            for(int i = 0; i < mFXList.Count; i++)
            {
                FX fx = mFXList[i];

                if (fx.Finished())
                {
                    mFXList.RemoveAt(i);
                    i--;
                }
                else
                {
                    fx.Update(gameTime);
                }
            }
        }

        public void Draw(DrawInfo info)
        {
            foreach(FX fx in mFXList)
            {
                fx.Draw(info);
            }
        }

        public void Clear()
        {
            mFXList.Clear();
        }
    }
}
