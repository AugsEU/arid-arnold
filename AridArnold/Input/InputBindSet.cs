namespace AridArnold
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



		/// <summary>
		/// Init set with a list of bindings
		/// </summary>
		/// <param name="bindings">List of bindings</param>
		public InputBindSet(List<InputBinding> bindings)
		{
			mBindings = bindings;
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
			return mBindings.Exists(x => x.InputDown());
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





		#region rBinding

		/// <summary>
		/// Add a new binding, overwriting those who have the same binding category
		/// </summary>
		public void AddRebinding(InputBinding binding)
		{
			BindingCategory newCategory = binding.GetBindingCategory();
			for(int i = 0; i < mBindings.Count; i++)
			{
				if (mBindings[i].GetBindingCategory() == newCategory)
				{
					// Replace this
					mBindings[i] = binding;
					return;
				}
			}

			// Otherwise add it to the list
			mBindings.Add(binding);
		}

		#endregion rBinding





		#region rUtility

		/// <summary>
		/// Convert to list of input strings
		/// </summary>
		public override string ToString()
		{
			string returnStr = "";
			for(int i = 0; i < mBindings.Count; i++)
			{
				returnStr += mBindings[i].ToString();
				if(i < mBindings.Count - 1) returnStr += ", ";
			}

			return returnStr;
		}

		#endregion rUtility





		#region rSerial

		/// <summary>
		/// Read binary segment
		/// </summary>
		public static InputBindSet ReadFromBinary(BinaryReader br)
		{
			List<InputBinding> inputBindings = new List<InputBinding>();
			int numButtons = br.ReadInt32();
			for(int i = 0; i < numButtons; i++)
			{
				InputBinding binding = null;
				InputBindingType bindingType = (InputBindingType)br.ReadInt32();
				switch (bindingType)
				{
					case InputBindingType.kKeyboard:
						binding = new KeyBinding(br);
						break;
					case InputBindingType.kGamepad:
						binding = new PadBinding(br);
						break;
					case InputBindingType.kMouse:
						binding = new MouseBtnBinding(br);
						break;
					default:
						throw new Exception("Invalid input type detected");
				}
				MonoDebug.Assert(binding is not null, "Corrupted global save.");
				inputBindings.Add(binding);
			}

			return new InputBindSet(inputBindings);
		}



		/// <summary>
		/// Write binary segment
		/// </summary>
		public void WriteFromBinary(BinaryWriter bw)
		{
			bw.Write((int)mBindings.Count);
			foreach(InputBinding binding in mBindings)
			{
				int bindingTypeInt = (int)binding.GetBindingType();
				bw.Write(bindingTypeInt);
				binding.WriteFromBinary(bw);
			}
		}

		#endregion rSerial
	}
}
