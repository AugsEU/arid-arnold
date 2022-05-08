//#define DEBUG_LOG

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace AridArnold
{

    struct DrawInfo
    {
        public GameTime gameTime;
        public SpriteBatch spriteBatch;
        public GraphicsDeviceManager graphics;
        public GraphicsDevice device;
    }


    internal static class Util
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static Vector2 Perpendicular(Vector2 a)
        {
            return new Vector2(a.Y, -a.X);
        }

        public static bool CompareHEX(Color color, ulong hexCode)
        {
            ulong colourHex = (ulong)(color.B) + ((ulong)(color.G) << 8) + +((ulong)(color.R) << 16);

            return colourHex == hexCode;
        }

        public static void DrawStringCentred(SpriteBatch sb, SpriteFont font, Vector2 position, Color color, string text)
        {
            Vector2 size = font.MeasureString(text);

            sb.DrawString(font, text, position - size / 2, color);
        }

        public static void Log(string msg)
        {
#if DEBUG_LOG
            Debug.WriteLine(msg);
#endif
        }

        public static void DLog(string msg)
        {
            Debug.WriteLine(msg);
        }
    }


}
