namespace WormWarp
{
	enum AppleType
	{
		AT_NORMAL,
		AT_DIMENSION
	}

	enum UpdateResult
	{
		UR_NORMAL,
		UR_NEW_DIMENSION,
		UR_APPLE_EATEN,
		UR_DEAD
	}

	struct Apple
	{
		public AppleType type;
		public Vector2 position;
		public bool active;

		public Apple(AppleType t, Vector2 p)
		{
			type = t;
			position = p;
			active = true;
		}
	}

	class SnakeGame
	{
		const float INTERP_SPEED = 0.25f;

		public Vector2 CurrPos;
		int CurrWidth;
		int CurrHeight;

		public Apple mApple;
		Texture2D GrassTexture;
		Texture2D NormalAppleT;
		Texture2D DimensionAppleT;
		Texture2D CrossT;
		Snake mSnake;

		public const int THEIGHT = 10;
		public const int TWIDTH = 10;
		int prev_step = -1;

		bool alive = true;


		public SnakeGame(Vector2 StartPos, int StartSize, Texture2D HT, Texture2D BT, Texture2D CT, Texture2D TT, Texture2D GT, Texture2D[] Apples, Texture2D Cross)
		{
			mSnake = new Snake(new Vector2(5, 5), ref HT, ref BT, ref CT, ref TT);

			GrassTexture = GT;
			CrossT = Cross;
			NormalAppleT = Apples[0];
			DimensionAppleT = Apples[1];
			alive = true;
			mApple = new Apple();
			prev_step = -1;
			CurrPos = StartPos;

			CurrWidth = CurrHeight = StartSize;
		}

		public void PlaceApple(AppleType type)
		{
			while (TWIDTH * THEIGHT > mSnake.Positions.Count)
			{
				AridArnold.MonoRandom rng = AridArnold.RandomManager.I.GetWorld();
				int x = rng.GetIntRange(0, TWIDTH - 1);
				int y = rng.GetIntRange(0, THEIGHT - 1);

				mApple.position = new Vector2(x, y);
				mApple.type = type;

				if (mSnake.IsSpaceAvailable(mApple.position))
				{
					mApple.active = true;
					break;
				}

			}
		}

		void Interpolate(ref int Curr, ref int NewVal)
		{
			if (Math.Abs(Curr - NewVal) < 3)
			{
				Curr = NewVal;
				return;
			}

			float delta = (INTERP_SPEED * (NewVal - Curr));

			if (delta > 0.0f)
			{
				NewVal = Curr + (int)(Math.Ceiling(delta));
			}
			else
			{
				NewVal = Curr + (int)(Math.Floor(delta));
			}

			Curr = NewVal;
		}

		void Interpolate(ref Vector2 Curr, ref Vector2 NewVal)
		{
			if ((Curr - NewVal).LengthSquared() < 3.0f)
			{
				Curr = NewVal;
				return;
			}

			NewVal = Curr + INTERP_SPEED * (NewVal - Curr);
			Curr = NewVal;
		}

		public void Draw(SpriteBatch spriteBatch, Camera Cam, Vector2 Offset, int width, int height)
		{
			//Interpolate params here
			Offset = CurrPos + INTERP_SPEED * (Offset - CurrPos);
			CurrPos = Offset;

			Interpolate(ref CurrWidth, ref width);

			Interpolate(ref CurrHeight, ref height);

			int block_height = height / THEIGHT;
			int block_width = width / TWIDTH;

			for (int x = 0; x < TWIDTH; x++)
			{
				for (int y = 0; y < THEIGHT; y++)
				{
					Rectangle bounds = Utilities.getBounds(Offset, x, y, block_width, block_height);

					spriteBatch.Draw(GrassTexture, bounds, Color.White);

					if (mApple.active && (int)mApple.position.X == x && (int)mApple.position.Y == y)
					{
						if (mApple.type == AppleType.AT_DIMENSION)
						{
							spriteBatch.Draw(DimensionAppleT, bounds, Color.White);
						}
						else
						{
							spriteBatch.Draw(NormalAppleT, bounds, Color.White);
						}
					}


				}
			}

			mSnake.Draw(spriteBatch, Cam, Offset, block_width, block_height);

			if (alive == false)
			{
				Rectangle bounds = Utilities.getBounds(Offset, (int)mSnake.Positions[0].X, (int)mSnake.Positions[0].Y, block_width, block_height);

				bounds.X -= bounds.Width / 2;
				bounds.Y -= bounds.Height / 2;
				bounds.Width *= 2;
				bounds.Height *= 2;
				spriteBatch.Draw(CrossT, bounds, Color.White);
			}
		}

		public UpdateResult Update(int curr_step)
		{
			if (prev_step == -1)
			{
				prev_step = curr_step;
			}

			UpdateResult update_res = UpdateResult.UR_NORMAL;

			bool up = AridArnold.InputManager.I.KeyHeld(AridArnold.InputAction.ArnoldUp);
			bool left = AridArnold.InputManager.I.KeyHeld(AridArnold.InputAction.ArnoldLeft);
			bool down = AridArnold.InputManager.I.KeyHeld(AridArnold.InputAction.ArnoldDown);
			bool right = AridArnold.InputManager.I.KeyHeld(AridArnold.InputAction.ArnoldRight);

			if (up && !down)
			{
				mSnake.SetNewDir(SDirection.Up);
			}
			if (left && !right)
			{
				mSnake.SetNewDir(SDirection.Left);
			}
			if (down && !up)
			{
				mSnake.SetNewDir(SDirection.Down);
			}
			if (right && !left)
			{
				mSnake.SetNewDir(SDirection.Right);
			}

			// In sync with 120bpm
			while (prev_step < curr_step)
			{
				prev_step++;

				Vector2 prevTail = mSnake.Positions[mSnake.Positions.Count - 1];
				mSnake.Move();

				if (mApple.active && mSnake.Positions[0] == mApple.position)
				{
					update_res = EatApple(prevTail);
				}

				if (mSnake.IsEatingItself())
				{
					update_res = UpdateResult.UR_DEAD;
					alive = false;
				}
			}

			return update_res;
		}

		public Vector2 GetSnakeHeadPos()
		{
			return new Vector2(mSnake.Positions[0].X * CurrWidth / TWIDTH, mSnake.Positions[0].Y * CurrHeight / THEIGHT) + CurrPos;
		}

		UpdateResult EatApple(Vector2 prev_tail)
		{
			UpdateResult ur = UpdateResult.UR_APPLE_EATEN;
			if (mApple.type == AppleType.AT_DIMENSION)
			{
				ur = UpdateResult.UR_NEW_DIMENSION;
			}
			mSnake.Positions.Add(prev_tail);
			mApple.active = false;

			return ur;
		}
	}
}
