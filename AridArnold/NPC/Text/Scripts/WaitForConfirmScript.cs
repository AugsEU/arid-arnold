
namespace AridArnold
{
	/// <summary>
	/// Script that halts text until enter is pressed.
	/// </summary>
	abstract class WaitForConfirmScript : TextScript
	{
		bool mConfirmPressed = false;
		bool mFirstUpdateDone = false;

		protected WaitForConfirmScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
		}

		public override bool HaltText()
		{
			return !mConfirmPressed;
		}

		protected void ForceConfirm()
		{
			mConfirmPressed = true;
		}

		public override void Update(GameTime gameTime)
		{
			if(!mFirstUpdateDone)
			{
				DoFirstUpdate();
				mFirstUpdateDone = true;
			}

			if(InputManager.I.KeyHeld(AridArnoldKeys.Confirm))
			{
				mConfirmPressed = true;
			}
		}

		public bool ConfirmPressed()
		{
			return mConfirmPressed;
		}

		public override bool IsFinished()
		{
			return mConfirmPressed;
		}

		protected virtual void DoFirstUpdate()
		{

		}
	}
}
