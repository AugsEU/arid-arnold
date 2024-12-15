namespace AridArnold
{
	/// <summary>
	/// Mouse button binding.
	/// </summary>
	class MouseBtnBinding : InputBinding, IEquatable<MouseBtnBinding>
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

		public override BindingCategory GetBindingCategory()
		{
			return BindingCategory.kMouseButton;
		}

		public override bool Equals(object obj)
		{
			if (obj is null || obj is not MouseBtnBinding)
			{
				return false;
			}
			return Equals((MouseBtnBinding)obj);
		}

		public bool Equals(MouseBtnBinding other)
		{
			return other.mMouseButton == mMouseButton;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void WriteFromBinary(BinaryWriter bw)
		{
			bw.Write((int)mMouseButton);
		}
	}
}
