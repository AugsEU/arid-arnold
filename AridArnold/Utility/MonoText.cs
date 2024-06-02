using System.Text;

namespace AridArnold
{
	/// <summary>
	/// Text related utility functions
	/// </summary>
	static class MonoText
	{
		public static char[] ALPHABET = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

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
	}
}
