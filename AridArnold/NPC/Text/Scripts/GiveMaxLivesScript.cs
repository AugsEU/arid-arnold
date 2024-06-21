using System.Globalization;

namespace AridArnold
{
	class GiveMaxLivesScript : OneShotScript
	{
		int mWaterID;
		uint mWaterNeeded;
		string mNotEnoughWater;
		string mFullyQuenched;

		public GiveMaxLivesScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
			// Make sure args are correct
			MonoDebug.Assert(args.Length == 4);

			mWaterID = int.Parse(args[0], CultureInfo.InvariantCulture);
			mWaterNeeded = uint.Parse(args[1], CultureInfo.InvariantCulture);

			mNotEnoughWater = LanguageManager.I.GetText(args[2]);
			mNotEnoughWater = LanguageManager.I.GetText(args[3]);
		}

		protected override void DoOneShot()
		{
			uint numWaterInCollection = CollectableManager.I.GetNumCollected(CollectableCategory.WaterBottle);

			if(numWaterInCollection >= mWaterNeeded)
			{
				//CollectableManager.I.ChangePermanentItem
			}
		}
	}
}
