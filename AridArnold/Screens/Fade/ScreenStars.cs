namespace AridArnold
{
	class ScreenStars : ScreenFade
	{
		float mSpacing;

		public ScreenStars(float spacing, float speed, bool forwards) : base(speed, forwards)
		{
			mSpacing = spacing;
		}

		protected override void DrawAtTime(DrawInfo info, float time)
		{
			Texture2D dummy = Main.GetDummyTexture();
			for(float x = 0.0f; x < GameScreen.GAME_AREA_WIDTH; x += mSpacing)
			{
				for (float y = 0.0f; y < GameScreen.GAME_AREA_HEIGHT; y += mSpacing)
				{
					MonoDraw.DrawTexture(info, dummy, new Vector2(x, y - time * mSpacing), null, Color.Black, MathF.PI * 0.25f, Vector2.Zero, time * mSpacing * 2.0f, SpriteEffects.None, DrawLayer.Front);
				}
			}
		}
	}
}
