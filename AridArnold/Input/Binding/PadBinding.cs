
namespace AridArnold
{
	/// <summary>
	/// Joypad button binding.
	/// </summary>
	class PadBinding : InputBinding, IEquatable<PadBinding>
	{
		Buttons mInputKey;

		public PadBinding(Buttons inputKey) : base()
		{
			mInputKey = inputKey;
		}

		public PadBinding(BinaryReader br) : base()
		{
			mInputKey = (Buttons)br.ReadInt32();
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
			string stringID = string.Format("Input.Pad.{0}", mInputKey.ToString());
			return LanguageManager.I.GetText(stringID);
		}

		public override InputBindingType GetBindingType()
		{
			return InputBindingType.kGamepad;
		}

		public override BindingCategory GetBindingCategory()
		{
			switch (mInputKey)
			{
				case Buttons.RightThumbstickUp:
				case Buttons.RightThumbstickDown:
				case Buttons.RightThumbstickRight:
				case Buttons.RightThumbstickLeft:
				case Buttons.LeftThumbstickUp:
				case Buttons.LeftThumbstickDown:
				case Buttons.LeftThumbstickRight:
				case Buttons.LeftThumbstickLeft:
					return BindingCategory.kGamepadAxis;
			}

			return BindingCategory.kGamepadButton;
		}

		public override bool Equals(object obj)
		{
			if (obj is null || obj is not PadBinding)
			{
				return false;
			}
			return Equals((PadBinding)obj);
		}

		public bool Equals(PadBinding other)
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
