namespace AridArnold
{
	/// <summary>
	/// Win condition status of the level
	/// </summary>
	enum LevelStatus
	{
		Continue,
		Win,
		Loss,
	}





	/// <summary>
	/// A level that can be played and has a win-condition
	/// </summary>
	abstract class Level
	{
		#region rMembers

		int mID;
		AuxData mAuxData;
		protected LevelTheme mTheme;
		string mLayoutPath;
		Layout mBGLayout;
		string mImagePath;
		bool mActive;
		int mStartTimezone;
		int mStartAge;
		protected LevelStatus mLevelStatus;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Level constructor
		/// </summary>
		public Level(AuxData data, int id)
		{
			mID = id;

			mAuxData = data;

			mImagePath = mAuxData.GetFilename();
			mImagePath = mImagePath.Substring(0, mImagePath.Length - 4);

			string themeFilePath = CampaignManager.I.GetThemePath(data.GetThemePath());
			mTheme = new LevelTheme(themeFilePath, data.GetRoot());

			mLayoutPath = "BG/" + data.GetRoot() + "/" + data.GetBGPath() + ".mlo";
			mBGLayout = new Layout(mLayoutPath); // TO DO: Get rid of this?

			// Level loaded but not playing.
			mActive = false;
		}



		/// <summary>
		/// Begin level
		/// </summary>
		public void Begin()
		{
			mLevelStatus = LevelStatus.Continue;
			mActive = true;

			// Load timezone
			mStartTimezone = TimeZoneManager.I.GetCurrentTimeZone();
			mStartAge = TimeZoneManager.I.GetCurrentPlayerAge();

			// Clear state
			EntityManager.I.ClearEntities();
			CollectableManager.I.ClearTransient();
			FXManager.I.Clear();

			// Re-Load BG
			mBGLayout = new Layout(mLayoutPath);

			// Load theme
			mTheme.Load();

			TileManager.I.LoadLevel(mImagePath);

			// Create rails
			List<LinearRailData> railList = mAuxData.GetRailsData();

			for (int i = 0; i < railList.Count; i++)
			{
				LinearRailData railData = railList[i];
				railData.ParseAllNodes();
			}

			// Create Entities
			List<EntityData> entityList = mAuxData.GetEntityData();

			for (int i = 0; i < entityList.Count; i++)
			{
				Entity newEntity = Entity.CreateEntityFromData(entityList[i]);
				EntityManager.I.RegisterEntity(newEntity);
			}

			// Reset camera
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			gameCam.Reset();

			// Inform others
			GhostManager.I.StartLevel(this);
			ItemManager.I.LevelBegin();

		}


		/// <summary>
		/// Unload all stuff
		/// </summary>
		public void End()
		{
			bool success = mLevelStatus == LevelStatus.Win;
			GhostManager.I.EndLevel(success);
			ItemManager.I.LevelEnd(success);
			mActive = false;
			mTheme.Unload();
		}


		/// <summary>
		/// Reset level
		/// </summary>
		public void Reset()
		{
			End();
			TimeZoneManager.I.SetCurrentTimeZoneAndAge(mStartTimezone, mStartAge);
			Begin();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update level.
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		/// <returns>Win condition status</returns>
		public LevelStatus Update(GameTime gameTime)
		{
			if (mActive == false)
			{
				throw new Exception("Cannot update inactive level. Did you forget to start it?");
			}

			if (EventManager.I.IsSignaled(EventType.PlayerDead))
			{
				mLevelStatus = LevelStatus.Loss;
				return LevelStatus.Loss;
			}

			mBGLayout.Update(gameTime);

			return UpdateInternal(gameTime);
		}

		/// <summary>
		/// Internal level update
		/// </summary>
		protected abstract LevelStatus UpdateInternal(GameTime gameTime);

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw level BG and other elements
		/// </summary>
		public virtual void Draw(DrawInfo info)
		{
			mBGLayout.Draw(info);
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get the image path for this level
		/// </summary>
		public string GetImagePath()
		{
			return mImagePath;
		}



		/// <summary>
		/// Active
		/// </summary>
		public bool IsActive()
		{
			return mActive;
		}



		/// <summary>
		/// Get the aux data
		/// </summary>
		public AuxData GetAuxData()
		{
			return mAuxData;
		}



		/// <summary>
		/// Get the level ID
		/// </summary>
		public int GetID()
		{
			return mID;
		}



		/// <summary>
		/// Get the theme data
		/// </summary>
		public LevelTheme GetTheme()
		{
			return mTheme;
		}

		#endregion rUtility





		#region rFactory

		/// <summary>
		/// Load a level from a an aux file path.
		/// </summary>
		public static Level LoadFromFile(string auxFilePath, int id)
		{
			// Load file
			AuxData auxData = new AuxData(auxFilePath);
			auxData.Load();

			switch (auxData.GetLevelType())
			{
				case AuxData.LevelType.CollectWater:
					return new CollectWaterLevel(auxData, id);
				case AuxData.LevelType.CollectKey:
					return new CollectKeyLevel(auxData, id);
				case AuxData.LevelType.Shop:
					return new ShopLevel(auxData, id);
				case AuxData.LevelType.Hub:
					return new HubLevel(auxData, id);
			}

			throw new NotImplementedException();
		}

		#endregion rFactory
	}


}
