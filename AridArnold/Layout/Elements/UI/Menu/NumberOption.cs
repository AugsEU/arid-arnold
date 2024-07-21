using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
{
	abstract class NumberOption : MenuOption
	{
		int mMin;
		int mMax;
		int mStep;

		public NumberOption(XmlNode rootNode, Layout parent, int min, int max, int step, int defaultVal = 0) : base(rootNode, parent)
		{
			MonoDebug.Assert((max - min) % step == 0, "Step does not evenly devide range");
			MonoDebug.Assert(max > min, "Maximum is less than or equal to minimum");

			mMin = min;
			mMax = max;
			mStep = step;
		}

		protected void SyncFromInt(int num)
		{
			num -= mMin;
			int index = num / mStep;

			SetSelectedOptionsIdx(index);
		}

		protected override int GetNumOptions()
		{
			return 1 + (mMax - mMin) / mStep;
		}

		protected override string GetOptionStr(int optionIdx)
		{
			return GetValue().ToString();
		}

		protected int GetValue()
		{
			return GetSelectedOptionIdx() * mStep + mMin;
		}
	}
}
