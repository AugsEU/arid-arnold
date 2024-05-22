namespace GMTK2023
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

	enum DrawLayer
	{
		Background = 0,
		BackgroundElement,
		SubEntity,
		Default,
		Player,
		Bullets,
		Text,
		Front,
		NumLayers
	}

	/// <summary>
	/// Simple rendering methods.
	/// </summary>
	static class MonoDraw
	{
		#region rConstants

		const float TOTAL_LAYERS = (float)DrawLayer.NumLayers;
		const float LAYER_MOVE_EPSILON = 0.000001f;

		static float sCurrentLayerDelta = 0.0f;

		#endregion rConstants





		#region rRender

		/// <summary>
		/// Draw a texture at a position.
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(DrawLayer.Default));
		}



		/// <summary>
		/// Draw a texture at to rect.
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Rectangle rect)
		{
			info.spriteBatch.Draw(texture2D, rect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, GetDepth(DrawLayer.Default));
		}





		/// <summary>
		/// Draw a texture at position(with effect).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, SpriteEffects effect)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, GetDepth(DrawLayer.Default));
		}


		/// <summary>
		/// Draw a texture at position(rotated about the centre).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, float rotation)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(DrawLayer.Default));
		}


		/// <summary>
		/// Draw a texture at position(rotated about the centre).
		/// </summary>
		public static void DrawTextureRotCent(DrawInfo info, Texture2D texture2D, Vector2 position, float rotation)
		{
			Vector2 rotationOffset = CalcRotationOffset(rotation, texture2D.Width, texture2D.Height);
			info.spriteBatch.Draw(texture2D, new Rectangle((int)position.X, (int)position.Y, texture2D.Width, texture2D.Height), null, Color.White, rotation, rotationOffset, SpriteEffects.None, GetDepth(DrawLayer.Default));
		}





		/// <summary>
		/// Draw a texture at a position(with layer depth).
		/// </summary>
		public static void DrawTextureDepth(DrawInfo info, Texture2D texture2D, Vector2 position, DrawLayer depth)
		{
			info.spriteBatch.Draw(texture2D, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(depth));
		}



		/// <summary>
		/// Draw a texture at a position(with layer depth + color).
		/// </summary>
		public static void DrawTextureDepthColor(DrawInfo info, Texture2D texture2D, Vector2 position, Color color, DrawLayer depth)
		{
			info.spriteBatch.Draw(texture2D, position, null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(depth));
		}



		/// <summary>
		/// Draw a texture at a position(all options).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Vector2 position, Rectangle? sourceRectange, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, DrawLayer depth)
		{
			info.spriteBatch.Draw(texture2D, position, sourceRectange, color, rotation, origin, scale, effect, GetDepth(depth));
		}


		/// <summary>
		/// Draw a texture to a rect(all options).
		/// </summary>
		public static void DrawTexture(DrawInfo info, Texture2D texture2D, Rectangle destRect, Rectangle? sourceRectange, Color color, float rotation, Vector2 origin, SpriteEffects effect, DrawLayer depth)
		{
			info.spriteBatch.Draw(texture2D, destRect, sourceRectange, color, rotation, origin, effect, GetDepth(depth));
		}



		/// <summary>
		/// Draw a string centred at a position
		/// </summary>
		public static void DrawStringCentred(DrawInfo info, SpriteFont font, Vector2 position, Color color, string text, DrawLayer depth = DrawLayer.Text)
		{
			Vector2 size = font.MeasureString(text);

			info.spriteBatch.DrawString(font, text, position - size / 2, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(depth));
		}


		/// <summary>
		/// Draw a string centred at a position
		/// </summary>
		public static void DrawShadowStringCentred(DrawInfo info, SpriteFont font, Vector2 position, Color color, string text, DrawLayer depth = DrawLayer.Text)
		{
			Vector2 size = font.MeasureString(text);

			Color dropColor = color;
			MonoColor.DarkenColour(ref dropColor, 0.1f);

			Vector2 pos = position - size / 2;
			Vector2 dropPos = pos + new Vector2(2.0f, 2.0f);

			info.spriteBatch.DrawString(font, text, dropPos, dropColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(depth));
			info.spriteBatch.DrawString(font, text, pos, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(depth));
		}

		/// <summary>
		/// Draw a string at position(top left)
		/// </summary>
		public static void DrawString(DrawInfo info, SpriteFont font, Vector2 position, Color color, string text, DrawLayer depth = DrawLayer.Text)
		{
			info.spriteBatch.DrawString(font, text, position, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(depth));
		}


		/// <summary>
		/// Draw a string at a position
		/// </summary>
		public static void DrawString(DrawInfo info, SpriteFont font, string text, Vector2 position, Color color, DrawLayer depth)
		{
			info.spriteBatch.DrawString(font, text, position, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth(depth));
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
			info.spriteBatch.Draw(GMTK2023.GetDummyTexture(), rect, col);
		}


		/// <summary>
		/// Draw a simple rectangle. Used mostly for debugging
		/// </summary>
		public static void DrawRectDepth(DrawInfo info, Rectangle rect, Color col, DrawLayer depth)
		{
			info.spriteBatch.Draw(GMTK2023.GetDummyTexture(), rect, null, col, 0.0f, Vector2.Zero, SpriteEffects.None, GetDepth(depth));
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
		public static void DrawDotDepth(DrawInfo info, Vector2 point, Color col, DrawLayer depth)
		{
			DrawDotDepth(info, new Point((int)point.X, (int)point.Y), col, depth);
		}



		/// <summary>
		/// Draw a dot at a position
		/// </summary>
		public static void DrawDotDepth(DrawInfo info, Point point, Color col, DrawLayer depth)
		{
			Rectangle rectangle = new Rectangle(point.X, point.Y, 1, 1);
			DrawRectDepth(info, rectangle, col, depth);
		}



		/// <summary>
		/// Draw a line from point A to B
		/// </summary>
		public static void DrawLine(DrawInfo info, Vector2 point1, Vector2 point2, Color color, float thickness = 1.0f, DrawLayer depth = DrawLayer.Default)
		{
			float distance = Vector2.Distance(point1, point2);
			float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
			DrawLine(info, point1, distance, angle, color, thickness, depth);
		}



		/// <summary>
		/// Draw a line from point A by an angle.
		/// </summary>
		public static void DrawLine(DrawInfo info, Vector2 point, float length, float angle, Color color, float thickness = 1.0f, DrawLayer depth = DrawLayer.Default)
		{
			var origin = new Vector2(0f, 0.5f);
			var scale = new Vector2(length, thickness);
			info.spriteBatch.Draw(GMTK2023.GetDummyTexture(), point, null, color, angle, origin, scale, SpriteEffects.None, GetDepth(depth));
		}



		/// <summary>
		/// Draw a line with drop shadow.
		/// </summary>
		public static void DrawLineShadow(DrawInfo info, Vector2 point1, Vector2 point2, Color color, Color shadowColor, float dropDistance, float thickness = 1.0f, DrawLayer depth = DrawLayer.Default)
		{
			Vector2 shadowPt1 = point1;
			Vector2 shadowPt2 = point2;

			shadowPt1.Y += dropDistance;
			shadowPt2.Y += dropDistance;

			DrawLine(info, shadowPt1, shadowPt2, shadowColor, thickness, depth);
			DrawLine(info, point1, point2, color, thickness, depth);
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
		/// Get draw layer from string. Exception if not a valid name.
		/// </summary>
		static public DrawLayer GetDrawLayer(string drawLayer)
		{
			return MonoAlg.GetEnumFromString<DrawLayer>(drawLayer);
		}


		/// <summary>
		/// Increment layer to avoid z-fighting.
		/// </summary>
		static float GetDepth(DrawLayer layer)
		{
			float layerNum = (float)layer;

			sCurrentLayerDelta += LAYER_MOVE_EPSILON;
			return layerNum / TOTAL_LAYERS + sCurrentLayerDelta;
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
