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

		string mText;
		int mCharHead;
		int mLetterFrameCount;
		int mDefaultFrameCount;
		TextMood mMood;

		#endregion rMembers





		#region rInitialise

		/// <summary>
		/// Initialise using a file path
		/// </summary>
		public SmartTextBlock(string stringID, int defaultLetterTime)
		{
			mText = LanguageManager.I.GetText(stringID);
			mCharHead = -1;
			mLetterFrameCount = 0;
			mDefaultFrameCount = defaultLetterTime;
			mMood = TextMood.Undecided;
			CalculateMood();
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
		/// Get head index.
		/// </summary>
		public int GetHead()
		{
			return mCharHead;
		}



		/// <summary>
		/// Get char at index.
		/// </summary>
		public char GetChar(int index)
		{
			if (index < mText.Length)
			{
				return mText[index];
			}

			return '\0';
		}



		/// <summary>
		/// Get the current character
		/// </summary>
		public char GetCurrentChar()
		{
			if(mCharHead < mText.Length)
			{
				return mText[mCharHead];
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
			mCharHead++;
			if(mCharHead >= mText.Length)
			{
				mCharHead = mText.Length;
			}

			if(IsDecisionLetter(GetCurrentChar()))
			{
				CalculateMood();
			}

			mLetterFrameCount = CalculateLetterTime(GetCurrentChar());
		}



		/// <summary>
		/// Analyse text to find what mood it has.
		/// </summary>
		void CalculateMood()
		{
			int forwardHead = mCharHead+1;
			TextMood textMood = TextMood.Normal;

			//Assume all caps until proven otherwise
			bool allCaps = true;

			//Was there anything to analyse?
			bool analysis = false;

			while(forwardHead < mText.Length && !IsDecisionLetter(mText[forwardHead]))
			{
				//There is content to analyse
				analysis = true;
				char letterToAnalyse = mText[forwardHead];
				
				//Check for all caps.
				if(!char.IsUpper(letterToAnalyse) && char.IsLetter(letterToAnalyse))
				{
					allCaps = false;
				}

				forwardHead++;
			}

			forwardHead--;

			if (analysis)
			{
				// All caps words are considered exclamations.
				if (allCaps)
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
					return mDefaultFrameCount + 140;
				case '!':
					return mDefaultFrameCount + 120;
				case '.':
					return mDefaultFrameCount + 100;
				case ',':
					return mDefaultFrameCount + 40;
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

		#endregion rAnalysis
	}
}
