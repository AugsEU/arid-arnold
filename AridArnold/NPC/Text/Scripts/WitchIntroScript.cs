
namespace AridArnold
{
	class WitchIntroScript : TextScript
	{
		int mWaterID;

		string mIntro; // Intro string
		string mPostQuenching; // Display if we already did this
		bool mSentScript;


		public WitchIntroScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
			// Make sure args are correct
			MonoDebug.Assert(args.Length == 3);

			mWaterID = int.Parse(args[0], CultureInfo.InvariantCulture);

			mIntro = LanguageManager.I.GetText(args[1]);
			mPostQuenching = LanguageManager.I.GetText(args[2]);
			mSentScript = false;
		}

		bool HasDoneThisAlready()
		{
			return FlagsManager.I.CheckFlag(FlagCategory.kWaterCollected, (uint)mWaterID);
		}

		public override void Update(GameTime gameTime)
		{
			if(mSentScript)
			{
				return;
			}

			if (HasDoneThisAlready())
			{
				GetSmartTextBlock().AppendTextAtHead(mPostQuenching);
			}
			else
			{
				GetSmartTextBlock().AppendTextAtHead(mIntro);
			}

			mSentScript = true;
		}

		public override bool HaltText()
		{
			return false;
		}

		public override bool IsFinished()
		{
			return true;
		}
	}
}
