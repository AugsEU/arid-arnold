namespace GMTK2023
{
	/// <summary>
	/// Represents a set of input bindings.
	/// </summary>
	internal class InputBindSet
	{
		#region rMembers

		List<InputBinding> mBindings;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init set with a list of bindings
		/// </summary>
		/// <param name="bindings">List of bindings</param>
		public InputBindSet(params InputBinding[] bindings)
		{
			mBindings = new List<InputBinding>(bindings);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update keys
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			mBindings.ForEach(x => x.Update(gameTime));
		}

		#endregion rUpdate





		#region rKeyPresses

		/// <summary>
		/// Any of our bindings held?
		/// </summary>
		/// <returns>True if any input in the list is currently held</returns>
		public bool AnyKeyHeld()
		{
			return mBindings.Exists(x => x.IsInputDown());
		}



		/// <summary>
		/// Any of our bindings pressed in the last update?
		/// </summary>
		/// <returns>True if any input in the list was pressed in the last update.</returns>
		public bool AnyKeyPressed()
		{
			return mBindings.Exists(x => x.InputPressed());
		}

		#endregion rKeyPresses
	}
}
