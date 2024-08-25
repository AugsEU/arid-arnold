#define ADD_TEMP_FILES

using System.Diagnostics;

namespace AridArnold
{
	internal class LanguageManager : Singleton<LanguageManager>
	{
		#region rType

		public enum LanguageType
		{
			None,
			English,
			French,
			Italian,
			German,
			Spanish
		}

		#endregion rType



		#region rConstants

#if ADD_TEMP_FILES
		const string TEMP_FILE_OUTPUT_PATH = "C:\\Users\\Augus\\Documents\\Programming_Stuff\\Games\\arid-arnold\\AridArnold\\";
#endif

		#endregion rConstants




		#region rMembers

		LanguageType mLanguageType = LanguageType.None;
		Dictionary<string, string> mKeyCache = new Dictionary<string, string>();
		HashSet<string> mInvalidKeys = new HashSet<string>();


		#endregion rMembers





		#region rText

		public void LoadLanguage(LanguageType languageType)
		{
			mLanguageType = languageType;

			mKeyCache.Clear();
			string[] filePaths = Directory.GetFiles(GetBasePath(), "*.txt", SearchOption.AllDirectories);

			foreach (string filePath in filePaths)
			{
				string rawText = File.ReadAllText(filePath);
				string ID = MonoData.I.StripBasePath(GetBasePath(), filePath);
				ID = ID.Substring(0, ID.Length - 4);//Remove .txt
				ID = ID.Replace('\\', '.');

				mKeyCache[ID] = rawText;
			}
		}



		/// <summary>
		/// Gets path to text file.
		/// </summary>
		public string GetTextPath(string ID)
		{
			ID = ID.Replace('.', '\\');
			return Path.Join(GetBasePath(), "\\" + ID + ".txt");
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public string GetBasePath()
		{
			return Path.Join("Content\\Text\\", GetLanguageCode());
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
			if (ID == "")
			{
				return "";
			}

			string rawText = "";

			if (!mKeyCache.TryGetValue(ID, out rawText))
			{
				string path = GetTextPath(ID);
				if(File.Exists(path))
				{
					rawText = File.ReadAllText(GetTextPath(ID));
				}
				else
				{
#if ADD_TEMP_FILES
					path = Path.Join(TEMP_FILE_OUTPUT_PATH, path);
					rawText = "TEMP TEXT";
					File.WriteAllText(path, rawText);
					Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
					MonoDebug.Log("Created translation file: {0}", path);
#else
					throw new Exception("Invalid string {0}", ID);
#endif
				}
			}

			return TextPreprocessor.Process(rawText);
		}



		/// <summary>
		/// Does the key exist in our loaded language?
		/// </summary>
		public bool KeyExists(string ID)
		{
			// Note: Cache is always complete.
			return mKeyCache.ContainsKey(ID);
		}

#endregion rText
	}
}
