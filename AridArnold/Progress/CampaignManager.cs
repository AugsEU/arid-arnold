#define DEBUG_LOADER

namespace AridArnold
{
	struct HubReturnInfo
	{
		public List<Entity> mPersistentEntities;
		public Level mHubRoom;
	}

	class CampaignManager : Singleton<CampaignManager>
	{
		#region rConstants

		const int START_LIVES = 3;
		public const int MAX_LIVES = 7;

		#endregion rConstants





		#region rTypes

		public enum GameplayState
		{
			HubWorld,
			LevelSequence
		}

		#endregion rTypes





		#region rMembers

		string mRootPath;
		CampaignMetaData mMetaData;

		GameplayState mGameplayState;
		Level mCurrentLevel;
		List<Level> mLevelSequence;
		int mCurrLives;

		LoadingSequence mQueuedLoad;
		HubReturnInfo? mHubReturnInfo;
		Point mPrevDoorPos;

		#endregion rMembers




		#region rInit

		/// <summary>
		/// Load a campaign from the folder.
		/// </summary>
		public void LoadCampaign(string campaignPath)
		{
			mRootPath = "Campaigns/" + campaignPath + "/";
			mMetaData = new CampaignMetaData("Content/" + mRootPath + "Meta.xml");

			mLevelSequence = new List<Level>();
			mGameplayState = GameplayState.HubWorld;

			mHubReturnInfo = null;
			mCurrLives = START_LIVES;
#if DEBUG_LOADER
			CollectableManager.I.ChangePermanentItem(0x0300, 10);
			//QueueLoadSequence(new HubDirectLoader(101));
			QueueLoadSequence(new LevelDirectLoader(212));
#else
			QueueLoadSequence(new HubDirectLoader(mMetaData.GetStartRoomID()));
#endif
			mPrevDoorPos = Point.Zero;
		}

		#endregion rInit





		#region rAccess

		/// <summary>
		/// Get current gameplay state
		/// </summary>
		/// <returns></returns>
		public GameplayState GetGameplayState()
		{
			return mGameplayState;
		}

		#endregion rAccess





		#region rPath

		/// <summary>
		/// Get hub room path from an id.
		/// </summary>
		string GetHubRoomPath(int roomId)
		{
			return mRootPath + "Hub/" + roomId.ToString().PadLeft(4, '0');
		}



		/// <summary>
		/// Get level path from id
		/// </summary>
		public string GetLevelPath(int roomId)
		{
			return mRootPath + "Levels/" + roomId.ToString().PadLeft(4, '0');
		}


		public string GetThemePath(string fileName)
		{
			return mRootPath + "/Themes/" + fileName + ".xml";
		}

		#endregion rPath





		#region rLevel

		/// <summary>
		/// Load a level from the hub world and set it as the current level.
		/// </summary>
		public Level LoadHubRoom(int roomId)
		{
			string startLevelPath = GetHubRoomPath(roomId);
			mCurrentLevel = Level.LoadFromFile(startLevelPath, roomId);
			return mCurrentLevel;
		}



		/// <summary>
		/// Load a level from the game and set it as the current level.
		/// </summary>
		public Level LoadGameLevel(int roomId)
		{
			string startLevelPath = GetLevelPath(roomId);
			mCurrentLevel = Level.LoadFromFile(startLevelPath, roomId);
			return mCurrentLevel;
		}


		/// <summary>
		/// Gets the current level
		/// </summary>
		public Level GetCurrentLevel()
		{
			return mCurrentLevel;
		}



		/// <summary>
		/// Set the current level directly
		/// </summary>
		public void SetCurrentLevel(Level level)
		{
			if(level.GetAuxData().GetLevelType() == AuxData.LevelType.Hub)
			{
				SetGameplayState(GameplayState.HubWorld);
			}
			else
			{
				SetGameplayState(GameplayState.LevelSequence);
			}
			mCurrentLevel = level;
		}



		/// <summary>
		/// Queue a transition
		/// </summary>
		public void QueueLoadSequence(LoadingSequence loadingSequence)
		{
			mQueuedLoad = loadingSequence;
		}



		/// <summary>
		/// Get hub transition that's queued.
		/// </summary>
		/// <returns>Null if no transition</returns>
		public LoadingSequence PopLoadSequence()
		{
			LoadingSequence retVal = mQueuedLoad;
			mQueuedLoad = null;
			return retVal;
		}



		/// <summary>
		/// Restart the current level
		/// </summary>
		public void RestartCurrentLevel()
		{
			mCurrentLevel.End();
			mCurrentLevel.Begin();
		}



		/// <summary>
		/// Set the gameplay state
		/// </summary>
		void SetGameplayState(GameplayState newState)
		{
			if(mGameplayState != newState)
			{
				if(newState == GameplayState.LevelSequence)
				{
					// Remember this info for later.
					HubReturnInfo retInfo = new HubReturnInfo();
					retInfo.mPersistentEntities = EntityManager.I.GetAllPersistent();
					retInfo.mHubRoom = mCurrentLevel;
					mHubReturnInfo = retInfo;

					// Set lives
					mCurrLives = START_LIVES;
				}
				else if(newState == GameplayState.HubWorld)
				{
					// Forget about it
					mHubReturnInfo = null;
				}
			}

			mGameplayState = newState;
		}

		#endregion rLevel





		#region rLevelSequence

		/// <summary>
		/// Add a level sequence to play.
		/// </summary>
		public void PushLevelSequence(List<Level> sequence, Point entryPoint)
		{
			mLevelSequence = sequence;
			mPrevDoorPos = entryPoint;
			SetGameplayState(GameplayState.LevelSequence);
			ItemManager.I.SequenceBegin();
		}



		/// <summary>
		/// Get infor for returning to the hub.
		/// </summary>
		public HubReturnInfo? GetReturnInfo()
		{
			return mHubReturnInfo;
		}



		/// <summary>
		/// Get next level in the sequence
		/// </summary>
		public Level GetNextLevelInSequence()
		{
			int currIdx = MonoAlg.GetIndex(mLevelSequence, mCurrentLevel);
			if(currIdx == mLevelSequence.Count - 1)
			{
				return null;
			}

			return mLevelSequence[currIdx + 1];
		}



		/// <summary>
		/// Called when the sequence ends
		/// </summary>
		public void EndSequence(bool success)
		{
			ItemManager.I.SequenceEnd(success);
			if (success)
			{
				// Collect "door"
				CollectableManager.I.CollectPermanentItem(mPrevDoorPos, (UInt16)PermanentCollectable.Door);
			}
		}

		#endregion rLevelSequence





		#region rLives

		/// <summary>
		/// Get how many lives we have.
		/// </summary>
		public int GetLives()
		{
			return mCurrLives;
		}



		/// <summary>
		/// Can we lose lives at the moment?
		/// </summary>
		public bool CanLoseLives()
		{
			if(mGameplayState == GameplayState.HubWorld || mLevelSequence.Count == 0)
			{
				return false;
			}

			//Can't lose lives on the first level
			return !object.ReferenceEquals(mCurrentLevel, mLevelSequence[0]);
		}



		/// <summary>
		/// Are we in a gameover?
		/// </summary>
		public bool IsGameover()
		{
			return mCurrLives <= 0;
		}



		/// <summary>
		/// Lose a life
		/// </summary>
		public void LoseLife()
		{
			if (CanLoseLives())
			{
				mCurrLives--;
			}
		}



		/// <summary>
		/// Gain a life
		/// </summary>
		public void GainLife()
		{
			if(mCurrLives < MAX_LIVES)
			{
				mCurrLives++;
			}
		}


		/// <summary>
		/// Gain multiple lives.
		/// </summary>
		/// <param name="numLives"></param>
		public void GainLives(int numLives)
		{
			mCurrLives = Math.Min(mCurrLives + numLives, MAX_LIVES);
		}

		#endregion rLives





		#region rCoins

		/// <summary>
		/// Get coin ID byte for this level
		/// </summary>
		public byte GetCurrCoinImpl()
		{
			string root = GetCurrentLevel().GetAuxData().GetRoot();
			return mMetaData.GetCoinTypeID(root);
		}


		/// <summary>
		/// Get coin ID uint16 for this level
		/// </summary>
		public UInt16 GetCurrCoinID()
		{
			return (UInt16)(((UInt16)PermanentCollectable.Coin << 8) | GetCurrCoinImpl());
		}


		#endregion rCoins
	}
}
