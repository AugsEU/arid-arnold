using Microsoft.Xna.Framework;

namespace AridArnold
{
	struct TextBoxStyle
	{
		public SpriteFont mFont;
		public float mLeading;
		public float mKerning;

		static public TextBoxStyle DefaultStyle
		{
			get
			{
				TextBoxStyle ret = new TextBoxStyle();
				ret.mFont = FontManager.I.GetFont("Pixica-12");
				ret.mLeading = 8.0f;
				ret.mKerning = 1.0f;
				return ret;
			}
		}
	}

	/// <summary>
	/// Simply renders text with animated letters
	/// </summary>
	internal class SimpleTextBoxRenderer
	{
		#region rMembers

		// Text
		SmartTextBlock mTextBlock;
		List<SpeechBoxLetter> mLetters;
		TextBoxStyle mStyle;

		//Transform
		Vector2 mPosition;
		float mWidth;

		//Internal Data
		float mCharHeight;

		#endregion rMembers


		#region rInitialisation

		/// <summary>
		/// Construct speech box renderer
		/// </summary>
		public SimpleTextBoxRenderer(string stringID, Vector2 pos, float width, TextBoxStyle style)
		{
			mPosition = pos;
			mWidth = width;

			mStyle = style;
			mTextBlock = new SmartTextBlock(stringID, 0);
			mLetters = new List<SpeechBoxLetter>();

			mCharHeight = mStyle.mFont.MeasureString("M").Y;
			mCharHeight /= 2.0f;
			PopulateLetters();
		}



		/// <summary>
		/// Parse text and populate the letters
		/// </summary>
		void PopulateLetters()
		{
			Vector2 charHead = Vector2.Zero;

			while (true)
			{
				bool newLetter = mTextBlock.AdvanceTimeStep();

				if(mTextBlock.GetCurrentTextChar() == '\0')
				{
					break;
				}

				if (newLetter)
				{
					char charToPrint = mTextBlock.GetCurrentTextChar();
					bool skipChar = (charHead.X == 0.0f && charToPrint == ' '); // Skip spaces

					if (charToPrint == '\n')
					{
						CRLF(ref charHead);
					}
					else if (!skipChar)
					{
						mLetters.Add(new SpeechBoxLetter(mStyle.mFont, charToPrint, mPosition + charHead, mTextBlock.GetAnimation()));

						charHead.X += GetCharWidth(charToPrint);

						if (charToPrint == ' ')
						{
							//Move to next line if word overflows
							float endOfWordX = ScanToEndOfWord(charHead);
							float wordWidth = endOfWordX - charHead.X;

							if (endOfWordX > mWidth && wordWidth < mWidth)
							{
								CRLF(ref charHead);
							}
						}
					}
				}
			}
		}



		/// <summary>
		/// Carridge return and line feed.
		/// </summary>
		void CRLF(ref Vector2 charHead)
		{
			charHead.X = 0.0f;
			charHead.Y += GetNewLineSize();
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
		float ScanToEndOfWord(Vector2 charHead)
		{
			int charIndex = mTextBlock.GetTextHead() + 1;
			float endOfWordX = charHead.X;
			char currentChar = mTextBlock.GetTextChar(charIndex);

			while (!MonoText.LineBreakOnChar(currentChar))
			{
				charIndex++;
				currentChar = mTextBlock.GetTextChar(charIndex);

				if (MonoText.LineBreakOnChar(currentChar))
				{
					break;
				}

				endOfWordX += GetCharWidth(currentChar);
			}

			return endOfWordX;
		}

		#endregion rInitialisation


		#region rUpdate

		/// <summary>
		/// Update animations for letters
		/// </summary>
		public void Update(GameTime gameTime)
		{
			foreach(SpeechBoxLetter letter in mLetters)
			{
				letter.Update(gameTime);
			}
		}

		#endregion rUpdate




		#region rDraw

		/// <summary>
		/// Draw the text
		/// </summary>
		public void Draw(DrawInfo info)
		{
			foreach (SpeechBoxLetter letter in mLetters)
			{
				letter.Draw(info);
			}
		}

		#endregion rDraw
	}
}
