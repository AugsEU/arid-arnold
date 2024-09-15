namespace AridArnold
{
	/// <summary>
	/// Type enum for serialisation
	/// </summary>
	enum InputBindingType
	{
		kKeyboard,
		kGamepad,
		kMouse
	}



	/// <summary>
	/// What kind of category does this binding fit into?
	/// </summary>
	enum BindingCategory
	{
		kKeyboard,
		kGamepadButton,
		kGamepadAxis,
		kMouseButton
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
			mCurrentState = Main.GameActive() && PollInput();
		}



		/// <summary>
		/// Type enum for serialisation
		/// </summary>
		public abstract InputBindingType GetBindingType();



		/// <summary>
		/// What category does this fit into?
		/// </summary>
		public abstract BindingCategory GetBindingCategory();



		/// <summary>
		/// Write binary segment
		/// </summary>
		public abstract void WriteFromBinary(BinaryWriter bw);
	}
}
