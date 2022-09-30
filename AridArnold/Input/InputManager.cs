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

        //Arnold
        ArnoldLeft,
        ArnoldRight,
        ArnoldUp,
        ArnoldDown,
        ArnoldJump
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
            mInputBindings.Add(AridArnoldKeys.Confirm,      new InputBindSet( new KeyBinding(Keys.Enter)));
            mInputBindings.Add(AridArnoldKeys.Pause,        new InputBindSet( new KeyBinding(Keys.Escape)));
            mInputBindings.Add(AridArnoldKeys.RestartLevel, new InputBindSet( new KeyBinding(Keys.R)));

            mInputBindings.Add(AridArnoldKeys.ArnoldLeft,   new InputBindSet( new KeyBinding(Keys.A)));
            mInputBindings.Add(AridArnoldKeys.ArnoldRight,  new InputBindSet( new KeyBinding(Keys.D)));
            mInputBindings.Add(AridArnoldKeys.ArnoldUp,     new InputBindSet( new KeyBinding(Keys.W)));
            mInputBindings.Add(AridArnoldKeys.ArnoldDown,   new InputBindSet( new KeyBinding(Keys.S)));
            mInputBindings.Add(AridArnoldKeys.ArnoldJump,   new InputBindSet( new KeyBinding(Keys.Space)));
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
        #endregion rKeySense
    }
}
