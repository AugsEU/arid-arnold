﻿namespace AridArnold
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
	/// Game specific utility functions
	/// </summary>
	internal static class Util
	{
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
		/// Convert cardinal direction enum to unit point
		/// </summary>
		/// <param name="dir">Cardinal direction</param>
		/// <returns>Cardinal direction unit vector</returns>
		/// <exception cref="NotImplementedException">Requires a valid cardinal direction</exception>
		public static Point GetNormalPoint(CardinalDirection dir)
		{
			switch (dir)
			{
				case CardinalDirection.Up:
					return new Point(0, -1);
				case CardinalDirection.Down:
					return new Point(0, 1);
				case CardinalDirection.Left:
					return new Point(-1, 0);
				case CardinalDirection.Right:
					return new Point(1, 0);
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Gets angle from cardinal direction
		/// </summary>
		public static float GetRotation(CardinalDirection dir)
		{
			switch (dir)
			{
				case CardinalDirection.Up:
					return 0.0f;
				case CardinalDirection.Right:
					return MathHelper.PiOver2;
				case CardinalDirection.Down:
					return MathHelper.Pi;
				case CardinalDirection.Left:
					return MathHelper.PiOver2 * 3.0f;
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
		/// Invert walking direction to opposite
		/// </summary>
		public static WalkDirection InvertDirection(WalkDirection dir)
		{
			switch (dir)
			{
				case WalkDirection.Left:
					return WalkDirection.Right;
				case WalkDirection.Right:
					return WalkDirection.Left;
				case WalkDirection.None:
					return WalkDirection.None;
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Convert a walk direction to a cardinal direction
		/// </summary>
		public static CardinalDirection WalkDirectionToCardinal(WalkDirection walk, CardinalDirection gravity)
		{
			switch (gravity)
			{
				case CardinalDirection.Up:
					return walk == WalkDirection.Right ? CardinalDirection.Right : CardinalDirection.Left;
				case CardinalDirection.Right:
					return walk == WalkDirection.Right ? CardinalDirection.Down : CardinalDirection.Up;
				case CardinalDirection.Down:
					return walk == WalkDirection.Right ? CardinalDirection.Right : CardinalDirection.Left;
				case CardinalDirection.Left:
					return walk == WalkDirection.Right ? CardinalDirection.Down : CardinalDirection.Up;
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Get walk direction from cardinal direction
		/// </summary>
		public static WalkDirection CardinalToWalkDirection(CardinalDirection card, CardinalDirection gravity)
		{
			switch (gravity)
			{
				case CardinalDirection.Up:
				case CardinalDirection.Down:
					return card == CardinalDirection.Left ? WalkDirection.Left : WalkDirection.Right;
				case CardinalDirection.Right:
				case CardinalDirection.Left:
					return card == CardinalDirection.Up ? WalkDirection.Left : WalkDirection.Right;
			}

			throw new NotImplementedException();
		}


		/// <summary>
		/// Round angle to cardinal direction
		/// </summary>
		public static CardinalDirection CardinalDirectionFromAngle(float angle)
		{
			angle = MonoMath.MainBranchRadian(angle);
			float PI8 = MathF.PI / 4.0f;

			if (angle < PI8 || angle > 7.0f * PI8)
			{
				return CardinalDirection.Up;
			}
			else if (angle < 3.0f * PI8)
			{
				return CardinalDirection.Left;
			}
			else if (angle < 5.0f * PI8)
			{
				return CardinalDirection.Down;
			}

			return CardinalDirection.Right;
		}



		/// <summary>
		/// Card from a vector.
		/// </summary>
		public static CardinalDirection CardinalDirectionFromVector(Vector2 vector)
		{
			if (vector.Y < vector.X)
			{
				return -vector.Y < vector.X ? CardinalDirection.Right : CardinalDirection.Up;
			}

			return -vector.Y < vector.X ? CardinalDirection.Down : CardinalDirection.Left;
		}



		/// <summary>
		/// Reflect card by a reflection normal
		/// </summary>
		public static CardinalDirection ReflectCardinalDirection(CardinalDirection direction, Vector2 normal)
		{
			Vector2 cardVec = GetNormal(direction);
			cardVec = MonoMath.Reflect(cardVec, normal);
			return CardinalDirectionFromVector(cardVec);
		}



		/// <summary>
		/// Converts level ID into hyphen notation.
		/// </summary>
		public static string LevelIDToString(int id)
		{
			string idStr = id.ToString();

			string firstPart = idStr.Substring(0, idStr.Length - 2).TrimStart('0');
			string lastPart = idStr.Substring(idStr.Length - 2).TrimStart('0');

			// Combine with a hyphen
			return $"{firstPart}-{lastPart}";
		}
	}
}
