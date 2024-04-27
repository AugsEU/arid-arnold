
namespace AridArnold
{
	/// <summary>
	/// Script that halts text until enter is pressed.
	/// </summary>
	abstract class WaitForInputScript : TextScript
	{
		bool mConfirmPressed = false;

		protected WaitForInputScript(SmartTextBlock parentBlock) : base(parentBlock)
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
