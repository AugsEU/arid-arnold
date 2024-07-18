namespace AridArnold
{
	class ScreenWipe : Fade
	{
		CardinalDirection mDirection;

		public ScreenWipe(CardinalDirection direction, Rectangle area) : base(area)
		{
			mDirection = direction;
		}

		public ScreenWipe(CardinalDirection direction) : base(DEFAULT_FADE_AREA)
		{
			mDirection = direction;
		}

		public override void DrawAtTime(DrawInfo info, float time)
		{
			Rectangle rectToDraw = mArea;

			switch (mDirection)
			{
				case CardinalDirection.Down:
					rectToDraw.Height = (int)(rectToDraw.Height * time);
					break;
				case CardinalDirection.Up:
					rectToDraw.Height = (int)(rectToDraw.Height * time);
					rectToDraw.Y = mArea.Y + mArea.Height - rectToDraw.Height;
					break;
				case CardinalDirection.Right:
					rectToDraw.Width = (int)(rectToDraw.Width * time);
					break;
				case CardinalDirection.Left:
					rectToDraw.Width = (int)(rectToDraw.Width * time); 
					rectToDraw.X = mArea.X + mArea.Width - rectToDraw.Width;
					break;
			}

			MonoDraw.DrawRectDepth(info, rectToDraw, Color.Black, DrawLayer.Front);
		}
	}
}
