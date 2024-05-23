using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WormWarp
{
	class Uptext
	{
		const float UP = -12.1f;
		const float decay = 1.7f;

		string mString;
		float opacity;

		Color textColor;
		Vector2 position;



		public Uptext(Vector2 start, string text, Color col)
		{
			mString = text;
			position = start;
			textColor = col;
			opacity = 1.0f;
		}

		public bool deleteMe()
		{
			return opacity == 0.0f;
		}

		public void Update(GameTime gameTime)
		{
			float secFraq = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

			position += new Vector2(0.0f, UP * secFraq);

			opacity = Math.Max(opacity - decay * secFraq, 0.0f);
		}

		public void Draw(ref SpriteBatch spriteBatch, ref SpriteFont font)
		{
			DrawString(ref spriteBatch, ref font, mString, position, textColor, 0.4f, true);
		}

		void DrawString(ref SpriteBatch spriteBatch, ref SpriteFont font, string text, Vector2 pos, Color color, float scale, bool center)
		{
			if (center)
			{
				Vector2 stringSize = font.MeasureString(text) * 0.5f;

				spriteBatch.DrawString(font, text, pos - (stringSize * scale), color * opacity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
			}
			else
			{
				spriteBatch.DrawString(font, text, pos, color * opacity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
			}
		}
	}
}
