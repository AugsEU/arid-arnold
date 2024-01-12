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
					return walk == WalkDirection.Right ? CardinalDirection.Left : CardinalDirection.Right;
				case CardinalDirection.Right:
					return walk == WalkDirection.Right ? CardinalDirection.Up : CardinalDirection.Down;
				case CardinalDirection.Down:
					return walk == WalkDirection.Right ? CardinalDirection.Right : CardinalDirection.Left;
				case CardinalDirection.Left:
					return walk == WalkDirection.Right ? CardinalDirection.Down : CardinalDirection.Up;
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
	}
}
