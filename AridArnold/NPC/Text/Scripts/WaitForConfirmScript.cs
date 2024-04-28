
namespace AridArnold
{
	/// <summary>
	/// Script that halts text until enter is pressed.
	/// </summary>
	abstract class WaitForConfirmScript : TextScript
	{
		bool mConfirmPressed = false;

		protected WaitForConfirmScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
		}

		public override bool HaltText()
		{
			return !mConfirmPressed;
		}

		public override void Update(GameTime gameTime)
		{
			if(InputManager.I.KeyHeld(AridArnoldKeys.Confirm))
			{
				mConfirmPressed = true;
			}
		}

		public bool ConfirmPressed()
		{
			return mConfirmPressed;
		}
	}
}
