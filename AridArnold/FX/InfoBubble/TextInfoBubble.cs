﻿namespace AridArnold
{
	internal class TextInfoBubble : InfoBubble
	{
		const int PADDING = 5;

		SpriteFont mFont;
		Color mColor;
		string mText;


		public TextInfoBubble(Vector2 botCentre, BubbleStyle style, SpriteFont font, string stringID, Color textColor) : base(botCentre, style, 0.0f)
		{
			mText = LanguageManager.I.GetText(stringID);
			mFont = font;
			mColor = textColor;
			Vector2 stringSize = font.MeasureString(mText);

			SetTargetSize((int)MathF.Ceiling(stringSize.X) + PADDING, (int)MathF.Ceiling(stringSize.Y) + PADDING);
		}

		protected override void DrawInner(DrawInfo info, Rectangle area)
		{
			Vector2 origin = new Vector2(area.X, area.Y);
			origin.X += area.Width * 0.5f;
			origin.Y += area.Height * 0.5f;

			MonoDraw.DrawStringCentred(info, mFont, origin, mColor, mText, DrawLayer.Bubble);
		}

		protected override void UpdateInternal()
		{
		}
	}
}