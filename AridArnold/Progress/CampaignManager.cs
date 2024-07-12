#define DEBUG_LOADER

namespace AridArnold
{
	struct HubReturnInfo
	{
		public List<Entity> mPersistentEntities;
		public Level mHubRoom;
		public int mEnterAge;
		public int mEnterTimeZone;
	}

	class CampaignManager : Singleton<CampaignManager>
	{
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

		HashSet<UInt64> mSeenCinematics;

		// Current lives
		int mMaxLives;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Load a campaign from the folder.
		/// </summary>
		public void LoadCampaign(string campaignPath)
		{
			mRootPath = "Campaigns/" + campaignPath + "/";
			mMetaData = new CampaignMetaData("Content/" + mRootPath);

			mLevelSequence = null;
			mGameplayState = GameplayState.HubWorld;

			mSeenCinematics = new HashSet<UInt64>();

#if DEBUG_LOADER


			mMaxLives = 0;
			CollectableManager.I.IncPermanentCount(0x0300, 100);
			CollectableManager.I.IncPermanentCount(0x0000, 100);
			//CollectableManager.I.IncPermanentCount((UInt16)CollectableCategory.WaterBottle, 100);
			TimeZoneManager.I.SetCurrentTimeZoneAndAge(-1, 0);
			QueueLoadSequence(new HubDirectLoader(902));
			//QueueLoadSequence(new LevelDirectLoader(603));

			//FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kGatewayKey, true);
			FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kRippedJeans, true);
			FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kSerpentToken, true);
			FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kDemonToken, true);
			FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kHorseToken, true);
#else
			QueueLoadSequence(new HubDirectLoader(mMetaData.GetStartRoomID()));
			mMaxLives = 0;
#endif
			mHubReturnInfo = null;
			mCurrLives = GetStartLives();
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



		/// <summary>
		/// Get time override if it exists
		/// </summary>
		public TimeZoneOverride? GetTimeOverride(int fromTime, int toTime)
		{
			return mMetaData.GetTimeOverride(fromTime, toTime);
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
			CheckCinematicTriggers();
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
			if (level.GetAuxData().GetLevelType() == AuxData.LevelType.Hub)
			{
				mCurrLives = GetStartLives();
				SetGameplayState(GameplayState.HubWorld);
			}
			else
			{
				SetGameplayState(GameplayState.LevelSequence);
			}
			mCurrentLevel = level;

			// TO DO: Think about this more.
			if (level is HubLevel)
			{
				CheckCinematicTriggers();
			}
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
			mCurrentLevel.Reset();
		}



		/// <summary>
		/// Set the gameplay state
		/// </summary>
		void SetGameplayState(GameplayState newState)
		{
			if (mGameplayState != newState)
			{
				if (newState == GameplayState.LevelSequence)
				{
					// Remember this info for later.
					HubReturnInfo retInfo = new HubReturnInfo();
					retInfo.mPersistentEntities = EntityManager.I.GetAllPersistent();
					retInfo.mHubRoom = mCurrentLevel;
					retInfo.mEnterAge = TimeZoneManager.I.GetCurrentPlayerAge();
					retInfo.mEnterTimeZone = TimeZoneManager.I.GetCurrentTimeZone();
					mHubReturnInfo = retInfo;

					// Set lives
					mCurrLives = GetStartLives();
				}
				else if (newState == GameplayState.HubWorld)
				{
					// Forget about it
					mHubReturnInfo = null;
				}
			}

			mGameplayState = newState;
		}



		/// <summary>
		/// Get loaded level type
		/// </summary>
		public AuxData.LevelType GetCurrentLevelType()
		{
			if(mCurrentLevel is null)
			{
				return AuxData.LevelType.Empty;
			}

			return mCurrentLevel.GetAuxData().GetLevelType();
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

			// Inform others
			CollectableManager.I.NotifySequenceBegin();
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
			if (currIdx == mLevelSequence.Count - 1)
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
			ItemManager.I.SequenceEnd();
			if (success)
			{
				// Collect "door"
				UInt16 doorID = CollectableManager.GetCollectableID(CollectableCategory.Door);
				CollectableManager.I.CollectSpecificItem(doorID, mPrevDoorPos);
			}
			else
			{
				CollectableManager.I.NotifySequenceFail();
			}

			// Clear level sequence
			mLevelSequence = null;
		}



		/// <summary>
		/// Returns reference to level sequence list
		/// </summary>
		public List<Level> GetLevelSequence()
		{
			return mLevelSequence;
		}

		#endregion rLevelSequence





		#region rLives

		/// <summary>
		/// How many lives to start with?
		/// </summary>
		public int GetStartLives()
		{
			int baseLives = (mMaxLives + 1) / 2;

			if(FlagsManager.I.CheckFlag(FlagCategory.kCurses, (UInt32)CurseFlagTypes.kBlessingLives))
			{
				baseLives += 1;
			}
			else if (FlagsManager.I.CheckFlag(FlagCategory.kCurses, (UInt32)CurseFlagTypes.kCurseLives))
			{
				baseLives -= 2;
			}

			baseLives = Math.Max(baseLives, 1);
			baseLives = Math.Min(baseLives, mMaxLives);

			return baseLives;
		}


		/// <summary>
		/// Get how many lives we have.
		/// </summary>
		public int GetLives()
		{
			return mCurrLives;
		}



		/// <summary>
		/// Get current maximum lives
		/// </summary>
		public int GetMaxLives()
		{
			return mMaxLives;
		}



		/// <summary>
		/// Get current maximum lives
		/// </summary>
		public void GiveMaxLifeLevel()
		{
			++mMaxLives;
			mCurrLives = GetStartLives();
		}



		/// <summary>
		/// Can we lose lives at the moment?
		/// </summary>
		public bool CanLoseLives()
		{
			if(mLevelSequence is null || mCurrentLevel is null)
			{
				return false;
			}

			//Can't lose lives on the first level
			return !object.ReferenceEquals(mCurrentLevel, mLevelSequence[0]) &&
				mCurrentLevel.CanLoseLives();
		}



		/// <summary>
		/// Are we in a gameover?
		/// </summary>
		public bool IsGameover()
		{
			return mCurrLives < 0;
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
			GainLives(1);
		}


		/// <summary>
		/// Gain multiple lives.
		/// </summary>
		/// <param name="numLives"></param>
		public void GainLives(int numLives)
		{
			mCurrLives = Math.Min(mCurrLives + numLives, mMaxLives);
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
			return CollectableManager.GetCollectableID(CollectableCategory.Coin, GetCurrCoinImpl());
		}


		#endregion rCoins





		#region rCinematics

		/// <summary>
		/// Check if cinematic triggers
		/// </summary>
		void CheckCinematicTriggers()
		{
			foreach (CinematicTrigger cinematicTrigger in mMetaData.GetCinematicTriggers())
			{
				UInt64 cineID = cinematicTrigger.GetTriggerID();
				if (!mSeenCinematics.Contains(cineID) && cinematicTrigger.DoesTrigger(CinematicTrigger.TriggerType.LevelEnterFirst))
				{
					cinematicTrigger.PlayCinematic();
					mSeenCinematics.Add(cineID);
				}
			}
		}

		#endregion rCinematics
	}
}
