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

		//Define layer depths of objects.
		public const float LAYER_DEFAULT = 0.1f;

		public const float LAYER_TILE = 0.29f;

		public const float LAYER_TEXT_BOX = 0.37f;
		public const float LAYER_TEXT_SHADOW = 0.38f;
		public const float LAYER_TEXT = 0.39f;
		public const float LAYER_TOP = 1.00f;

		//Add this constant to something to make it slightly infront of something within the same "layer".
		public const float FRONT_EPSILON = 0.0001f;
		public const float BACK_EPSILON = -FRONT_EPSILON;

		const float LAYER_MOVE_EPSILON = 0.0000001f;

		static float sCurrentLayerDelta = 0.0f;

		#endregion rConstants





		#region rRender

		/// <summary>
		/// Draw a texture at a position.
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, IncrementLayer(LAYER_DEFAULT));
		}



		/// <summary>
		/// Draw a texture at to rect.
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Rectangle rect)
		{
			info.spriteBatch.Draw(texture2D, rect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, IncrementLayer(LAYER_DEFAULT));
		}



		/// <summary>
		/// Draw a texture at position(rotated about the centre).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, float rotation)
		{
			Vector2 rotationOffset = CalcRotationOffset(rotation, texture2D.Width, texture2D.Height);
			info.spriteBatch.Draw(texture2D, new Rectangle((int)position.X, (int)position.Y, texture2D.Width, texture2D.Height), null, Color.White, rotation, rotationOffset, SpriteEffects.None, IncrementLayer(LAYER_DEFAULT));
		}



		/// <summary>
		/// Draw a texture at position(with effect).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, SpriteEffects effect)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, IncrementLayer(LAYER_DEFAULT));
		}



		/// <summary>
		/// Draw a texture at a position(with layer depth).
		/// </summary>
		public static void DrawTextureDepth(DrawInfo info, Texture2D texture2D, Vector2 position, float depth)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, IncrementLayer(depth));
		}

		/// <summary>
		/// Draw a texture at a position(all options).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, Rectangle? sourceRectange, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
		{
			info.spriteBatch.Draw(texture2D, position, sourceRectange, color, rotation, origin, scale, effect, IncrementLayer(depth));
		}


		/// <summary>
		/// Draw a texture to a rect(all options).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Rectangle destRect, Rectangle? sourceRectange, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
		{
			info.spriteBatch.Draw(texture2D, destRect, sourceRectange, color, rotation, origin, effect, IncrementLayer(depth));
		}


		/// <summary>
		/// Draw a texture to a rect(all options).
		/// </summary>
		public static void DrawPlatformer(DrawInfo info, Rect2f collider, Texture2D texture2D, Vector2 position, Color color, CardinalDirection gravityDir, WalkDirection walkDir, float depth)
		{
			Vector2 drawOffset = Vector2.Zero;

			float rotation = 0.0f;
			SpriteEffects effect = SpriteEffects.None;

			if (walkDir == WalkDirection.Left)
			{
				effect = SpriteEffects.FlipHorizontally;
			}

			switch (gravityDir)
			{
				case CardinalDirection.Down:
					drawOffset.X = (collider.Width - texture2D.Width) / 2.0f;
					drawOffset.Y = collider.Height - texture2D.Height;

					rotation = 0.0f;
					break;
				case CardinalDirection.Up:
					rotation = MathHelper.Pi;

					drawOffset.X = (collider.Width + texture2D.Width) / 2.0f;
					drawOffset.Y = texture2D.Height;
					
					effect = effect ^ SpriteEffects.FlipHorizontally;
					break;
				case CardinalDirection.Left:
					drawOffset.X = texture2D.Height;

					rotation = MathHelper.PiOver2;
					break;
				case CardinalDirection.Right:
					rotation = MathHelper.PiOver2 * 3.0f;

					drawOffset.X = collider.Width - texture2D.Height;
					drawOffset.Y = texture2D.Width;

					effect = effect ^ SpriteEffects.FlipHorizontally;
					break;
			}

			Vector2 drawPosition = position + drawOffset;

			drawPosition.X = MathF.Round(drawPosition.X);
			drawPosition.Y = MathF.Round(drawPosition.Y);

			MonoDraw.DrawTexture(info, texture2D, drawPosition, null, color, rotation, Vector2.Zero, 1.0f, effect, depth);
		}



		/// <summary>
		/// Draw a string centred at a position
		/// </summary>
		public static void DrawStringCentred(DrawInfo info, SpriteFont font, Vector2 position, Color color, string text, float depth)
		{
			Vector2 size = font.MeasureString(text);

			info.spriteBatch.DrawString(font, text, position - size / 2, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, IncrementLayer(depth));
		}


		/// <summary>
		/// Draw a string at a position
		/// </summary>
		public static void DrawString(DrawInfo info, SpriteFont font, string text, Vector2 position, Color color, float depth)
		{
			info.spriteBatch.DrawString(font, text, position, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, IncrementLayer(depth));
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

			DrawRect(info, new Rectangle(min, max - min), col);
		}



		/// <summary>
		/// Draw a simple rectangle. Used mostly for debugging
		/// </summary>
		public static void DrawRect(DrawInfo info, Rectangle rect, Color col)
		{
			info.spriteBatch.Draw(Main.GetDummyTexture(), rect, col);
		}


		/// <summary>
		/// Draw a simple rectangle. Used mostly for debugging
		/// </summary>
		public static void DrawRectDepth(DrawInfo info, Rectangle rect, Color col, float depth)
		{
			info.spriteBatch.Draw(Main.GetDummyTexture(), rect, null, col, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
		}



		/// <summary>
		/// Draw a dot at a position.
		/// </summary>
		public static void DrawDot(DrawInfo info, Vector2 point, Color col)
		{
			DrawDot(info, new Point((int)point.X, (int)point.Y), col);
		}



		/// <summary>
		/// Draw a dot at a position
		/// </summary>
		public static void DrawDot(DrawInfo info, Point point, Color col)
		{
			Rectangle rectangle = new Rectangle(point.X, point.Y, 1, 1);
			DrawRect(info, rectangle, col);
		}



		/// <summary>
		/// Draw a dot at a position.
		/// </summary>
		public static void DrawDotDepth(DrawInfo info, Vector2 point, Color col, float depth)
		{
			DrawDotDepth(info, new Point((int)point.X, (int)point.Y), col, depth);
		}



		/// <summary>
		/// Draw a dot at a position
		/// </summary>
		public static void DrawDotDepth(DrawInfo info, Point point, Color col, float depth)
		{
			Rectangle rectangle = new Rectangle(point.X, point.Y, 1, 1);
			DrawRectDepth(info, rectangle, col, depth);
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


		/// <summary>
		/// Increment layer to avoid z-fighting.
		/// </summary>
		public static float IncrementLayer(float baseLayer)
		{
			sCurrentLayerDelta += LAYER_MOVE_EPSILON;
			return baseLayer + sCurrentLayerDelta;
		}


		/// <summary>
		/// Call this to clear out each frame.
		/// </summary>
		public static void FlushRender()
		{
			sCurrentLayerDelta = 0.0f;
		}

		#endregion rUtility
	}
}
