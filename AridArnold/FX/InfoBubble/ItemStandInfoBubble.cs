namespace AridArnold
{
	internal class ItemStandInfoBubble : InfoBubble
	{
		const int PADDING = 7;

		SpriteFont mFont;
		string mTitle;
		string mDescription;



		/// <summary>
		/// Construct specific bubble
		/// </summary>
		public ItemStandInfoBubble(Vector2 botCentre, BubbleStyle style, string titleText, string descText) : base(botCentre, style, 0.0f)
		{
			mTitle = titleText;
			mDescription = descText;
			mExpandSpeed = 65.0f;

			mFont = FontManager.I.GetFont("Pixica", 12, false);

			Vector2 titleSize = mFont.MeasureString(mTitle);

			float width = titleSize.X;
			float height = titleSize.Y * 1.5f;

			string accumulatedStr = "";
			for(int c = 0; c < descText.Length; c++)
			{
				char newChar = descText[c];
				if(newChar == '\n' || c == descText.Length - 1)
				{
					Vector2 lineSize = mFont.MeasureString(accumulatedStr);
					width = Math.Max(width, lineSize.X);
					accumulatedStr = "";
					height += lineSize.Y;
				}
				else
				{
					accumulatedStr += newChar;
				}
			}

			SetTargetSize((int)MathF.Ceiling(width) + (int)(PADDING * 1.3f), (int)MathF.Ceiling(height) + PADDING);
		}

		protected override void DrawInner(DrawInfo info, Rectangle area)
		{
			Vector2 descSize = mFont.MeasureString("M");
			Vector2 origin = new Vector2(area.X, area.Y);
			origin.X += area.Width * 0.5f;
			origin.Y += PADDING;

			// Title
			MonoDraw.DrawStringCentred(info, mFont, origin, UIPanelBase.PANEL_GOLD, mTitle, DrawLayer.Bubble);

			origin.Y += descSize.Y * 1.5f;

			// Description
			MonoDraw.DrawParagraphCentred(info, mFont, origin, UIPanelBase.PANEL_WHITE, mDescription, descSize.Y, DrawLayer.Bubble);
		}

		protected override void UpdateInternal()
		{
		}
	}
}
