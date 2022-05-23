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

    enum CardinalDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
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

        public static Vector2 GetNormal(CardinalDirection dir)
        {
            switch(dir)
            {
                case CardinalDirection.Up:
                    return new Vector2(0.0f, -1.0f);
                case CardinalDirection.Down:
                    return new Vector2(0.0f, 1.0f);
                case CardinalDirection.Left:
                    return new Vector2(-1.0f, 0.0f);
                case CardinalDirection.Right:
                    return new Vector2(1.0f, 0.0f);
            }

            throw new NotImplementedException();
        }

        public static CardinalDirection InvertDirection(CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.Up:
                    return CardinalDirection.Down;
                case CardinalDirection.Right:
                    return CardinalDirection.Left;
                case CardinalDirection.Down:
                    return CardinalDirection.Up;
                case CardinalDirection.Left:
                    return CardinalDirection.Right;
            }

            throw new NotImplementedException();
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

        public static void DrawRect(DrawInfo info, Rect2f rect2f, Color col)
        {
            Point min = new Point((int)rect2f.min.X, (int)rect2f.min.Y);
            Point max = new Point((int)rect2f.max.X, (int)rect2f.max.Y);

            int width = max.X - min.X;
            int height = max.Y - min.Y;

            Texture2D rect = new Texture2D(info.device, width, height);

            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = col;
            }
            rect.SetData(data);

            info.spriteBatch.Draw(rect, rect2f.min, Color.White);
        }

        public static void BrightenColour(ref Color col, float bright)
        {
            col.R = (byte)((col.R * (1 - bright)) + (255 * bright));
            col.G = (byte)((col.G * (1 - bright)) + (255 * bright));
            col.B = (byte)((col.B * (1 - bright)) + (255 * bright));
        }

        public static Vector2 CalcRotationOffset(float rotation, float height)
        {
            return CalcRotationOffset(rotation, height, height);
        }

        public static Vector2 CalcRotationOffset(float rotation, float width, float height)
        {
            float c = MathF.Cos(rotation);
            float s = MathF.Sin(-rotation);

            Vector2 oldCentre = new Vector2(width / 2.0f, height / 2.0f);
            Vector2 newCentre = new Vector2(oldCentre.X * c - oldCentre.Y * s, oldCentre.X * s + oldCentre.Y * c);

            return oldCentre - newCentre;
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
