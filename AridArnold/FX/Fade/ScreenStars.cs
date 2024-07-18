using WormWarp;

namespace AridArnold
{
	class ScreenStars : Fade
	{
		float mSpacing;

		public ScreenStars(float spacing, Rectangle area) : base(area)
		{
			mSpacing = spacing;
		}

		public ScreenStars(float spacing = 10.0f) : base(DEFAULT_FADE_AREA)
		{
			mSpacing = spacing;
		}

		public override void DrawAtTime(DrawInfo info, float time)
		{
			Texture2D dummy = Main.GetDummyTexture();

			float xStart = mArea.X - mSpacing;
			float xEnd = mArea.X + mArea.Width + mSpacing;
			float yStart = mArea.Y - mSpacing;
			float yEnd = mArea.Y + mArea.Height + mSpacing;

			for (float x = xStart; x < xEnd; x += mSpacing)
			{
				for (float y = yStart; y < yEnd; y += mSpacing)
				{
					MonoDraw.DrawTexture(info, dummy, new Vector2(x, y - time * mSpacing), null, Color.Black, MathF.PI * 0.25f, Vector2.Zero, time * mSpacing * 2.0f, SpriteEffects.None, DrawLayer.Front);
				}
			}
		}
	}
}
