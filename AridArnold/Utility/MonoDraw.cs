namespace AridArnold
{
	/// <summary>
	/// Info needed to draw
	/// </summary>
	struct DrawInfo
	{
		public GameTime gameTime;
		public SpriteBatch spriteBatch;
		public GraphicsDeviceManager graphics;
		public GraphicsDevice device;
	}

	/// <summary>
	/// Simple rendering methods.
	/// </summary>
	static class MonoDraw
	{
		#region rConstants

		public const float LAYER_DEFAULT = 0.0f;
		
		public const float LAYER_TILE = 0.19f;

		public const float LAYER_TEXT_BOX = 0.27f;
		public const float LAYER_TEXT_SHADOW = 0.28f;
		public const float LAYER_TEXT = 0.29f;

		#endregion rConstants





		#region rRender

		/// <summary>
		/// Draw a texture at a position.
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, LAYER_DEFAULT);
		}



		/// <summary>
		/// Draw a texture at to rect.
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Rectangle rect)
		{
			info.spriteBatch.Draw(texture2D, rect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, LAYER_DEFAULT);
		}



		/// <summary>
		/// Draw a texture at position(rotated about the centre).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, float rotation)
		{
			Vector2 rotationOffset = CalcRotationOffset(rotation, texture2D.Width, texture2D.Height);
			info.spriteBatch.Draw(texture2D, new Rectangle((int)position.X, (int)position.Y, texture2D.Width, texture2D.Height), null, Color.White, rotation, rotationOffset, SpriteEffects.None, LAYER_DEFAULT);
		}



		/// <summary>
		/// Draw a texture at position(with effect).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, SpriteEffects effect)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, LAYER_DEFAULT);
		}



		/// <summary>
		/// Draw a texture at a position(with layer depth).
		/// </summary>
		public static void DrawTextureDepth(DrawInfo info, Texture2D texture2D, Vector2 position, float depth)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
		}

		/// <summary>
		/// Draw a texture at a position(all options).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, Rectangle? sourceRectange, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
		{
			info.spriteBatch.Draw(texture2D, position, sourceRectange, color, rotation, origin, scale, effect, depth);
		}


		/// <summary>
		/// Draw a texture to a rect(all options).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Rectangle destRect, Rectangle? sourceRectange, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
		{
			info.spriteBatch.Draw(texture2D, destRect, sourceRectange, color, rotation, origin, effect, depth);
		}



		/// <summary>
		/// Draw a string centred at a position
		/// </summary>
		public static void DrawStringCentred(DrawInfo info, SpriteFont font, Vector2 position, Color color, string text, float depth)
		{
			Vector2 size = font.MeasureString(text);

			info.spriteBatch.DrawString(font, text, position - size / 2, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
		}


		/// <summary>
		/// Draw a string at a position
		/// </summary>
		public static void DrawString(DrawInfo info, SpriteFont font, string text, Vector2 position, Color color, float depth)
		{
			info.spriteBatch.DrawString(font, text, position, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
		}



		/// <summary>
		/// Draw a simple rectangle. Used mostly for debugging
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <param name="rect2f">Rectangle to draw</param>
		/// <param name="col">Color to draw rectangle in</param>
		public static void DrawRect(DrawInfo info, Rect2f rect2f, Color col)
		{
			Point min = new Point((int)rect2f.min.X, (int)rect2f.min.Y);
			Point max = new Point((int)rect2f.max.X, (int)rect2f.max.Y);

			DrawRect(info, new Rectangle(min, max), col);
		}



		/// <summary>
		/// Draw a simple rectangle. Used mostly for debugging
		/// </summary>
		public static void DrawRect(DrawInfo info, Rectangle rect, Color col)
		{
			DrawTexture(info, Main.GetDummyTexture(), rect);
		}


		/// <summary>
		/// Draw a simple rectangle. Used mostly for debugging
		/// </summary>
		public static void DrawRectDepth(DrawInfo info, Rectangle rect, Color col, float depth)
		{
			info.spriteBatch.Draw(Main.GetDummyTexture(), rect, null, col, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
		}

		#endregion rRender





		#region rUtility

		/// <summary>
		/// Calculate rotation offset so we can rotate about the centre. For squares
		/// </summary>
		/// <param name="rotation">Rotation in rads</param>
		/// <param name="height">Square height</param>
		/// <returns>Rotation offset used in draw call</returns>
		public static Vector2 CalcRotationOffset(float rotation, float height)
		{
			return CalcRotationOffset(rotation, height, height);
		}



		/// <summary>
		/// Calculate rotation offset so we can rotate about the centre. For rectangles
		/// </summary>
		/// <param name="rotation">Rotation in rads</param>
		/// <param name="width">Rectangle width</param>
		/// <param name="height">Rectangle height</param>
		/// <returns>Rotation offset used in draw call</returns>
		public static Vector2 CalcRotationOffset(float rotation, float width, float height)
		{
			float c = MathF.Cos(rotation);
			float s = MathF.Sin(-rotation);

			Vector2 oldCentre = new Vector2(width / 2.0f, height / 2.0f);
			Vector2 newCentre = new Vector2(oldCentre.X * c - oldCentre.Y * s, oldCentre.X * s + oldCentre.Y * c);

			return oldCentre - newCentre;
		}

		#endregion rUtility
	}
}
