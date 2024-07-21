
namespace AridArnold
{
	abstract class PercentageOption : NumberOption
	{
		protected PercentageOption(XmlNode rootNode, Layout parent, int step = 5, int defaultVal = 50) : base(rootNode, parent, 0, 100, step, defaultVal)
		{
		}

		protected void SyncFromFloat(float percent)
		{
			percent *= 100.0f;
			int percentInt = (int)Math.Round(percent);
			SyncFromInt(percentInt);
		}

		protected float GetPercentage()
		{
			return (float)GetValue() / 100.0f;
		}
	}
}
