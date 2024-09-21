namespace AridArnold
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
		Vector2 mTextSize;
		float mMaxHeight;
		PercentageTimer mRiseTimer;
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
		public ScrollerTextFX(SpriteFont font, Color colour, Vector2 pos, string text, float maxHeight, float time)
		{
			mFont = font;
			mColor = colour;
			mText = text;
			mMaxHeight = maxHeight;
			mRiseTimer = new PercentageTimer(time * 100.0f);
			mRiseTimer.Start();

			mStartPos = pos;


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
			mRiseTimer.Update(gameTime);
		}



		/// <summary>
		/// Are we finished?
		/// </summary>
		/// <returns>True if we are finished.</returns>
		public override bool Finished()
		{
			return mRiseTimer.GetPercentageF() >= 1.0f;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw text scroller
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public override void Draw(DrawInfo info)
		{
			float t = GetRiseT();

			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			float rot = -gameCam.GetCurrentSpec().mRotation;
			Vector2 downVec = MonoMath.Rotate(new Vector2(0.0f, 1.0f), rot);

			Vector2 relStartPos = mStartPos - downVec * 5.0f;

			Vector2 destPos = relStartPos - downVec * mMaxHeight;

			Vector2 drawPos = MonoMath.Lerp(relStartPos, destPos, t);

			// Clamp to visible area
			Point visSize = FXManager.I.GetDrawableSize();
			drawPos.X = Math.Clamp(drawPos.X, SCREEN_BORDER + (mTextSize.X / 2.0f), visSize.X - SCREEN_BORDER - (mTextSize.X / 2.0f));
			drawPos.Y = Math.Clamp(drawPos.Y, SCREEN_BORDER + (mTextSize.Y / 2.0f), visSize.Y - SCREEN_BORDER - (mTextSize.Y / 2.0f));

			Vector2 dropShadow = drawPos + 2.0f * downVec + MonoMath.Perpendicular(downVec);

			MonoDraw.DrawStringCentredRot(info, mFont, dropShadow, Color.Black, mText, rot, DrawLayer.Bubble);
			MonoDraw.DrawStringCentredRot(info, mFont, drawPos, mColor, mText, rot, DrawLayer.Bubble);

		}

		float GetRiseT()
		{
			float t = mRiseTimer.GetPercentageF();
			t *= 4.0f;

			return Math.Clamp(t, 0.0f, 1.0f);
		}

		#endregion rDraw
	}
}
