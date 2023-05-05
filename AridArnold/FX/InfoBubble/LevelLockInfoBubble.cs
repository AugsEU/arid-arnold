namespace AridArnold
{
	internal class LevelLockInfoBubble : InfoBubble
	{
		const int BUBBLE_WIDTH = 12;
		const int BUBBLE_HEIGHT = 20;
		static Vector2 KEY_OFFSET = new Vector2(-1.0f, 2.0f);

		Texture2D mKeyIcon;

		public LevelLockInfoBubble(Vector2 botCentre, BubbleStyle style) : base(botCentre, style)
		{
			mKeyIcon = MonoData.I.MonoGameLoad<Texture2D>("Tiles/KeyFull");

			SetTargetSize(BUBBLE_WIDTH, BUBBLE_HEIGHT);
		}

		protected override void DrawInner(DrawInfo info, Rectangle area)
		{
			Vector2 origin = new Vector2(area.X, area.Y);

			MonoDraw.DrawTextureDepth(info, mKeyIcon, origin + KEY_OFFSET, DrawLayer.Bubble);
		}

		protected override void UpdateInternal()
		{
		}
	}
}
