
namespace AridArnold
{
	/// <summary>
	/// Script that runs once after enter is pressed
	/// </summary>
	class SetFlagScript : TextScript
	{
		bool mHasSetFlag = false;
		FlagCategory mFlagCategory;
		UInt32 mFlagImpl;
		bool mFlagValue;

		public SetFlagScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
			MonoDebug.Assert(args.Length == 3);
			mFlagCategory = MonoEnum.GetEnumFromString<FlagCategory>(args[0]);
			mFlagImpl = uint.Parse(args[1], CultureInfo.InvariantCulture);
			mFlagValue = args[2] == "1" || args[2].ToLower() == "true";
		}

		public override bool HaltText()
		{
			return !mHasSetFlag;
		}

		public override bool IsFinished()
		{
			return mHasSetFlag;
		}

		public override void Update(GameTime gameTime)
		{
			if(!mHasSetFlag)
			{
				FlagsManager.I.SetFlag(mFlagCategory, mFlagImpl, mFlagValue);
				mHasSetFlag = true;
			}
		}

		
	}
}
