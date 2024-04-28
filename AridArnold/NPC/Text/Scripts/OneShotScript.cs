
namespace AridArnold
{
	/// <summary>
	/// Script that runs once after enter is pressed
	/// </summary>
	abstract class OneShotScript : WaitForConfirmScript
	{
		bool mDoneOneShot = false;

		public OneShotScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
		}

		public override void Update(GameTime gameTime)
		{
			if(mDoneOneShot)
			{
				return;
			}

			base.Update(gameTime);
			if(ConfirmPressed())
			{
				DoOneShot();
				mDoneOneShot = true;
			}
		}

		public override bool IsFinished()
		{
			return mDoneOneShot;
		}

		protected abstract void DoOneShot();
	}
}
