
namespace AridArnold
{
	abstract class ConditionalTextScript : TextScript
	{
		string mCondFalseStr; // Display if condition is false
		string mCondTrueStr; // Display if condition is true
		bool mSentScript;


		public ConditionalTextScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
			// Make sure args are correct
			MonoDebug.Assert(args.Length == 2);

			mCondTrueStr = LanguageManager.I.GetText(args[0]);
			mCondFalseStr = LanguageManager.I.GetText(args[1]);

			mSentScript = false;
		}

		protected abstract bool CheckCondition();

		public override void Update(GameTime gameTime)
		{
			if(mSentScript)
			{
				return;
			}

			if (CheckCondition())
			{
				GetSmartTextBlock().AppendTextAtHead(mCondTrueStr);
			}
			else
			{
				GetSmartTextBlock().AppendTextAtHead(mCondFalseStr);
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
