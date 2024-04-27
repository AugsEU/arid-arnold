namespace AridArnold
{
	using InputBindingMap = Dictionary<AridArnoldKeys, InputBindSet>;
	using InputBindingPair = KeyValuePair<AridArnoldKeys, InputBindSet>;

	enum AridArnoldKeys
	{
		//Menu
		Confirm,
		Pause,
		RestartLevel,
		SkipLevel,

		//Arnold
		ArnoldLeft,
		ArnoldRight,
		ArnoldUp,
		ArnoldDown,
		ArnoldJump,

		//Game
		UseItem
	}

	/// <summary>
	/// Singleton that manages inputs.
	/// </summary>
	internal class InputManager : Singleton<InputManager>
	{
		#region rMembers

		InputBindingMap mInputBindings = new InputBindingMap();

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init input manager
		/// </summary>
		public void Init()
		{
			//Set default bindings for now. TO DO: Load from file.
			SetDefaultBindings();
		}



		/// <summary>
		/// Set default bindings
		/// </summary>
		public void SetDefaultBindings()
		{
			mInputBindings.Add(AridArnoldKeys.Confirm, new InputBindSet(new KeyBinding(Keys.Enter)));
			mInputBindings.Add(AridArnoldKeys.Pause, new InputBindSet(new KeyBinding(Keys.Escape)));
			mInputBindings.Add(AridArnoldKeys.RestartLevel, new InputBindSet(new KeyBinding(Keys.R)));
			mInputBindings.Add(AridArnoldKeys.SkipLevel, new InputBindSet(new KeyBinding(Keys.P)));

			mInputBindings.Add(AridArnoldKeys.ArnoldLeft, new InputBindSet(new KeyBinding(Keys.Left)));
			mInputBindings.Add(AridArnoldKeys.ArnoldRight, new InputBindSet(new KeyBinding(Keys.Right)));
			mInputBindings.Add(AridArnoldKeys.ArnoldUp, new InputBindSet(new KeyBinding(Keys.Up)));
			mInputBindings.Add(AridArnoldKeys.ArnoldDown, new InputBindSet(new KeyBinding(Keys.Down)));
			mInputBindings.Add(AridArnoldKeys.ArnoldJump, new InputBindSet(new KeyBinding(Keys.Space)));

			mInputBindings.Add(AridArnoldKeys.UseItem, new InputBindSet(new KeyBinding(Keys.LeftShift)));
		}

		#endregion rInitialisation





		#region rKeySense

		/// <summary>
		/// Update Inputs
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			foreach (InputBindingPair keyBindPair in mInputBindings)
			{
				keyBindPair.Value.Update(gameTime);
			}
		}



		/// <summary>
		/// Was a button pressed recently?
		/// </summary>
		/// <param name="key">Key to check</param>
		/// <returns>True if it was pressed in the last update</returns>
		public bool KeyPressed(AridArnoldKeys key)
		{
			return mInputBindings[key].AnyKeyPressed();
		}



		/// <summary>
		/// Was a button pressed recently?
		/// </summary>
		/// <param name="key">Key to check</param>
		/// <returns>True if it was pressed in the last update</returns>
		public bool KeyHeld(AridArnoldKeys key)
		{
			return mInputBindings[key].AnyKeyHeld();
		}



		/// <summary>
		/// Get set of input bindings for a certain action type
		/// </summary>
		public InputBindSet GetInputBindSet(AridArnoldKeys key)
		{
			return mInputBindings[key];
		}
		#endregion rKeySense
	}
}
