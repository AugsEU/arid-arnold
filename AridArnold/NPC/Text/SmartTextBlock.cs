namespace AridArnold
{
	internal class SmartTextBlock
	{
		#region rTypes

		public enum TextMood
		{
			Undecided,
			Normal,
			Exclaim,
			Question
		}

		#endregion rTypes





		#region rMembers

		string mText; // Raw text
		string mSanitisedText; // Text without the control characters.
		int mRawCharHead;
		int mSanitisedCharHead;
		int mLetterFrameCount;
		int mDefaultFrameCount;
		TextMood mMood;
		LetterAnimation mAnimation;

		#endregion rMembers





		#region rInitialise

		/// <summary>
		/// Initialise using a file path
		/// </summary>
		public SmartTextBlock(string stringID, int defaultLetterTime)
		{
			mText = LanguageManager.I.GetText(stringID);
			mRawCharHead = -1;
			mLetterFrameCount = 0;
			mDefaultFrameCount = defaultLetterTime;
			mMood = TextMood.Undecided;
			mAnimation = new LetterAnimColor(Color.White);
			CalculateMood();

			mSanitisedText = SanitiseText(ref mText);

			mSanitisedCharHead = mRawCharHead;
		}

		#endregion rInitialise





		#region rUpdate

		/// <summary>
		/// Move forward 1 time step.(Independant of frame time)
		/// </summary>
		/// <returns>True if we have a new letter.</returns>
		public bool AdvanceTimeStep()
		{
			mLetterFrameCount--;

			if (mLetterFrameCount <= 0)
			{
				AdvanceCharacter();
				return true;
			}

			return false;
		}

		#endregion rUpdate





		#region rInterface


		/// <summary>
		/// Get char at index.(Ignoring control characters)
		/// </summary>
		public int GetTextHead()
		{
			return mSanitisedCharHead;
		}



		/// <summary>
		/// Get char at index.(Ignoring control characters)
		/// </summary>
		public char GetTextChar(int index)
		{
			if (index < mSanitisedText.Length)
			{
				return mSanitisedText[index];
			}

			return '\0';
		}



		/// <summary>
		/// Get the current character(Ignoring control characters)
		/// </summary>
		public char GetCurrentTextChar()
		{
			if (mSanitisedCharHead < mSanitisedText.Length)
			{
				return mSanitisedText[mSanitisedCharHead];
			}

			return '\0';
		}


		/// <summary>
		/// Get the current character
		/// </summary>
		public char GetCurrentTextCharSafe()
		{
			if (0 < mSanitisedCharHead && mSanitisedCharHead < mSanitisedText.Length)
			{
				return mSanitisedText[mSanitisedCharHead];
			}

			return '\0';
		}



		/// <summary>
		/// Get the current mood of the text.
		/// </summary>
		public TextMood GetMood()
		{
			return mMood;
		}

		#endregion rInterface





		#region rAnalysis

		/// <summary>
		/// Move to the next character.
		/// </summary>
		void AdvanceCharacter()
		{
			mRawCharHead++;
			if (mRawCharHead >= mText.Length)
			{
				mRawCharHead = mText.Length;
			}

			mSanitisedCharHead++;
			if(mSanitisedCharHead >= mSanitisedText.Length)
			{
				mSanitisedCharHead = mSanitisedText.Length;
			}

			ParseControlCharacters();

			if (IsDecisionLetter(GetCurrentChar()))
			{
				CalculateMood();
			}

			mLetterFrameCount = CalculateLetterTime(GetCurrentChar());

			if(GetCurrentChar() != GetCurrentTextChar())
			{
				MonoDebug.DLog("DLOG: {0} vs {1}", GetCurrentChar(), GetCurrentTextChar());
			}
			System.Diagnostics.Debug.Assert(GetCurrentChar() == GetCurrentTextChar());
		}



		/// <summary>
		/// Analyse text to find what mood it has.
		/// </summary>
		void CalculateMood()
		{
			int forwardHead = mRawCharHead + 1;
			TextMood textMood = TextMood.Normal;

			//Assume all caps until proven otherwise
			bool allCaps = true;
			int wordLength = 0;

			//Was there anything to analyse?
			bool analysis = false;

			while (forwardHead < mText.Length && !IsDecisionLetter(mText[forwardHead]))
			{
				//There is content to analyse
				analysis = true;
				char letterToAnalyse = mText[forwardHead];

				//Check for all caps.
				if (!char.IsUpper(letterToAnalyse) && char.IsLetter(letterToAnalyse))
				{
					allCaps = false;
				}

				forwardHead++;
				wordLength++;
			}

			forwardHead--;

			if (analysis)
			{
				// All caps words are considered exclamations.
				if (allCaps && wordLength > 1)
				{
					textMood = TextMood.Exclaim;
				}
				else if (forwardHead < mText.Length)
				{
					//Look at punctuation
					switch (mText[forwardHead])
					{
						case '!':
							textMood = TextMood.Exclaim;
							break;
						case '?':
							textMood = TextMood.Question;
							break;
						default:
							break;
					}
				}
			}

			mMood = textMood;
		}



		/// <summary>
		/// Get number of frames a particular letter should display for.
		/// </summary>
		int CalculateLetterTime(char letter)
		{
			switch (letter)
			{
				case '?':
					return mDefaultFrameCount * 8;
				case '!':
					return mDefaultFrameCount * 7;
				case '.':
					return mDefaultFrameCount * 6;
				case ',':
					return mDefaultFrameCount * 3;
				case ' ':
					return mDefaultFrameCount / 2;
				default:
					break;
			}

			return mDefaultFrameCount;
		}



		/// <summary>
		/// Do we need to re-calculate the mood on this letter?
		/// </summary>
		bool IsDecisionLetter(char letter)
		{
			return letter == ' ' || letter == '\n' || letter == '\0';
		}


		/// <summary>
		/// Gets animation.
		/// </summary>
		public LetterAnimation GetAnimation()
		{
			return mAnimation;
		}

		#endregion rAnalysis





		#region rControlCharacters

		/// <summary>
		/// Check for control character and add text animations as needed.
		/// </summary>
		void ParseControlCharacters()
		{
			while(true)
			{
				if (mRawCharHead >= mText.Length)
				{
					mRawCharHead = mText.Length;
					break;
				}

				if (GetCurrentChar() == '<')
				{
					mRawCharHead++;
					char controlChar = GetCurrentChar();

					switch (controlChar)
					{
						case 'α':
							mAnimation = new LetterAnimColor(ParseColor());
							break;
						case 'ψ':
							mAnimation = new LetterAnimColor(Color.White);
							mRawCharHead++;
							break;
						case 'γ':
							mAnimation = new LetterAnimColor(Color.Yellow);
							mRawCharHead++;
							break;
						case 'Σ':
							ParseShaker();
							break;
						default:
							return;
					}

					if (GetCurrentChar() != '>')
					{
						throw new FormatException("INVALID TEXT FORMAT! DID NOT END CONTROL SEQUENCE!!");
					}
					mRawCharHead++;
				}
				else
				{
					break;
				}
			}
		}

		/// <summary>
		/// Parse the text at the char head as a color.
		/// </summary>
		Color ParseColor()
		{
			char magic = mText[mRawCharHead];
			if(magic != 'α')
			{
				return Color.White;
			}

			mRawCharHead+=2;
			int r = ParseNumber();
			int g = ParseNumber();
			int b = ParseNumber();
			mRawCharHead++;
			
			return new Color(r, g, b);
		}

		/// <summary>
		/// Parse the text at the char head as a 2-hexit number(little-endian).
		/// </summary>
		int ParseNumber()
		{
			string number = mText.Substring(mRawCharHead, 2);
			mRawCharHead += 2;
			return Convert.ToInt32(number, 16);
		}

		/// <summary>
		/// Parse the text at the char head as a text shaker
		/// </summary>
		void ParseShaker()
		{
			mRawCharHead++;
			Color col = ParseColor();

			int intensity = ParseNumber();
			int frame = ParseNumber();

			mAnimation = new LetterAnimShaking(intensity, frame, col);
		}



		/// <summary>
		/// Take in raw text and remove control characters.
		/// </summary>
		string SanitiseText(ref string rawText)
		{
			string result = "";
			bool inControlSequence = false;
			for(int i = 0; i < rawText.Length; i++)
			{
				char currentChar = rawText[i];

				if (currentChar == '<')
				{
					inControlSequence = true;
				}

				if(!inControlSequence)
				{
					result += currentChar;
				}

				if (currentChar == '>')
				{
					inControlSequence = false;
				}
			}

			if(inControlSequence == true)
			{
				throw new FormatException("INVALID TEXT FORMAT! DID NOT END CONTROL SEQUENCE!!");
			}

			return result;
		}

		#endregion rControlCharacters





		#region rUtility

		/// <summary>
		/// Get the current character
		/// </summary>
		private char GetCurrentChar()
		{
			if (mRawCharHead < mText.Length)
			{
				return mText[mRawCharHead];
			}

			return '\0';
		}

		#endregion rUtility
	}
}
