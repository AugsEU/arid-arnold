using static AridArnold.CampaignManager;

namespace AridArnold
{
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

		LoadingSequence mQueuedLoad;

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

			QueueLoadSequence(new HubDirectLoader(mMetaData.GetStartRoomID()));
		}

		#endregion rInit





		#region rAccess

		/// <summary>
		/// Get current gameplay state
		/// </summary>
		/// <returns></returns>
		GameplayState GetGameplayState()
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
			mCurrentLevel = Level.LoadFromFile(startLevelPath);
			return mCurrentLevel;
		}



		/// <summary>
		/// Load a level from the game and set it as the current level.
		/// </summary>
		public Level LoadGameLevel(int roomId)
		{
			string startLevelPath = GetLevelPath(roomId);
			mCurrentLevel = Level.LoadFromFile(startLevelPath);
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

		#endregion rLevel
	}
}
