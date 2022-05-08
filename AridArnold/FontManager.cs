using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    internal class FontManager : Singleton<FontManager>
    {
        Dictionary<string, SpriteFont> mFonts = new Dictionary<string, SpriteFont>();

        public void LoadAllFonts(ContentManager content)
        {
            mFonts.Add("Pixica-24", content.Load<SpriteFont>("Fonts/Pixica"));
        }

        public SpriteFont GetFont(string key)
        {
            return mFonts.GetValueOrDefault(key);
        }
    }
}
