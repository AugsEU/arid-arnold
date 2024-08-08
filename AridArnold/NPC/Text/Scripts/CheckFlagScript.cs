

namespace AridArnold
{
	class CheckFlagScript : ConditionalTextScript
	{
		FlagCategory mFlagCategory;
		uint mFlagImpl;

		public CheckFlagScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, MonoAlg.GetSubArray(args, 2, 2))
		{
			mFlagCategory = MonoEnum.GetEnumFromString<FlagCategory>(args[0]);
			mFlagImpl = uint.Parse(args[1], CultureInfo.InvariantCulture);
		}

		protected override bool CheckCondition()
		{
			return FlagsManager.I.CheckFlag(mFlagCategory, mFlagImpl);
		}
	}
}
