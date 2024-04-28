namespace AridArnold
{
	/// <summary>
	/// One shot script that takes "Confirm" or "Decline"
	/// </summary>
	abstract class ConditionalOneShotScript : WaitForConfirmDeclineScript
	{
		bool mDoneOneShot = false;

		public ConditionalOneShotScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
		}

		public override void Update(GameTime gameTime)
		{
			if(mDoneOneShot)
			{
				return;
			}

			base.Update(gameTime);
			if (GetAcceptState() == ScriptAcceptState.Accepted)
			{
				DoOneShotAccept();
				mDoneOneShot = true;
			}
			else if(GetAcceptState() == ScriptAcceptState.Declined)
			{
				DoOneShotDecline();
				mDoneOneShot = false;
			}
		}

		public override bool IsFinished()
		{
			return mDoneOneShot;
		}

		protected abstract void DoOneShotAccept();


		protected abstract void DoOneShotDecline();
	}
}
