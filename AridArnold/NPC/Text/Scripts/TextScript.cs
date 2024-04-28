namespace AridArnold
{
	abstract class TextScript
	{
		SmartTextBlock mParentBlock;
		string[] mArgs;

		public TextScript(SmartTextBlock parentBlock, string[] args)
		{
			mParentBlock = parentBlock;
			mArgs = args;
		}

		protected SmartTextBlock GetSmartTextBlock()
		{
			return mParentBlock;
		}

		public abstract void Update(GameTime gameTime);

		public abstract bool HaltText();

		public abstract bool IsFinished();
	}
}
