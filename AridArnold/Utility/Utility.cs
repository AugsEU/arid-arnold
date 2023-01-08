using System.Diagnostics;

namespace AridArnold
{
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
		/// Finds the maximum in a list.
		/// </summary>
		public static T GetMax<T>(ref List<T> list, IComparer<T> comparer)
		{
			T maxValue = list[0];

			for(int i = 1; i < list.Count; i++)
			{
				T temp = list[i];
				if(comparer.Compare(temp, maxValue) > 0)
				{
					maxValue = temp;
				}
			}

			return maxValue;
		}



		/// <summary>
		/// Finds the maximum in a list.
		/// </summary>
		public static T GetMin<T>(ref List<T> list, IComparer<T> comparer)
		{
			T minValue = list[0];

			for (int i = 1; i < list.Count; i++)
			{
				T temp = list[i];
				if (comparer.Compare(temp, minValue) < 0)
				{
					minValue = temp;
				}
			}

			return minValue;
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
		/// Biject float into a fixed region.
		/// </summary>
		public static float SquashToRange(float value, float min, float max)
		{
			if(value > 0.0f)
			{
				value = 1.0f - 1.0f / (value + 2.0f);
			}
			else
			{
				value = -1.0f / (value - 2.0f);
			}

			return (max - min) * value + min;
		}


		/// <summary>
		/// Round a float to an int.
		/// </summary>
		public static int Round(float f)
		{
			return (int)MathF.Round(f);
		}



		/// <summary>
		/// Get uint hex code from color.
		/// </summary>
		public static uint ColorToHEX(Color color)
		{
			return color.B + ((uint)(color.G) << 8) + +((uint)(color.R) << 16);
		}



		/// <summary>
		/// Get color from string hex code.
		/// </summary>
		public static Color HEXToColor(string hex)
		{
			byte r, g, b, a;

			r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			a = 255;

			return new Color(r, g, b, a);
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
		/// Returns true if the letter is a vowel.
		/// </summary>
		public static bool IsVowel(char character)
		{
			switch (char.ToLower(character))
			{
				case 'a':
				case 'e':
				case 'i':
				case 'o':
				case 'u':
				case 'y':
					return true;
			}

			return false;
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
