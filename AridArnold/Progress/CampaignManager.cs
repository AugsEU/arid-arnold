﻿namespace AridArnold
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

		bool mSpeedrunMode = false;

		string mCampaignName;
		CampaignMetaData mMetaData;

		GameplayState mGameplayState;
		Level mCurrentLevel;
		List<Level> mLevelSequence;
		bool mPlayingCompletedSequence = false;

		LoadingSequence mQueuedLoad;
		HubReturnInfo? mHubReturnInfo;
		Point mPrevDoorPos;

		HashSet<UInt64> mSeenCinematics;

		// Current lives
		int mCurrLives;
		int mMaxLives;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Begin a fresh campaign.
		/// </summary>
		public void BeginCampaign(string campaignPath)
		{
			LoadCampaign(campaignPath);
			if (BF.DEBUG_LOADER)
			{
				// Unlock max lives
				mMaxLives = 7;

				// Give resources
				CollectableManager.I.IncPermanentCount(0x0300, 100);
				CollectableManager.I.IncPermanentCount(0x0000, 100);
				CollectableManager.I.IncPermanentCount((UInt16)CollectableCategory.WaterBottle, 100);

				// Give 100 coins of every local currency
				for (int i = 0; i < 20; i++)
				{
					UInt16 coinID = CollectableManager.GetCollectableID(CollectableCategory.Coin, (byte)i);
					CollectableManager.I.IncPermanentCount(coinID, 100);
				}

				TimeZoneManager.I.SetCurrentTimeZoneAndAge(0, 0);

				QueueLoadSequence(new HubDirectLoader(701));
				//QueueLoadSequence(new LevelDirectLoader(204));

				// Unlock all items.
				FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kGatewayKey, true);
				FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kRippedJeans, true);
				FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kSerpentToken, true);
				FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kDemonToken, true);
				FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kHorseToken, true);
				FlagsManager.I.SetFlag(FlagCategory.kPanelsUnlocked, (UInt32)PanelUnlockedType.kInventory, true);
			}
			else
			{
				QueueLoadSequence(new HubDirectLoader(mMetaData.GetStartRoomID()));
				mMaxLives = 0;
			}

			mSeenCinematics = new HashSet<UInt64>();
			mCurrLives = GetStartLives();
			InputManager.I.LoadInputFrames(0);
		}



		/// <summary>
		/// Load a campaign from the folder.
		/// </summary>
		public void LoadCampaign(string campaignPath)
		{
			mCampaignName = campaignPath;
			string contentPath = Path.Join("Content/", GetRootPath());
			mMetaData = new CampaignMetaData(contentPath);

			mLevelSequence = null;
			mPlayingCompletedSequence = false;
			mGameplayState = GameplayState.HubWorld;

			mHubReturnInfo = null;
			mPrevDoorPos = Point.Zero;
		}

		#endregion rInit





		#region rAccess

		/// <summary>
		/// Get the currently loaded metadata
		/// </summary>
		public CampaignMetaData GetCampaignMetaData() 
		{ 
			return mMetaData; 
		}



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



		/// <summary>
		/// Are we in speedrun mode?
		/// </summary>
		public bool IsSpeedrunMode()
		{
			return mSpeedrunMode;
		}



		/// <summary>
		/// Turn speedrun mode on or off
		/// </summary>
		public void SetSpeedrunMode(bool isOn)
		{
			mSpeedrunMode = isOn;
		}

		#endregion rAccess





		#region rPath

		/// <summary>
		/// Get hub room path from an id.
		/// </summary>
		string GetHubRoomPath(int roomId)
		{
			string roomIdStr = roomId.ToString().PadLeft(4, '0');
			string relHubPath = string.Format("/Hub/{0}", roomIdStr);
			return Path.Join(GetRootPath(), relHubPath);
		}



		/// <summary>
		/// Get level path from id
		/// </summary>
		public string GetLevelPath(int roomId)
		{
			string roomIdStr = roomId.ToString().PadLeft(4, '0');
			string relLevelPath = string.Format("/Levels/{0}", roomIdStr);
			return Path.Join(GetRootPath(), relLevelPath);
		}



		/// <summary>
		/// Get path for a theme
		/// </summary>
		public string GetThemePath(string fileName)
		{
			string relThemePath = string.Format("/Themes/{1}.xml", GetRootPath(), fileName);
			return Path.Join(GetRootPath(), relThemePath);
		}


		/// <summary>
		/// Get a root path
		/// </summary>
		public string GetRootPath()
		{
			return string.Format("Campaigns/{0}", mCampaignName);
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
			CheckCinematicTriggers(CinematicTrigger.TriggerType.LevelEnterFirst);
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
			if(object.ReferenceEquals(mCurrentLevel, level)) return;

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

			if (mCurrentLevel.GetAuxData().GetLevelType() == AuxData.LevelType.Hub)
			{
				CheckCinematicTriggers(CinematicTrigger.TriggerType.LevelEnterFirst);
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
			mPlayingCompletedSequence = CollectableManager.I.HasSpecific(entryPoint, CollectableCategory.Door);
			mPrevDoorPos = entryPoint;

			// Inform others
			CollectableManager.I.NotifySequenceBegin();
			FlagsManager.I.NotifySequenceStart();
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

				bool coinPurse = FlagsManager.I.CheckFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kCoinPurse);
				if (coinPurse)
				{
					ushort moneyID = GetCurrCoinID();
					int moneyDiff = CollectableManager.I.GetSequenceEndDiff(moneyID);

					if (moneyDiff < 0)
					{
						CollectableManager.I.IncPermanentCount(moneyID, -moneyDiff);
					}
				}
			}
			else
			{
				CollectableManager.I.NotifySequenceFail();
			}

			// Clear level sequence
			mLevelSequence = null;
			mPlayingCompletedSequence = false;
		}



		/// <summary>
		/// Returns reference to level sequence list
		/// </summary>
		public List<Level> GetLevelSequence()
		{
			return mLevelSequence;
		}


		/// <summary>
		/// Are we playing a completed level sequence?
		/// Used to make items free.
		/// </summary>
		public bool IsPlayingCompletedSequence()
		{
			return mPlayingCompletedSequence;
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
			SFXManager.I.PlaySFX(AridArnoldSFX.OneUp, 0.4f);
			++mMaxLives;
			mCurrLives = GetStartLives();
		}


		/// <summary>
		/// Call to notify curse
		/// </summary>
		public void RefreshCurrLives()
		{
			mCurrLives = GetStartLives();
		}



		/// <summary>
		/// Can we lose lives at the moment?
		/// </summary>
		public bool CanLoseLives()
		{
			if(mLevelSequence is null || mCurrentLevel is null || mLevelSequence.Count == 0)
			{
				return false;
			}

			if(!mCurrentLevel.CanLoseLives())
			{
				return false;
			}

			//Can't lose lives on the first level(that isn't a shop)
			int i = 0;
			for( ; i < mLevelSequence.Count - 1; i++)
			{
				if(mLevelSequence[i].CanLoseLives())
				{
					break;
				}
			}

			return !object.ReferenceEquals(mCurrentLevel, mLevelSequence[i]);
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
		public void CheckCinematicTriggers(CinematicTrigger.TriggerType triggerType)
		{
			// No cinematics for you!
			// v1.1: Except the ending.
			if(IsSpeedrunMode() && triggerType != CinematicTrigger.TriggerType.Ending)
			{
				return;
			}

			foreach (CinematicTrigger cinematicTrigger in mMetaData.GetCinematicTriggers())
			{
				UInt64 cineID = cinematicTrigger.GetTriggerID();
				if (!mSeenCinematics.Contains(cineID) && cinematicTrigger.DoesTrigger(triggerType))
				{
					cinematicTrigger.PlayCinematic();
					mSeenCinematics.Add(cineID);
				}
			}
		}

		#endregion rCinematics





		#region rSerial

		/// <summary>
		/// Read from a binary file
		/// </summary>
		public void ReadBinary(BinaryReader br, int version)
		{
			// Load campaign by name
			string campaignName = br.ReadString();
			LoadCampaign(campaignName);

			// Get ready to load the hub room
			int hubRoomID = br.ReadInt32();
			LoadingSequence loadSequence = new HubDirectLoader(hubRoomID);

			// General info
			mMaxLives = br.ReadInt32();
			mCurrLives = GetStartLives();

			// Cinematic
			mSeenCinematics = new HashSet<UInt64>();
			int numCine = br.ReadInt32();
			for(int i = 0; i < numCine; i++)
			{
				UInt64 seenCineID = br.ReadUInt64();
				mSeenCinematics.Add(seenCineID);
			}

			// Arnold
			CardinalDirection gravityDir = (CardinalDirection)br.ReadInt32();
			WalkDirection prevWalkDir = (WalkDirection)br.ReadInt32();
			float posX = br.ReadSingle();
			float posY = br.ReadSingle();
			Vector2 arnoldPos = new Vector2(posX, posY);

			Arnold arnold = new Arnold(arnoldPos);
			arnold.SetGravity(gravityDir);
			arnold.SetPrevWalkDirection(prevWalkDir);
			arnold.LoadContent();

			// Force this to avoid weird jank...
			arnold.SetPos(arnoldPos);

			loadSequence.AddPersistentEntities(arnold);

			QueueLoadSequence(loadSequence);
		}



		/// <summary>
		/// Write to a binary file
		/// </summary>
		public void WriteBinary(BinaryWriter bw)
		{
			MonoDebug.Assert(GetCurrentLevelType() == AuxData.LevelType.Hub, "Cannot save game outside of hub level.");
			
			// Load data to be saved
			int currLevelId = mCurrentLevel.GetID();

			// General info
			bw.Write(mCampaignName);
			bw.Write(currLevelId);
			bw.Write(mMaxLives);

			// Cinematics
			int numCinematicsSeen = mSeenCinematics.Count;
			bw.Write(numCinematicsSeen);
			foreach(UInt64 cineID in mSeenCinematics)
			{
				bw.Write(cineID);
			}

			// Arnold...
			Arnold currArnold = EntityManager.I.FindArnold();

			CardinalDirection gravityDir = currArnold.GetGravityDir();
			WalkDirection prevWalkDir = currArnold.GetPrevWalkDirection();
			Vector2 position = currArnold.GetPos();

			bw.Write((int)gravityDir);
			bw.Write((int)prevWalkDir);
			bw.Write(position.X);
			bw.Write(position.Y);
		}

		#endregion rSerial
	}
}
