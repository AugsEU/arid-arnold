
namespace AridArnold
{
	/// <summary>
	/// Script that runs once after enter is pressed or is declined with shift
	/// </summary>
	abstract class WaitForConfirmDeclineScript : TextScript
	{
		public enum ScriptAcceptState
		{
			None,
			Accepted,
			Declined
		}

		ScriptAcceptState mScriptState = ScriptAcceptState.None;

		protected WaitForConfirmDeclineScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
		}

		public override bool HaltText()
		{
			return mScriptState != ScriptAcceptState.None;
		}

		public override void Update(GameTime gameTime)
		{
			if(mScriptState != ScriptAcceptState.None)
			{
				return;
			}

			if (InputManager.I.KeyHeld(AridArnoldKeys.Confirm))
			{
				mScriptState = ScriptAcceptState.Accepted;
			}
			else if(InputManager.I.KeyHeld(AridArnoldKeys.UseItem))
			{
				mScriptState = ScriptAcceptState.Declined;
			}

		}

		public ScriptAcceptState GetAcceptState()
		{
			return mScriptState;
		}
	}
}
