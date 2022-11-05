namespace AridArnold
{
	/// <summary>
	/// Class for rendering speech boxes.
	/// </summary>
	internal class SpeechBoxRenderer
	{
		#region rConstants

		static Color BG_COLOR = new Color(0, 10, 20, 100);
		static Color BORDER_COLOR = new Color(56, 89, 122);
		static Color TEXT_COLOR = Color.Wheat;

		static int PADDING = 5;
		static int BORDER_WIDTH = 2;

		#endregion rConstants

		#region rMembers

		// Text
		SmartTextBlock mCurrentBlock;
		List<SpeechBoxLetter> mLetters;

		//Transform
		float mWidth;
		Vector2 mBottomLeft;
		Vector2 mTopLeft;
		float mLeading;
		float mKerning;
		float mSpeed;

		//Internal Data
		Vector2 mCharHead;
		Vector2 mCharSize;

		//Textures
		SpriteFont mFont;
		Texture2D mSpikeTexture;

		#endregion rMembers






		#region rInitialisation

		/// <summary>
		/// Construct speech box renderer
		/// </summary>
		public SpeechBoxRenderer(SmartTextBlock textBlock, float width, float scrollSpeed, Vector2 bottomLeft, SpriteFont font, float leading, float kerning)
		{
			mFont = font;
			mCurrentBlock = textBlock;
			mWidth = width;
			mLetters = new List<SpeechBoxLetter>();
			mSpeed = scrollSpeed;
			mBottomLeft = bottomLeft;

			mCharSize = font.MeasureString("M");
			mCharSize.Y /= 2.0f;
			mCharHead = new Vector2(0.0f , -GetNewLineSize());
			mTopLeft = mBottomLeft + mCharHead;
			mLeading = leading;
			mKerning = kerning;
		}



		/// <summary>
		/// Load content needed for speech box renderer
		/// </summary>
		public void LoadContent(ContentManager content)
		{
			mSpikeTexture = content.Load<Texture2D>("NPC/Dialog/DialogSpike");
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

			if ((mCharHead.Y + GetNewLineSize()) < 0.0f)
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
			foreach(SpeechBoxLetter letter in mLetters)
			{
				letter.MoveUp(dt * mSpeed);
			}

			mCharHead.Y -= dt * mSpeed;
			mTopLeft.Y -= dt * mSpeed;
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
			Point rectPosition = new Point((int)mTopLeft.X - PADDING, (int)mTopLeft.Y);
			int height = (int)(mBottomLeft.Y - mTopLeft.Y);
			int width = (int)mWidth + 2 * PADDING;
			Rectangle bgRectangle = new Rectangle(rectPosition.X , rectPosition.Y, width, height);

			// Draw bg
			Util.DrawRect(info, bgRectangle, BG_COLOR);

			// Draw borders
			Rectangle topRect = new Rectangle(rectPosition.X - BORDER_WIDTH / 2, rectPosition.Y - BORDER_WIDTH, width + BORDER_WIDTH, BORDER_WIDTH);
			Rectangle bottomRect = new Rectangle(rectPosition.X - BORDER_WIDTH / 2, rectPosition.Y + height, width + BORDER_WIDTH, BORDER_WIDTH);
			Rectangle leftRect = new Rectangle(rectPosition.X - BORDER_WIDTH, rectPosition.Y - BORDER_WIDTH / 2, BORDER_WIDTH, height + BORDER_WIDTH);
			Rectangle rightRect = new Rectangle(rectPosition.X + width, rectPosition.Y - BORDER_WIDTH / 2, BORDER_WIDTH, height + BORDER_WIDTH);

			Util.DrawRect(info, topRect, BORDER_COLOR);
			Util.DrawRect(info, bottomRect, BORDER_COLOR);
			Util.DrawRect(info, leftRect, BORDER_COLOR);
			Util.DrawRect(info, rightRect, BORDER_COLOR);

			Vector2 spikePos = new Vector2(rectPosition.X + 30, rectPosition.Y + height);
			info.spriteBatch.Draw(mSpikeTexture, spikePos, Color.White);
		}

		#endregion rDraw





		#region rPlacement

		/// <summary>
		/// Place new letter.
		/// </summary>
		void PlaceNewLetter()
		{
			char charToPrint = mCurrentBlock.GetCurrentChar();

			if (charToPrint == '\n')
			{
				CRLF();
			}
			else
			{
				if (ShouldDrawChar(charToPrint))
				{
					mLetters.Add(new SpeechBoxLetter(mFont, charToPrint, mCharHead + mBottomLeft, TEXT_COLOR));
					mCharHead.X += GetCharWidth();

					if(charToPrint == ' ')
					{
						//Move to next line if word overflows
						float endOfWordX = ScanToEndOfWord();
						float wordWidth = endOfWordX - mCharHead.X;

						if (endOfWordX > mWidth && wordWidth < mWidth)
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
		/// Calculates size of new line.
		/// </summary>
		float GetNewLineSize()
		{
			return mCharSize.Y + mLeading;
		}



		/// <summary>
		/// Calculates size of new line.
		/// </summary>
		float GetCharWidth()
		{
			return mCharSize.X + mKerning;
		}



		/// <summary>
		/// Scans to the end of the word and finds theoretical X point.
		/// </summary>
		float ScanToEndOfWord()
		{
			int charIndex = mCurrentBlock.GetHead() + 1;
			float endOfWordX = mCharHead.X;
			char currentChar = mCurrentBlock.GetChar(charIndex);

			while(currentChar != ' ' && currentChar != '\0')
			{
				charIndex++;
				currentChar = mCurrentBlock.GetChar(charIndex);
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
	}
}