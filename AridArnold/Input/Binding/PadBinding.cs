
namespace AridArnold
{
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
			return mInputKey.ToString();
		}

		public override InputBindingType GetBindingType()
		{
			return InputBindingType.kGamepad;
		}

		public override void WriteFromBinary(BinaryWriter bw)
		{
			bw.Write((int)mInputKey);
		}
	}
}
