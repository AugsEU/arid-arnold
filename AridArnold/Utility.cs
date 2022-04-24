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
    }


    internal class Util
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void Log(String msg)
        {
#if DEBUG_LOG
            Debug.WriteLine(msg);
#endif
        }
    }
}
