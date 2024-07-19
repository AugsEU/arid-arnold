namespace AridArnold
{
	/// <summary>
	/// Mouse button
	/// </summary>
	enum MouseButton
	{
		Left,
		Middle,
		Right
	}



	/// <summary>
	/// Represents a single keybinding. Is either on or off
	/// </summary>
	abstract class InputBinding
	{
		bool mPreviousState;
		bool mCurrentState;


		public InputBinding()
		{
			mPreviousState = false;
			mCurrentState = false;
		}



		/// <summary>
		/// Is the input currently down?
		/// </summary>
		/// <returns>True if the input is currently held</returns>
		abstract protected bool PollInput();



		/// <summary>
		/// Is the input pressed since the last update?
		/// </summary>
		/// <returns>True if the input was pressed since the last update</returns>
		public bool InputPressed()
		{
			return mCurrentState && !mPreviousState;
		}



		/// <summary>
		/// Is this key down at all?
		/// </summary>
		/// <returns></returns>
		public bool InputDown()
		{
			return mCurrentState;
		}



		/// <summary>
		/// Update key state
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			mPreviousState = mCurrentState;
			mCurrentState = PollInput();
		}
	}



	/// <summary>
	/// Keyboard button binding.
	/// </summary>
	class KeyBinding : InputBinding
	{
		Keys mInputKey;

		public KeyBinding(Keys inputKey) : base()
		{
			mInputKey = inputKey;
		}

		protected override bool PollInput()
		{
			return Keyboard.GetState().IsKeyDown(mInputKey);
		}

		public override string ToString()
		{
			return mInputKey.ToString();
		}
	}

	/// <summary>
	/// Joypad button binding.
	/// </summary>
	class PadBinding : InputBinding
	{
		Buttons mInputKey;

		public PadBinding(Buttons inputKey) : base()
		{
			mInputKey = inputKey;
		}

		protected override bool PollInput()
		{
			// Poll all controllers since there is no multiplayer for now.
			if (GamePad.GetState(PlayerIndex.One).IsButtonDown(mInputKey)) return true;
			if (GamePad.GetState(PlayerIndex.Two).IsButtonDown(mInputKey)) return true;
			if (GamePad.GetState(PlayerIndex.Three).IsButtonDown(mInputKey)) return true;
			if (GamePad.GetState(PlayerIndex.Four).IsButtonDown(mInputKey)) return true;

			return false;
		}

		public override string ToString()
		{
			return mInputKey.ToString();
		}
	}



	/// <summary>
	/// Mouse button binding.
	/// </summary>
	class MouseBtnBinding : InputBinding
	{
		MouseButton mMouseButton;

		public MouseBtnBinding(MouseButton mouseBtn) : base()
		{
			mMouseButton = mouseBtn;
		}

		protected override bool PollInput()
		{
			switch (mMouseButton)
			{
				case MouseButton.Left:
					return Mouse.GetState().LeftButton == ButtonState.Pressed;
				case MouseButton.Middle:
					return Mouse.GetState().MiddleButton == ButtonState.Pressed;
				case MouseButton.Right:
					return Mouse.GetState().RightButton == ButtonState.Pressed;
				default:
					break;
			}

			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return mMouseButton.ToString();
		}
	}
}
