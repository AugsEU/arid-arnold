using System.Diagnostics;

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
	/// Cardinal direciton enum
	/// </summary>
	enum CardinalDirection
	{
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3,
	}





	/// <summary>
	/// Random utility functions
	/// </summary>
	internal static class Util
	{
		public static bool mDebugOn = false;

		/// <summary>
		/// Swap two objects
		/// </summary>
		/// <typeparam name="T">Type of objects to swap</typeparam>
		/// <param name="lhs">Object 1</param>
		/// <param name="rhs">Object 2</param>
		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T temp;
			temp = lhs;
			lhs = rhs;
			rhs = temp;
		}



		/// <summary>
		/// Convert cardinal direction enum to unit vector
		/// </summary>
		/// <param name="dir">Cardinal direction</param>
		/// <returns>Cardinal direction unit vector</returns>
		/// <exception cref="NotImplementedException">Requires a valid cardinal direction</exception>
		public static Vector2 GetNormal(CardinalDirection dir)
		{
			switch (dir)
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



		/// <summary>
		/// Get delta T from gameTime object
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		/// <returns>Float in tens of seconds</returns>
		public static float GetDeltaT(GameTime gameTime)
		{
			return (float)gameTime.ElapsedGameTime.TotalSeconds * 10.0f;
		}



		/// <summary>
		/// Swap cardinal direction for it's opposite
		/// </summary>
		/// <param name="dir">Cardinal direction</param>
		/// <returns>Opposite cardinal direction of input</returns>
		/// <exception cref="NotImplementedException">Requires a valid cardinal direction</exception>
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



		/// <summary>
		/// Cross two vectors
		/// </summary>
		/// <param name="a">Vector a</param>
		/// <param name="b">Vector b</param>
		/// <returns>Cross product of a and b</returns>
		public static float Cross(Vector2 a, Vector2 b)
		{
			return a.X * b.Y - a.Y * b.X;
		}




		/// <summary>
		/// Generate a perpendicular to vector
		/// </summary>
		/// <param name="a">Input vector</param>
		/// <returns>Clockwise perpendicular vector</returns>
		public static Vector2 Perpendicular(Vector2 a)
		{
			return new Vector2(a.Y, -a.X);
		}




		/// <summary>
		/// Clamp number between -absLimit and absLimit
		/// </summary>
		/// <param name="toClamp">Number to clamp</param>
		/// <param name="absLimit">Absolute limit of number</param>
		/// <returns>Number in the range [-absLimitm,absLimit]</returns>
		public static float ClampAbs(float toClamp, float absLimit)
		{
			absLimit = Math.Abs(absLimit);
			if (toClamp > absLimit)
			{
				toClamp = absLimit;
			}
			else if (toClamp < -absLimit)
			{
				toClamp = -absLimit;
			}

			return toClamp;
		}



		/// <summary>
		/// Compare colour to hex value. Does not look at alpha
		/// </summary>
		/// <param name="color">Colour to check</param>
		/// <param name="hexCode">Hexcode to check</param>
		/// <returns>True if colour and hexcode match</returns>
		public static bool CompareHEX(Color color, ulong hexCode)
		{
			ulong colourHex = (ulong)(color.B) + ((ulong)(color.G) << 8) + +((ulong)(color.R) << 16);

			return colourHex == hexCode;
		}



		/// <summary>
		/// Draw a string centred at a position
		/// </summary>
		/// <param name="sb">Sprite batch</param>
		/// <param name="font">Font to draw text in</param>
		/// <param name="position">Centre position</param>
		/// <param name="color">Colour of text</param>
		/// <param name="text">Text to draw</param>
		public static void DrawStringCentred(SpriteBatch sb, SpriteFont font, Vector2 position, Color color, string text)
		{
			Vector2 size = font.MeasureString(text);

			sb.DrawString(font, text, position - size / 2, color);
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



		/// <summary>
		/// Brighten colour linearly
		/// </summary>
		/// <param name="col">Initial colour, output value</param>
		/// <param name="bright">Brightness factor from 0 to 1</param>
		public static void BrightenColour(ref Color col, float bright)
		{
			col.R = (byte)((col.R * (1 - bright)) + (255 * bright));
			col.G = (byte)((col.G * (1 - bright)) + (255 * bright));
			col.B = (byte)((col.B * (1 - bright)) + (255 * bright));
		}



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
		/// Log message to console. Only if debug is on.
		/// </summary>
		/// <param name="msg">Message to log</param>
		public static void Log(string msg)
		{
			if (mDebugOn)
			{
				Debug.WriteLine(msg);
			}
		}



		/// <summary>
		/// Log a message to console.
		/// </summary>
		/// <param name="msg">Message to log</param>
		public static void DLog(string msg)
		{
			Debug.WriteLine(msg);
		}
	}


}
