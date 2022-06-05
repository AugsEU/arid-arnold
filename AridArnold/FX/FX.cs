using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AridArnold
{
    abstract class FX
    {
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(DrawInfo info);

        public abstract bool Finished();
    }
}
