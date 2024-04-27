
namespace AridArnold
{
	/// <summary>
	/// Script that runs once after enter is pressed
	/// </summary>
	abstract class OneShotScript : WaitForInputScript
	{
		bool mDoneOneShot = false;

		public OneShotScript(SmartTextBlock parentBlock) : base(parentBlock)
		{
		}

		public override void Update(GameTime gameTime)
		{
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
