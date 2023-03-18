namespace AridArnold
{
	/// <summary>
	/// Simple state machine that can wait between states
	/// </summary>
	/// <typeparam name="T">State type</typeparam>
	internal class StateMachine<T>
	{
		#region rMembers

		T mState;
		bool mCanMoveState;
		MonoTimer mTimer;
		double mWaitTime;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct a state machine with a starting state
		/// </summary>
		/// <param name="_start">State to start in</param>
		public StateMachine(T _start)
		{
			mState = _start;
			mCanMoveState = true;
			mWaitTime = 0.0;
			mTimer = new MonoTimer();
		}

		#endregion





		#region rStateChanges

		/// <summary>
		/// Get the current state of the machine
		/// </summary>
		/// <returns>Current state of the machine</returns>
		public T GetState()
		{
			return mState;
		}



		/// <summary>
		/// Can we move state or are we stuck?
		/// </summary>
		/// <returns>True if we are waiting in a state</returns>
		public bool CanMoveState()
		{
			if (mTimer.GetElapsedMs() >= mWaitTime)
			{
				mCanMoveState = true;
			}

			return mCanMoveState;
		}



		/// <summary>
		/// State the state if we can.
		/// </summary>
		/// <param name="state">State we aim to go to.</param>
		/// <returns>True if we actually changed state</returns>
		public bool SetState(T state)
		{
			if (CanMoveState())
			{
				mState = state;
			}

			return mCanMoveState;
		}



		/// <summary>
		/// Set state even if we are waiting
		/// </summary>
		/// <param name="state">State we want to go to</param>
		public void ForceSetState(T state)
		{
			ForceAvailable();
			SetState(state);
		}



		/// <summary>
		/// Go to a state and hold there for a specified time
		/// </summary>
		/// <param name="newState">New state to go to</param>
		/// <param name="waitTime">Wait time in ms</param>
		public void GoToStateAndWait(T newState, double waitTime)
		{
			if (SetState(newState))
			{
				mTimer.FullReset();
				mTimer.Start();

				mCanMoveState = false;
				mWaitTime = waitTime;
			}
		}



		/// <summary>
		/// Go to a state and hold there forever.
		/// </summary>
		/// <param name="newState">New state to go to</param>
		public void GoToStateAndWaitForever(T newState)
		{
			if (SetState(newState))
			{
				mTimer.FullReset();
				mTimer.Start();

				mCanMoveState = false;

				// Not technically forever, but long enough.
				mWaitTime = double.MaxValue;
			}
		}



		/// <summary>
		/// Force go to a new state and wait there for some time.
		/// </summary>
		/// <param name="newState">New state to go to</param>
		/// <param name="waitTime">Wait time in ms</param>
		public void ForceGoToStateAndWait(T newState, double waitTime)
		{
			ForceAvailable();
			GoToStateAndWait(newState, waitTime);
		}



		/// <summary>
		/// Force ourselves to be available
		/// </summary>
		private void ForceAvailable()
		{
			mTimer.FullReset();
			mWaitTime = 0.0;
			mCanMoveState = true;
		}

		#endregion rStateChanges
	}
}
