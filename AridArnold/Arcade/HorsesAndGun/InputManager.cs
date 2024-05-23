namespace HorsesAndGun
{
	internal class InputManager : Singleton<InputManager>
	{
		KeyboardState mCurrentKeyState;
		KeyboardState mPreviousKeyState;

		MouseState mPrevMouseState;
		MouseState mMouseState;

		public void Update(GameTime gameTime)
		{
			mPreviousKeyState = mCurrentKeyState;

			mCurrentKeyState = Keyboard.GetState();

			mPrevMouseState = mMouseState;
			mMouseState = Mouse.GetState();
		}

		public bool AllKeysPressed(Keys[] keys)
		{
			foreach (Keys key in keys)
			{
				if (!KeyPressed(key))
				{
					return false;
				}
			}

			return true;
		}

		public bool AnyKeysPressed(Keys[] keys)
		{
			foreach (Keys key in keys)
			{
				if (KeyPressed(key))
				{
					return true;
				}
			}

			return false;
		}

		public bool KeyPressed(Keys key)
		{
			return mCurrentKeyState.IsKeyDown(key) && !mPreviousKeyState.IsKeyDown(key);
		}

		public bool LClick()
		{
			bool nowPress = mMouseState.LeftButton == ButtonState.Pressed;
			bool prevPress = mPrevMouseState.LeftButton == ButtonState.Pressed;

			return nowPress && !prevPress;
		}

		public bool RClick()
		{
			bool nowPress = mMouseState.RightButton == ButtonState.Pressed;
			bool prevPress = mPrevMouseState.RightButton == ButtonState.Pressed;

			return nowPress && !prevPress;
		}

		public Point GetMousePos()
		{
			Point screenPoint;

			screenPoint.X = (mMouseState.Position.X - Game1.sRenderTargetRect.Location.X) / Game1.sRenderTargetRect.Width;
			screenPoint.Y = (mMouseState.Position.Y - Game1.sRenderTargetRect.Location.Y) / Game1.sRenderTargetRect.Height;

			return screenPoint;
		}
	}
}
