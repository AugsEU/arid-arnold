namespace GMTK2023
{
	/// <summary>
	/// Text related utility functions
	/// </summary>
	static class MonoText
	{
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
	}
}
