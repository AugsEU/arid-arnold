﻿namespace AridArnold
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
		/// Rotate vector clockwise
		/// </summary>
		public static Vector2 Rotate(Vector2 a, float angle)
		{
			float s = MathF.Sin(angle);
			float c = MathF.Cos(angle);
			return new Vector2(c * a.X - s * a.Y, s * a.X + c * a.Y);
		}



		/// <summary>
		/// Lerp two vectors
		/// </summary>
		public static Vector2 Lerp(Vector2 p1, Vector2 p2, float t)
		{
			return (p1) * (1.0f - t) + (p2) * t;
		}



		public static Vector3 ToVec3(Vector2 vec)
		{
			return new Vector3(vec, 0.0f);
		}


		/// <summary>
		/// Gets diff in direction between two vectors. 0 is no diff, 1 is max diff, 0.5 is perp
		/// </summary>
		public static float VectorDiff(Vector2 p1, Vector2 p2)
		{
			p1.Normalize();
			p2.Normalize();

			return 0.5f * (1.0f - Vector2.Dot(p1, p2));
		}



		/// <summary>
		/// Lerp two colours
		/// </summary>
		public static Color Lerp(Color p1, Color p2, float t)
		{
			float R = p1.R * (1.0f - t) + p2.R * t;
			float G = p1.G * (1.0f - t) + p2.G * t;
			float B = p1.B * (1.0f - t) + p2.B * t;

			R = Math.Clamp(R, 0.0f, 255.0f);
			G = Math.Clamp(G, 0.0f, 255.0f);
			B = Math.Clamp(B, 0.0f, 255.0f);

			return new Color((byte)R, (byte)G, (byte)B);
		}


		/// <summary>
		/// Rotate vector clockwise
		/// </summary>
		public static Vector2 RotateDeg(Vector2 a, float angle)
		{
			return Rotate(a, DegToRad(angle));
		}


		/// <summary>
		/// Rotate vector clockwise
		/// </summary>
		public static float RadToDeg(float angle)
		{
			return (angle / (MathF.PI * 2.0f)) * 360.0f;
		}


		/// <summary>
		/// Rotate vector clockwise
		/// </summary>
		public static float DegToRad(float angle)
		{
			return (angle / (360.0f)) * MathF.PI * 2.0f;
		}


		/// <summary>
		/// Turn radian into 0 -> 2PI range
		/// </summary>
		public static float MainBranchRadian(float angle)
		{
			while (angle <= 0.0f)
			{
				angle += MathF.PI * 2.0f;
			}
			return angle % (MathF.PI * 2.0f);
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
		/// Sine-wave like function that goes from 0 to 1 and repeats every unit. Starts at 0
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float UnitWave(float t)
		{
			t *= 4.0f;
			t += 1.0f;
			t %= 4.0f;
			t -= 2.0f;
			t *= 2.0f - MathF.Abs(t);
			t += 1.0f;
			t *= 0.5f;
			return t;
		}



		/// <summary>
		/// Function that goes smoothly from 0 to 1
		/// </summary>
		public static float SmoothZeroToOne(float t)
		{
			t *= 2.0f;
			t -= 1.0f;
			t *= 2.0f - MathF.Abs(t);
			t += 1.0f;
			t *= 0.5f;
			return t;
		}


		/// <summary>
		/// Goes from 0 to 1, but starts sharp.
		/// </summary>
		public static float LeapZeroToSmoothOne(float t)
		{
			t -= 1.0f;
			t *= -t;
			t += 1.0f;
			return t;
		}



		/// <summary>
		/// Skews range to lower end, such as with an exponential curve
		/// </summary>
		public static float FakeExpSquash(float min, float max, float t)
		{
			t -= min;
			t /= (min - max);
			t *= t;
			return min * (1.0f - t) + max * t;
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


		/// <summary>
		/// Counts the number of bits which are set
		/// </summary>
		public static UInt32 BitCountI32(UInt32 i)
		{
			i = i - ((i >> 1) & 0x55555555);                // add pairs of bits
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333); // quads
			i = (i + (i >> 4)) & 0x0F0F0F0F;                // groups of 8
			return (i * 0x01010101) >> 24;                  // horizontal sum of bytes
		}



		/// <summary>
		/// Get adjacent points.
		/// </summary>
		public static List<Point> GetAdjacentPoints(Point point)
		{
			List<Point> returnVal = new List<Point>();
			returnVal.Add(new Point(point.X + 1, point.Y));
			returnVal.Add(new Point(point.X - 1, point.Y));
			returnVal.Add(new Point(point.X, point.Y + 1));
			returnVal.Add(new Point(point.X, point.Y - 1));
			return returnVal;
		}


		/// <summary>
		/// Get digits of a number as a list
		/// </summary>
		public static int[] GetDigits(int num)
		{
			if (num == 0)
			{
				return new int[] { 0 };
			}

			int count = (int)Math.Log10(num) + 1;
			int[] digits = new int[count];

			// Extract digits
			for (int i = count - 1; i >= 0; --i)
			{
				digits[i] = num % 10;
				num /= 10;
			}

			return digits;
		}



		/// <summary>
		/// Create a square centered at a location
		/// </summary>
		public static Rectangle SquareCenteredAt(Vector2 centre, float side)
		{
			int iS = (int)MathF.Round(side);
			side *= 0.5f;

			Vector2 TL = centre;
			if (side > 1.0f)
			{
				TL.X -= side;
				TL.Y -= side;
			}

			int iX = (int)MathF.Round(TL.X);
			int iY = (int)MathF.Round(TL.Y);

			return new Rectangle(iX, iY, iS, iS);
		}



		/// <summary>
		/// Reflect vector along normal
		/// </summary>
		public static Vector2 Reflect(Vector2 vec, Vector2 normal)
		{
			return vec - 2.0f * Vector2.Dot(vec, normal) * normal;
		}



		/// <summary>
		/// Reflect vector along normal and centre of reflection
		/// </summary>
		public static Vector2 Reflect(Vector2 vec, Vector2 normal, Vector2 centre)
		{
			vec -= centre;
			return vec - 2.0f * Vector2.Dot(vec, normal) * normal + centre;
		}



		/// <summary>
		/// Multiply vector components
		/// </summary>
		public static Vector2 CompMult(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X * b.X, a.Y * b.Y);
		}



		/// <summary>
		/// Truncate annoying float stuff
		/// </summary>
		public static Vector2 TruncateSmall(Vector2 a)
		{
			const float THRESH = 0.000001f;
			if(-THRESH < a.X && a.X < THRESH)
			{
				a.X = 0.0f;
			}
			else if(-THRESH < a.Y && a.Y < THRESH)
			{
				a.Y = 0.0f;
			}

			return a;
		}
	}
}
