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
	/// Keyboard button binding.
	/// </summary>
	class KeyBinding : InputBinding, IEquatable<KeyBinding>
	{
		Keys mInputKey;

		public KeyBinding(Keys inputKey) : base()
		{
			mInputKey = inputKey;
		}

		public KeyBinding(BinaryReader br) : base()
		{
			mInputKey = (Keys)br.ReadInt32();
		}

		protected override bool PollInput()
		{
			return Keyboard.GetState().IsKeyDown(mInputKey);
		}

		public override string ToString()
		{
			return string.Format("[{0}]", mInputKey.ToString());
		}

		public override InputBindingType GetBindingType()
		{
			return InputBindingType.kKeyboard;
		}

		public override BindingCategory GetBindingCategory()
		{
			return BindingCategory.kKeyboard;
		}

		public override bool Equals(object obj)
		{
			if(obj is null || obj is not KeyBinding)
			{
				return false;
			}
			return Equals((KeyBinding)obj);
		}

		public bool Equals(KeyBinding other)
		{
			return other.mInputKey == mInputKey;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void WriteFromBinary(BinaryWriter bw)
		{
			bw.Write((int)mInputKey);
		}


	}
}
