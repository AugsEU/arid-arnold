﻿using System.Text.RegularExpressions;

namespace AridArnold
{
	internal class EProfileNameOption : StringOption
	{
		/// Weird hack due to init order so this has to be static
		static string[] mPossibleProfileNames = null;

		public EProfileNameOption(XmlNode rootNode, Layout parent) : base(rootNode, parent, GenerateProfileNameList(), true)
		{
		}

		protected override void OnOptionSelect(int optionIdx)
		{
			SaveManager.I.NewProfileName(GetOptionStr(optionIdx));
		}

		protected override void SyncOption()
		{
			// Don't sync
		}

		static string[] GenerateProfileNameList()
		{
			// This has to be localisable
			string rawListStr = LanguageManager.I.GetText("UI.ProfileNames");
			string sanListStr = Regex.Replace(rawListStr, @"\t|\n|\r", "");
			mPossibleProfileNames = sanListStr.Split(",");
			return mPossibleProfileNames;
		}
	}
}
