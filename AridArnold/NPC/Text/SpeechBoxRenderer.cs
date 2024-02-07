namespace AridArnold
{
	struct SpeechBoxStyle
	{
		public SpriteFont mFont;
		public float mLeading;
		public float mKerning;
		public float mWidth;
		public float mScrollSpeed;
		public int mFramesPerLetter;
		public Color mBorderColor;
		public Color mFillColor;
		public bool mFlipSpike;

		static public SpeechBoxStyle DefaultStyle
		{
			get
			{
				SpeechBoxStyle ret = new SpeechBoxStyle();
				ret.mFont = FontManager.I.GetFont("Pixica-12");
				ret.mWidth = 230.1f;
				ret.mLeading = 8.0f;
				ret.mKerning = 1.0f;
				ret.mScrollSpeed = 0.75f;
				ret.mFramesPerLetter = 16;
				ret.mFillColor = new Color(0, 10, 20, 200);
				ret.mBorderColor = new Color(56, 89, 122);
				ret.mFlipSpike = false;
				return ret;
			}
		}
	}

	/// <summary>
	/// Class for rendering speech boxes.
	/// </summary> 
	internal class SpeechBoxRenderer
	{
		#region rConstants

		const int PADDING = 5;
		const int BORDER_WIDTH = 2;

		#endregion rConstants





		#region rMembers

		// Static
		static Texture2D sSpikeBorder;
		static Texture2D sSpikeInner;

		// Text
		SmartTextBlock mCurrentBlock;
		Queue<string> mPendingStringIDs;
		List<SpeechBoxLetter> mLetters;
		SpeechBoxStyle mStyle;
		float mSpikeOffset;

		//Transform
		Vector2 mBottomLeft;
		Vector2 mTopLeft;

		//Internal Data
		Vector2 mCharHead;
		Vector2 mLastDrawnCharHead;
		float mCharHeight;

		#endregion rMembers






		#region rInitialisation

		/// <summary>
		/// Construct speech box renderer
		/// </summary>
		public SpeechBoxRenderer(string stringID, Vector2 bottomLeft, float spikeOffset, SpeechBoxStyle style)
		{
			mStyle = style;
			mCurrentBlock = new SmartTextBlock(stringID, style.mFramesPerLetter);
			mLetters = new List<SpeechBoxLetter>();
			mBottomLeft = bottomLeft;

			mCharHeight = mStyle.mFont.MeasureString("M").Y;
			mCharHeight /= 2.0f;
			mCharHead = new Vector2(0.0f, -(int)GetNewLineSize());
			mTopLeft = mBottomLeft + mCharHead;

			mLastDrawnCharHead = mCharHead;

			mPendingStringIDs = new Queue<string>();
			mSpikeOffset = spikeOffset;
		}



		/// <summary>
		/// Load content.
		/// </summary>
		public static void LoadContent()
		{
			sSpikeBorder = MonoData.I.MonoGameLoad<Texture2D>("NPC/Dialog/DialogSpikeBorder");
			sSpikeInner = MonoData.I.MonoGameLoad<Texture2D>("NPC/Dialog/DialogSpikeInner");
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update text box.
		/// </summary>
		public void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			//Slowly increase speed when the tile is stopped.
			if (IsStopped())
			{
				mStyle.mScrollSpeed += 0.05f * dt;
			}

			ScrollLettersUp(dt);

			foreach (SpeechBoxLetter letter in mLetters)
			{
				letter.Update(gameTime);
			}

			if (!IsStopped() && (mCharHead.Y + GetNewLineSize()) < 0.0f)
			{
				bool newLetter = mCurrentBlock.AdvanceTimeStep();

				if (newLetter)
				{
					PlaceNewLetter();
				}
			}

			CheckForDeletion();
		}



		/// <summary>
		/// Scroll all the letters up by a certain amount.
		/// </summary>
		void ScrollLettersUp(float dt)
		{
			float dy = -dt * mStyle.mScrollSpeed;

			foreach (SpeechBoxLetter letter in mLetters)
			{
				letter.MoveUp(-dy);
			}

			mTopLeft.Y += dy;

			if (IsStopped())
			{
				mBottomLeft.Y += dy;

				if (-mLastDrawnCharHead.Y > GetNewLineSize())
				{
					mBottomLeft.Y += dy;
					mCharHead.Y -= dy;
					mLastDrawnCharHead.Y -= dy;
				}
			}
			else
			{
				mCharHead.Y += dy;
				mLastDrawnCharHead.Y += dy;
			}
		}



		/// <summary>
		/// Move everything up by a certain amount
		/// </summary>
		public void DisplaceVertically(float dy)
		{
			foreach (SpeechBoxLetter letter in mLetters)
			{
				letter.MoveUp(-dy);
			}

			mTopLeft.Y += dy;
			mBottomLeft.Y += dy;
		}


		/// <summary>
		/// Scans letters and deletes any that aren't needed.
		/// </summary>
		void CheckForDeletion()
		{
			float newLine = GetNewLineSize() + 20.0f;

			while (mLetters.Count > 0 && mLetters[0].GetPosition().Y < -newLine)
			{
				mLetters.RemoveAt(0);
			}
		}
		#endregion





		#region rDraw

		/// <summary>
		/// Draw the text out
		/// </summary>
		public void Draw(DrawInfo drawInfo)
		{
			// Draw BG and border
			DrawBox(drawInfo);

			// Draw the text.
			foreach (SpeechBoxLetter letter in mLetters)
			{
				letter.Draw(drawInfo);
			}
		}



		/// <summary>
		/// Draw box
		/// </summary>
		public void DrawBox(DrawInfo info)
		{
			Point rectPosition = new Point(MonoMath.Round(mTopLeft.X) - PADDING, MonoMath.Round(mTopLeft.Y) - PADDING);
			int height = MonoMath.Round(mBottomLeft.Y - mTopLeft.Y) + PADDING;
			int width = (int)mStyle.mWidth + 2 * PADDING + 5 + (int)((mCharHeight - 7.0f) / 2.0f);
			Rectangle bgRectangle = new Rectangle(rectPosition.X, rectPosition.Y, width, height);

			// Draw bg
			MonoDraw.DrawRectDepth(info, bgRectangle, mStyle.mFillColor, DrawLayer.Bubble);

			// Calc corners
			Vector2 TL = new Vector2(bgRectangle.X,                     bgRectangle.Y);
			Vector2 BL = new Vector2(bgRectangle.X,                     bgRectangle.Y + bgRectangle.Height);
			Vector2 BR = new Vector2(bgRectangle.X + bgRectangle.Width, bgRectangle.Y + bgRectangle.Height);
			Vector2 TR = new Vector2(bgRectangle.X + bgRectangle.Width, bgRectangle.Y);

			// Spike left and right
			Vector2 SL = BL + new Vector2(mSpikeOffset, 0.0f);
			Vector2 SR = SL + new Vector2(sSpikeBorder.Width, 0.0f);

			MonoDraw.DrawLine(info, TL, BL, mStyle.mBorderColor, BORDER_WIDTH, DrawLayer.Bubble);
			MonoDraw.DrawLine(info, BR, TR, mStyle.mBorderColor, BORDER_WIDTH, DrawLayer.Bubble);
			MonoDraw.DrawLine(info, TR, TL, mStyle.mBorderColor, BORDER_WIDTH, DrawLayer.Bubble);

			if (!IsStopped() && mSpikeOffset > 0.0f)
			{
				SpriteEffects spikeEffects = mStyle.mFlipSpike ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				Vector2 spikePos = new Vector2(rectPosition.X + mSpikeOffset, rectPosition.Y + height);
				MonoDraw.DrawTexture(info, sSpikeInner, spikePos, null, mStyle.mFillColor, 0.0f, Vector2.Zero, 1.0f, spikeEffects, DrawLayer.Bubble);
				MonoDraw.DrawTexture(info, sSpikeBorder, spikePos, null, mStyle.mBorderColor, 0.0f, Vector2.Zero, 1.0f, spikeEffects, DrawLayer.Bubble);
				MonoDraw.DrawLine(info, BL, SL, mStyle.mBorderColor, BORDER_WIDTH, DrawLayer.Bubble);
				MonoDraw.DrawLine(info, SR, BR, mStyle.mBorderColor, BORDER_WIDTH, DrawLayer.Bubble);
			}
			else
			{
				MonoDraw.DrawLine(info, BL, BR, mStyle.mBorderColor, BORDER_WIDTH, DrawLayer.Bubble);
			}
		}

		#endregion rDraw





		#region rPlacement

		/// <summary>
		/// Place new letter.
		/// </summary>
		void PlaceNewLetter()
		{
			char charToPrint = mCurrentBlock.GetCurrentTextChar();

			if (charToPrint == '\0')
			{
				EndOfStringReached();
				return;
			}

			if (charToPrint == '\n')
			{
				CRLF();
			}
			else
			{
				if (ShouldDrawChar(charToPrint))
				{
					mLetters.Add(new SpeechBoxLetter(mStyle.mFont, charToPrint, GetCharPlacementPos(), mCurrentBlock.GetAnimation()));
					mLastDrawnCharHead = mCharHead;
					mCharHead.X += GetCharWidth(charToPrint);

					if (charToPrint == ' ')
					{
						//Move to next line if word overflows
						float endOfWordX = ScanToEndOfWord();
						float wordWidth = endOfWordX - mCharHead.X;

						if (endOfWordX > mStyle.mWidth && wordWidth < mStyle.mWidth)
						{
							CRLF();
						}
					}
				}
			}
		}



		/// <summary>
		/// Where should we put the next letter?
		/// </summary>
		Vector2 GetCharPlacementPos()
		{
			return mCharHead + mBottomLeft + GetCharShift();
		}



		/// <summary>
		/// Offset to make the characters place a bit better.
		/// </summary>
		Vector2 GetCharShift()
		{
			return new Vector2(0.0f, -MonoMath.Round(GetNewLineSize() / 4.0f));
		}



		/// <summary>
		/// Should we draw this character?
		/// </summary>
		bool ShouldDrawChar(char charToPrint)
		{
			if (mCharHead.X == 0.0f && charToPrint == ' ')
			{
				return false;
			}

			if (charToPrint == '\n')
			{
				return false;
			}

			return true;
		}


		/// <summary>
		/// Returns true if char ends word
		/// </summary>
		bool CharEndsWord(char charToPrint)
		{
			if (charToPrint == '\n' ||
				charToPrint == ' ' ||
				charToPrint == '\0')
			{
				return true;
			}

			return false;
		}



		/// <summary>
		/// Calculates size of new line.
		/// </summary>
		float GetNewLineSize()
		{
			return mCharHeight + mStyle.mLeading;
		}



		/// <summary>
		/// Calculates size of new line.
		/// </summary>
		float GetCharWidth(char character)
		{
			if (!mStyle.mFont.GetGlyphs().ContainsKey(character))
			{
				return 0.0f;
			}

			return mStyle.mFont.MeasureString(character.ToString()).X + mStyle.mKerning;
		}



		/// <summary>
		/// Scans to the end of the word and finds theoretical X point.
		/// </summary>
		float ScanToEndOfWord()
		{
			int charIndex = mCurrentBlock.GetTextHead() + 1;
			float endOfWordX = mCharHead.X;
			char currentChar = mCurrentBlock.GetTextChar(charIndex);

			while (!CharEndsWord(currentChar))
			{
				charIndex++;
				currentChar = mCurrentBlock.GetTextChar(charIndex);

				if (CharEndsWord(currentChar))
				{
					break;
				}

				endOfWordX += GetCharWidth(currentChar);
			}

			return endOfWordX;
		}


		/// <summary>
		/// Carridge return and line feed.
		/// </summary>
		void CRLF()
		{
			mCharHead.X = 0.0f;
			mCharHead.Y += GetNewLineSize();
		}

		#endregion rPlacement





		#region rUtility

		/// <summary>
		/// Gets the mood of the block
		/// </summary>
		public SmartTextBlock.TextMood GetMood()
		{
			if (!IsStopped())
			{
				return mCurrentBlock.GetMood();
			}

			return SmartTextBlock.TextMood.Normal;
		}


		/// <summary>
		/// Get rectangle bounds
		/// </summary>
		/// <returns></returns>
		public Rect2f GetRectBounds()
		{
			Vector2 min = mTopLeft - 2.0f * new Vector2(PADDING, PADDING);
			Vector2 max = min + new Vector2(mStyle.mWidth + 4 * PADDING, mBottomLeft.Y - mTopLeft.Y + 3.0f * PADDING);
			return new Rect2f(min, max);
		}



		/// <summary>
		/// Stops the text block
		/// </summary>
		public void Stop()
		{
			mCurrentBlock = null;
		}



		/// <summary>
		/// Are we done with the text block?
		/// </summary>
		public bool IsStopped()
		{
			return mCurrentBlock is null;
		}



		/// <summary>
		/// Get current char.
		/// </summary>
		public char GetCurrentChar()
		{
			return mCurrentBlock.GetCurrentTextCharSafe();
		}



		/// <summary>
		/// Called when the end of the string is reached.
		/// </summary>
		private void EndOfStringReached()
		{
			// Terminate current block
			Stop();

			// Queue up next string
			if (GetNumStringsInQueue() > 0)
			{
				string stringID = mPendingStringIDs.Dequeue();
				mCurrentBlock = new SmartTextBlock(stringID, mStyle.mFramesPerLetter);
				CRLF(); // Line feed between blocks
			}
		}


		/// <summary>
		/// How many strings are pending?
		/// </summary>
		public int GetNumStringsInQueue()
		{
			return mPendingStringIDs.Count;
		}



		/// <summary>
		/// Queue up string to be displayed.
		/// </summary>
		public void PushNewString(string stringID)
		{
			mPendingStringIDs.Enqueue(stringID);
		}

		#endregion rUtility
	}
}