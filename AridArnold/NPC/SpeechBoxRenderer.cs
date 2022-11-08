namespace AridArnold
{
	struct SpeechBoxStyle
	{
		public SpriteFont mFont;
		public Texture2D mSpikeTexture;
		public float mLeading;
		public float mKerning;
		public float mWidth;
		public float mSpeed;
	}

	/// <summary>
	/// Class for rendering speech boxes.
	/// </summary> 
	internal class SpeechBoxRenderer
	{
		#region rConstants

		static Color BG_COLOR = new Color(0, 10, 20, 200);
		static Color BORDER_COLOR = new Color(56, 89, 122);
		static Color TEXT_COLOR = Color.Wheat;

		static int PADDING = 5;
		static int BORDER_WIDTH = 2;

		#endregion rConstants

		#region rMembers

		// Text
		SmartTextBlock mCurrentBlock;
		List<SpeechBoxLetter> mLetters;
		SpeechBoxStyle mStyle;

		//Transform
		Vector2 mBottomLeft;
		Vector2 mTopLeft;

		//Internal Data
		Vector2 mCharHead;
		Vector2 mCharSize;

		#endregion rMembers






		#region rInitialisation

		/// <summary>
		/// Construct speech box renderer
		/// </summary>
		public SpeechBoxRenderer(string stringID, Vector2 bottomLeft, SpeechBoxStyle style)
		{
			mStyle = style;
			mCurrentBlock = new SmartTextBlock(stringID, 20); //TO DO: Calculate text speed as a function of scroll speed.
			mLetters = new List<SpeechBoxLetter>();
			mBottomLeft = bottomLeft;

			mCharSize = mStyle.mFont.MeasureString("M");
			mCharSize.Y /= 2.0f;
			mCharHead = new Vector2(0.0f , -GetNewLineSize());
			mTopLeft = mBottomLeft + mCharHead;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update text box.
		/// </summary>
		public void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
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
		}



		/// <summary>
		/// Scroll all the letters up by a certain amount.
		/// </summary>
		void ScrollLettersUp(float dt)
		{
			float dy = -dt * mStyle.mSpeed;
			foreach (SpeechBoxLetter letter in mLetters)
			{
				letter.MoveUp(-dy);
			}

			mCharHead.Y += dy;
			mTopLeft.Y += dy;

			if(IsStopped())
			{
				mBottomLeft.Y += dy;
			}
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
		/// 
		/// </summary>
		public void DrawBox(DrawInfo info)
		{
			Point rectPosition = new Point((int)mTopLeft.X - PADDING, (int)mTopLeft.Y - PADDING);
			int height = (int)(mBottomLeft.Y - mTopLeft.Y) + PADDING;
			int width = (int)mStyle.mWidth + 2 * PADDING;
			Rectangle bgRectangle = new Rectangle(rectPosition.X , rectPosition.Y, width, height);

			// Draw bg
			MonoDraw.DrawRectDepth(info, bgRectangle, BG_COLOR, MonoDraw.LAYER_TEXT_BOX);

			// Draw borders
			Rectangle topRect = new Rectangle(rectPosition.X - BORDER_WIDTH / 2, rectPosition.Y - BORDER_WIDTH, width + BORDER_WIDTH, BORDER_WIDTH);
			Rectangle bottomRect = new Rectangle(rectPosition.X - BORDER_WIDTH / 2, rectPosition.Y + height, width + BORDER_WIDTH, BORDER_WIDTH);
			Rectangle leftRect = new Rectangle(rectPosition.X - BORDER_WIDTH, rectPosition.Y - BORDER_WIDTH / 2, BORDER_WIDTH, height + BORDER_WIDTH);
			Rectangle rightRect = new Rectangle(rectPosition.X + width, rectPosition.Y - BORDER_WIDTH / 2, BORDER_WIDTH, height + BORDER_WIDTH);

			MonoDraw.DrawRectDepth(info, topRect, BORDER_COLOR, MonoDraw.LAYER_TEXT_BOX);
			MonoDraw.DrawRectDepth(info, bottomRect, BORDER_COLOR, MonoDraw.LAYER_TEXT_BOX);
			MonoDraw.DrawRectDepth(info, leftRect, BORDER_COLOR, MonoDraw.LAYER_TEXT_BOX);
			MonoDraw.DrawRectDepth(info, rightRect, BORDER_COLOR, MonoDraw.LAYER_TEXT_BOX);

			if (!IsStopped())
			{
				Vector2 spikePos = new Vector2(rectPosition.X + 30, rectPosition.Y + height);
				MonoDraw.DrawTextureDepth(info, mStyle.mSpikeTexture, spikePos, MonoDraw.LAYER_TEXT_BOX + MonoDraw.FRONT_EPSILON);
			}
		}

		#endregion rDraw





		#region rPlacement

		/// <summary>
		/// Place new letter.
		/// </summary>
		void PlaceNewLetter()
		{
			char charToPrint = mCurrentBlock.GetCurrentChar();

			if(charToPrint == '\0')
			{
				Stop();
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
					mLetters.Add(new SpeechBoxLetter(mStyle.mFont, charToPrint, mCharHead + mBottomLeft + new Vector2(0.0f, -GetNewLineSize() / 2.0f), TEXT_COLOR));
					mCharHead.X += GetCharWidth();

					if(charToPrint == ' ')
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
		/// Should we draw this character?
		/// </summary>
		bool ShouldDrawChar(char charToPrint)
		{
			if(mCharHead.X == 0.0f && charToPrint == ' ')
			{
				return false;
			}

			if(charToPrint == '\n')
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
			return mCharSize.Y + mStyle.mLeading;
		}



		/// <summary>
		/// Calculates size of new line.
		/// </summary>
		float GetCharWidth()
		{
			return mCharSize.X + mStyle.mKerning;
		}



		/// <summary>
		/// Scans to the end of the word and finds theoretical X point.
		/// </summary>
		float ScanToEndOfWord()
		{
			int charIndex = mCurrentBlock.GetHead() + 1;
			float endOfWordX = mCharHead.X;
			char currentChar = mCurrentBlock.GetChar(charIndex);

			while (!CharEndsWord(currentChar))
			{
				charIndex++;
				currentChar = mCurrentBlock.GetChar(charIndex);

				if (CharEndsWord(currentChar))
				{
					break;
				}

				endOfWordX += GetCharWidth();
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
			if(!IsStopped())
			{
				return mCurrentBlock.GetMood();
			}

			return SmartTextBlock.TextMood.Normal;
		}

		#endregion rUtility
	}
}