namespace AridArnold
{
	internal class LanguageManager : Singleton<LanguageManager>
	{
		#region rType

		enum LanguageType
		{
			English,
			French,
			Italian,
			German,
			Spanish
		}

		#endregion rType





		#region rMembers

		LanguageType mLanguageType = LanguageType.English;

		#endregion rMembers





		#region rText

		/// <summary>
		/// Gets path to text file.
		/// </summary>
		public string GetTextPath(string ID)
		{
			ID = ID.Replace('.', '\\');
			return "Content\\Text\\" + GetLanguageCode() + "\\" + ID;
		}



		/// <summary>
		/// Gets current language code.
		/// </summary>
		string GetLanguageCode()
		{
			switch (mLanguageType)
			{
				case LanguageType.English:
					return "EN";
				case LanguageType.French:
					return "FR";
				case LanguageType.Italian:
					return "IT";
				case LanguageType.German:
					return "DE";
				case LanguageType.Spanish:
					return "ES";
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Get text block corresponding to ID.
		/// </summary>
		public string GetText(string ID)
		{
			return File.ReadAllText(GetTextPath(ID));
		}

		#endregion rText
	}
}
