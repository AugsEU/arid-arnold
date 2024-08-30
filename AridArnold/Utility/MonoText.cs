using System.Text;

namespace AridArnold
{
	/// <summary>
	/// Text related utility functions
	/// </summary>
	static class MonoText
	{
		public static char[] ALPHABET = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' '};

		/// <summary>
		/// Returns true if the letter is a vowel.
		/// </summary>
		public static bool IsVowel(char character)
		{
			switch (char.ToLower(character))
			{
				case 'a':
				case 'e':
				case 'i':
				case 'o':
				case 'u':
				case 'y':
					return true;
			}

			return false;
		}



		/// <summary>
		/// Can we line break on this character?
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public static bool LineBreakOnChar(char character)
		{
			return character == '\n' ||
			character == ' ' ||
			character == '\0';
		}



		/// <summary>
		/// Go to next letter in alphabet
		/// </summary>
		public static char IncrementInAlphabet(char character)
		{
			for(int i = 0; i  < ALPHABET.Length; i++)
			{
				if(ALPHABET[i] == character)
				{
					int idx = (i + 1) % ALPHABET.Length;
					return ALPHABET[idx];
				}
			}

			throw new Exception("Can't increment non-alphabet character");
		}



		/// <summary>
		/// Go to previous letter in alphabet
		/// </summary>
		public static char DecrementInAlphabet(char character)
		{
			for (int i = 0; i < ALPHABET.Length; i++)
			{
				if (ALPHABET[i] == character)
				{
					int idx = (i + ALPHABET.Length - 1) % ALPHABET.Length;
					return ALPHABET[idx];
				}
			}

			throw new Exception("Can't increment non-alphabet character");
		}



		/// <summary>
		/// Replace single char in string
		/// </summary>
		public static void ReplaceChar(ref string str, char newChar, int index)
		{
			if (index < 0 || index >= str.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the string.");
			}

			StringBuilder sb = new StringBuilder(str);
			sb[index] = newChar;
			str = sb.ToString();
		}



		/// <summary>
		/// Convert frame count into a time string
		/// </summary>
		public static string GetTimeTextFromFrames(int frame)
		{
			int ms = (int)(frame * (1000.0f / 60.0f));
			int cs = ms / 10;
			int s = cs / 100;
			int m = s / 60;


			if (m == 0)
			{
				return string.Format("{0:D2} : {1:D2}", s, cs % 100);
			}

			return string.Format("{0:D} : {1:D2} : {2:D2}", m, s % 60, cs % 100);
		}


		/// <summary>
		/// Convert frame count into a time string
		/// </summary>
		public static string GetTimeTextFromFrames(UInt64 frame)
		{
			UInt64 ms = (UInt64)(frame * (1000.0 / 60.0));
			UInt64 cs = ms / 10;
			UInt64 s = cs / 100;
			UInt64 m = s / 60;


			if (m == 0)
			{
				return string.Format("{0:D2} : {1:D2}", s, cs % 100);
			}

			return string.Format("{0:D} : {1:D2} : {2:D2}", m, s % 60, cs % 100);
		}
	}
}
