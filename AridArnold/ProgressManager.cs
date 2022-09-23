namespace AridArnold
{
    struct WorldData
    {
        public int startLevel;
        public string name;

        //Style
        public Color worldColor; 
        public string wallTexture;
        public string platformTexture;

        public WorldData(int _start, string _name, Color _worldColor, string _wallTexture, string _platformTexture)
        {
            startLevel = _start;
            name = _name;

            //Style
            worldColor = _worldColor;
            wallTexture = _wallTexture;
            platformTexture = _platformTexture;
        }
    }

    internal class ProgressManager : Singleton<ProgressManager>
    {
        const int START_LEVEL = 0;
        const int START_LIVES = 4;
        const int MAX_LIVES = 6;

        WorldData[] mWorldData =
        {
            new WorldData(0, "Iron Works", new Color(0, 10,20), "steel", "platform"),
            new WorldData(4, "Land of Mirrors", new Color(0, 24, 14), "cobble", "gold-platform"),
            new WorldData(8, "Buk's Cave", new Color(3, 3, 9), "cave", "cave-platform")
        };

        int mCurrentLevel = START_LEVEL;
        int mLastCheckPoint = START_LEVEL;
        int mLives = START_LIVES;

        public void Init()
        {
            mLastCheckPoint = START_LEVEL;
            ResetGame();
        }

        public void ResetGame()
        {
            mLives = START_LIVES;
            mCurrentLevel = mLastCheckPoint;
        }

        public void ReportCheckpoint()
        {
            mLastCheckPoint = mCurrentLevel + 1;

            if (mLives < START_LIVES)
            {
                mLives = START_LIVES;
            }
        }

        public void ReportLevelLoss()
        {
            //Don't lose lives on the checkpoint levels.
            if (mCurrentLevel != mLastCheckPoint)
            {
                mLives--;
            }
        }

        public void ReportLevelWin()
        {
            mCurrentLevel++;
        }

        public void GiveLife()
        {
            if(mLives < MAX_LIVES)
            {
                mLives++;
            }
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

        int mPrevLevelQuery = -1;
        int mPrevWorldIndex = -1;
        public WorldData GetWorldData()
        {
            //Memoize 
            if(mCurrentLevel == mPrevLevelQuery && mPrevWorldIndex != -1)
            {
                return mWorldData[mPrevWorldIndex];
            }

            mPrevLevelQuery = mCurrentLevel;


            for (int i = 0; i < mWorldData.Length; i++)
            {
                mPrevWorldIndex = i;

                if (mCurrentLevel < mWorldData[i].startLevel)
                {
                    mPrevWorldIndex = i - 1;
                    break;
                }
            }

            return mWorldData[mPrevWorldIndex];
        }
    }
}
