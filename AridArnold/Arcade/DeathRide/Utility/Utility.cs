namespace GMTK2023
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

	enum EightDirection
	{
		UpLeft = 0,
		Up,
		UpRight,
		Left,
		Right,
		DownLeft,
		Down,
		DownRight
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


		public static EightDirection GetDirectionFromAngle(float angle)
		{
			angle = MonoMath.MainBranchRadian(angle);

			if (angle < (1.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.Right;
			}
			else if (angle < (2.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.UpRight;
			}
			else if (angle < (3.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.Up;
			}
			else if (angle < (4.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.UpLeft;
			}
			else if (angle < (5.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.Left;
			}
			else if (angle < (6.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.DownLeft;
			}
			else if (angle < (7.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.Down;
			}
			else if (angle < (8.0f * MathF.PI / 4.0f) - MathF.PI / 8.0f)
			{
				return EightDirection.DownRight;
			}

			return EightDirection.Right;
		}

		public static float GetAngleFromDirection(EightDirection dir)
		{
			switch (dir)
			{
				case EightDirection.UpLeft:
					return MathF.PI * 0.75f;
				case EightDirection.Up:
					return MathF.PI / 2.0f;
				case EightDirection.UpRight:
					return MathF.PI / 4.0f;
				case EightDirection.Left:
					return MathF.PI;
				case EightDirection.Right:
					return 0.0f;
				case EightDirection.DownLeft:
					return MathF.PI * 1.25f;
				case EightDirection.Down:
					return MathF.PI * 1.5f;
				case EightDirection.DownRight:
					return MathF.PI * 1.75f;
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
	}
}
