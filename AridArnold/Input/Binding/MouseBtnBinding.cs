namespace AridArnold
{
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

		public MouseBtnBinding(BinaryReader br) : base()
		{
			mMouseButton = (MouseButton)br.ReadInt32();
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

		public override InputBindingType GetBindingType()
		{
			return InputBindingType.kMouse;
		}

		public override void WriteFromBinary(BinaryWriter bw)
		{
			bw.Write((int)mMouseButton);
		}
	}
}
