using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    internal class StateMachine<T>
    {
        T mState;
        bool mCanMoveState;
        MonoTimer mTimer;
        double mWaitTime;

        public StateMachine(T _start)
        {
            mState = _start;
            mCanMoveState = true;
            mWaitTime = 0;
            mTimer = new MonoTimer();
        }

        public T GetState()
        {
            return mState;
        }

        public bool SetState(T state)
        {
            if(mTimer.GetElapsedMs() > mWaitTime)
            {
                mCanMoveState = true;
            }

            if(mCanMoveState)
            {
                mState = state;
            }

            return mCanMoveState;
        }

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
        
    }
}
