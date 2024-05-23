using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WormWarp
{
	class Utilities
	{
		static public Random RNG = new Random();

		static public Rectangle getBounds(Vector2 Offset, int x, int y, int bwidth, int bheight)
		{
			return new Rectangle((int)Offset.X + x * bwidth, (int)Offset.Y + y * bheight, bwidth, bheight);

		}

		static public void DrawWithRotation(SpriteBatch sp, Texture2D toDraw, Rectangle Bounds, float rotation, SpriteEffects efx)
		{
			Vector2 about = new Vector2(toDraw.Width / 2, toDraw.Height / 2);
			Rectangle source = new Rectangle(0, 0, toDraw.Width, toDraw.Height);
			Bounds.Offset(Bounds.Width / 2.0f, Bounds.Height / 2.0f);
			sp.Draw(toDraw, Bounds, source, Color.White, rotation, about, efx, 1.0f);
		}

		static public float VectAngle(Vector2 input)
		{
			return (float)Math.Atan2(input.Y, input.X);

		}

		static public bool OrthogonallyAdj(Vector2 input1, Vector2 input2)
		{
			Vector2 dir = input1 - input2;

			if (dir.Length() == 1.0f && dir.X * dir.Y == 0.0f)
			{
				return true;
			}

			return false;
		}
	}


}
