namespace AridArnold
{
	abstract class TextScript
	{
		SmartTextBlock mParentBlock;

		public TextScript(SmartTextBlock parentBlock)
		{
			mParentBlock = parentBlock;
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
