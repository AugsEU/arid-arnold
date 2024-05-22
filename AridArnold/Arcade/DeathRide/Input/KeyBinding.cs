namespace GMTK2023
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
		abstract public bool IsInputDown();



		/// <summary>
		/// Is the input pressed since the last update?
		/// </summary>
		/// <returns>True if the input was pressed since the last update</returns>
		public bool InputPressed()
		{
			return mCurrentState && !mPreviousState;
		}



		/// <summary>
		/// Update key state
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			mPreviousState = mCurrentState;
			mCurrentState = IsInputDown();
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

		public override bool IsInputDown()
		{
			return Keyboard.GetState().IsKeyDown(mInputKey);
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

		public override bool IsInputDown()
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
	}
}
