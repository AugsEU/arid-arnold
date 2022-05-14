using System;
using System.Collections.Generic;
using System.Text;

namespace AridArnold
{
    internal class ProgressManager : Singleton<ProgressManager>
    {
        int mCurrentLevel = 4;
        int mLastCheckPoint = 4;
        int mLives = 4;

        public void ResetGame()
        {
            mLives = 4;
            mCurrentLevel = mLastCheckPoint;
        }

        public void ReportCheckpoint()
        {
            mLastCheckPoint = mCurrentLevel + 1;
        }
        public void ReportLevelLoss()
        {
            mLives--;
        }

        public void ReportLevelWin()
        {
            mCurrentLevel++;
        }

        public int Lives
        {
            get { return mLives; }
            set { mLives = value; }
        }
        
        public int CurrentLevel
        {
            get { return mCurrentLevel; }
        }

        public int LastCheckPoint
        { 
            get { return mLastCheckPoint; } 
        }
    }
}
