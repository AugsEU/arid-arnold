namespace AridArnold
{
	class InputBuffer
	{
		enum BufferState
		{
			kNotPressed,
			kPendingInput,
			kActionDone
		}

		InputAction mAction;
		PercentageTimer mBufferTimer;
		BufferState mState;

		public InputBuffer(InputAction action, float bufferTime)
		{
			mAction = action;
			mState = BufferState.kNotPressed;
			mBufferTimer = new PercentageTimer(bufferTime);
		}

		public void Update(GameTime gameTime)
		{
			mBufferTimer.Update(gameTime);

			bool keyDown = InputManager.I.KeyHeld(mAction);
			switch (mState)
			{
				case BufferState.kNotPressed:
					if(keyDown)
					{
						mState = BufferState.kPendingInput;
						mBufferTimer.ResetStart();
					}
					break;
				case BufferState.kActionDone:
				case BufferState.kPendingInput:
					if(!keyDown)
					{
						mState = BufferState.kNotPressed;
						mBufferTimer.FullReset();
					}
					break;
			}
		}

		public bool QueryAction()
		{
			return mState == BufferState.kPendingInput && mBufferTimer.GetPercentageF() < 1.0f;
		}

		public void NotifyActionDone()
		{
			mState = BufferState.kActionDone;
		}
	}
}
