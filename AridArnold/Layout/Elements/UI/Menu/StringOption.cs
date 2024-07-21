using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
{
	abstract class StringOption : MenuOption
	{
		string[] mOptionStrs;

		public StringOption(XmlNode rootNode, Layout parent, string[] optionIDs, bool isLoc = false) : base(rootNode, parent)
		{
			// Parse options and localise if needed
			mOptionStrs = new string[optionIDs.Length];
			for (int i = 0; i < optionIDs.Length; i++)
			{
				string locStr = isLoc ? optionIDs[i] : LanguageManager.I.GetText(optionIDs[i]);
				mOptionStrs[i] = string.Format("<{0}>", locStr);
			}
		}

		protected override int GetNumOptions()
		{
			return mOptionStrs.Length;
		}

		protected override string GetOptionStr(int optionIdx)
		{
			return mOptionStrs[(int)optionIdx];
		}
	}
}
