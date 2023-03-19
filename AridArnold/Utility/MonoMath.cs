namespace AridArnold
{
	/// <summary>
	/// Math utility functions
	/// </summary>
	static class MonoMath
	{
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
			if (value > 0.0f)
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
		/// Round a float to an int.
		/// </summary>
		public static Vector2 Round(Vector2 v)
		{
			return new Vector2(MathF.Round(v.X), MathF.Round(v.Y));
		}
	}
}
