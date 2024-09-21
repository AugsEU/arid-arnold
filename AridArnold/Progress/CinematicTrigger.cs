namespace AridArnold
{
	/// <summary>
	/// Cinematic trigger
	/// </summary>
	class CinematicTrigger
	{
		#region rTypes

		public enum TriggerType
		{
			Opening,
			Ending,
			LevelEnterFirst,
		}

		#endregion rTypes





		#region rMembers

		TriggerType mType;
		int mLevelTrigger;
		GameCinematic mCinematic;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Cinematic trigger
		/// </summary>
		public CinematicTrigger(string campaignRoot, XmlNode cineNode)
		{
			mType = MonoParse.GetEnum<TriggerType>(cineNode.Attributes["type"]);
			mLevelTrigger = MonoParse.GetInt(cineNode["level"]);
			mCinematic = new GameCinematic(Path.Combine(campaignRoot, cineNode["cinematicPath"].InnerText));
		}

		#endregion rInit





		#region rFunc

		/// <summary>
		/// Get unique ID for this trigger
		/// </summary>
		public UInt64 GetTriggerID()
		{
			UInt64 typeHash = (UInt64)mType;
			UInt64 triggerHash = (UInt64)mLevelTrigger;

			// Weird hash
			UInt64 totalHash = typeHash ^ (triggerHash << 23 | triggerHash >> 43);

			return totalHash;
		}



		/// <summary>
		/// Does the cinematic trigger
		/// </summary>
		public bool DoesTrigger(TriggerType cineEvent)
		{
			if (cineEvent != mType)
			{
				return false;
			}

			switch (cineEvent)
			{
				case TriggerType.LevelEnterFirst:
					if (CampaignManager.I.GetCurrentLevel() is null)
					{
						return false;
					}

					return CampaignManager.I.GetCurrentLevel().GetID() == mLevelTrigger;
				case TriggerType.Ending:
					return FlagsManager.I.CheckFlag(FlagCategory.kFinishedGame);
				case TriggerType.Opening:
					return true;
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Get cinematic
		/// </summary>
		public void PlayCinematic()
		{
			if(mCinematic is null)
			{
				return;
			}
			CinematicScreen cinematicScreen = ScreenManager.I.GetScreen(ScreenType.CinematicScreen) as CinematicScreen;
			cinematicScreen.StartCinematic(mCinematic, ScreenManager.I.GetActiveScreenType());
			ScreenManager.I.ActivateScreen(ScreenType.CinematicScreen);
		}

		#endregion rFunc
	}
}
