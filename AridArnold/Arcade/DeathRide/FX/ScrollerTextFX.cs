namespace GMTK2023
{
	internal class ScrollerTextFX : FX
	{
		#region rConstants

		const float SCREEN_BORDER = 5.0f;

		#endregion rConstants





		#region rMembers

		SpriteFont mFont;
		Color mColor;
		Vector2 mStartPos;
		Vector2 mPos;
		Vector2 mShadowOffset;
		Vector2 mTextSize;
		float mSpeed;
		float mMaxHeight;
		float mTime;
		string mText;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Add small piece of text that scrolls up.
		/// </summary>
		/// <param name="font">Font to draw text in</param>
		/// <param name="colour">Colour of text</param>
		/// <param name="pos">Starting position</param>
		/// <param name="text">String to display</param>
		/// <param name="upSpeed">Speed at which text goes up</param>
		/// <param name="maxHeight">Maximum height difference reached by text</param>
		/// <param name="time">Time that text shows up</param>
		public ScrollerTextFX(SpriteFont font, Color colour, Vector2 pos, string text, float upSpeed, float maxHeight, float time)
		{
			mFont = font;
			mColor = colour;
			mPos = pos;
			mSpeed = upSpeed;
			mText = text;
			mMaxHeight = maxHeight;
			mTime = time;

			mStartPos = pos;

			mShadowOffset = new Vector2(1.0f, 2.0f);

			mTextSize = mFont.MeasureString(mText);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update text scroller
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			mPos.Y -= mSpeed * Util.GetDeltaT(gameTime);
		}



		/// <summary>
		/// Are we finished?
		/// </summary>
		/// <returns>True if we are finished.</returns>
		public override bool Finished()
		{
			return Math.Abs(mStartPos.Y - mPos.Y) > mTime + mMaxHeight;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw text scroller
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public override void Draw(DrawInfo info)
		{
			Vector2 drawPos = mPos;
			drawPos.Y = Math.Max(drawPos.Y, mStartPos.Y - mMaxHeight);

			// Clamp to visible area

			Point visSize = FXManager.I.GetDrawableSize();
			drawPos.X = Math.Clamp(drawPos.X, SCREEN_BORDER + (mTextSize.X / 2.0f), visSize.X - SCREEN_BORDER - (mTextSize.X / 2.0f));
			drawPos.Y = Math.Clamp(drawPos.Y, SCREEN_BORDER + (mTextSize.Y / 2.0f), visSize.Y - SCREEN_BORDER - (mTextSize.Y / 2.0f));

			Vector2 dropShadow = drawPos + mShadowOffset;

			MonoDraw.DrawStringCentred(info, mFont, dropShadow, Color.Black, mText, DrawLayer.Text);
			MonoDraw.DrawStringCentred(info, mFont, drawPos, mColor, mText, DrawLayer.Text);

		}

		#endregion rDraw
	}
}
