using System.IO;

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

		AuxData mAuxData;
		LevelTheme mTheme;
		string mImagePath;
		protected LevelStatus mLevelStatus;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Level constructor
		/// </summary>
		public Level(AuxData data)
		{
			mAuxData = data;

			mImagePath = System.IO.Path.GetFileNameWithoutExtension(mAuxData.GetFilename());

			mTheme = new LevelTheme(data.GetThemePath()); // To do: Ammend campaign path here?
			EventManager.I.AddListener(EventType.PlayerDead, HandlePlayerDeath);
		}



		/// <summary>
		/// Begin level
		/// </summary>
		public void Begin()
		{
			mLevelStatus = LevelStatus.Continue;

			TileManager.I.LoadLevel(mImagePath);

			// Create rails
			List<LinearRailData> railList = mAuxData.GetRailsData();

			for (int i = 0; i < railList.Count; i++)
			{
				LinearRailData railData = railList[i];
				for (int j = 0; j < railData.GetCount(); j++)
				{
					RailPlatform.TryCreateRailPlatformAtNode(railData, j);
				}
			}

			// Create Entities
			List<EntityData> entityList = mAuxData.GetEntityData();

			for (int i = 0; i < entityList.Count; i++)
			{
				Entity newEntity = Entity.CreateEntityFromData(entityList[i]);
				EntityManager.I.RegisterEntity(newEntity);
			}

			// Clear state
			EntityManager.I.ClearEntities();
			CollectableManager.I.ClearAllCollectables();
			FXManager.I.Clear();

			// Load theme
			mTheme.Load();
		}


		/// <summary>
		/// Unload all stuff
		/// </summary>
		public void End()
		{
			mTheme.Unload();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update level.
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		/// <returns>Win condition status</returns>
		public abstract LevelStatus Update(GameTime gameTime);

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Handle player's death event.
		/// </summary>
		/// <param name="args">Event sender args</param>
		public virtual void HandlePlayerDeath(EArgs args)
		{
			mLevelStatus = LevelStatus.Loss;
		}



		/// <summary>
		/// Get the image path for this level
		/// </summary>
		public string GetImagePath()
		{
			return mImagePath;
		}

		#endregion rUtility





		#region rFactory

		/// <summary>
		/// Load a level from a an aux file path.
		/// </summary>
		static Level LoadFromFile(string auxFilePath)
		{
			// Load file
			AuxData auxData = new AuxData(auxFilePath);

			switch (auxData.GetLevelType())
			{
				case AuxData.LevelType.CollectWater:
					return new CollectWaterLevel(auxData);
				case AuxData.LevelType.CollectFlag:
					return new CollectFlagLevel(auxData);
			}

			throw new NotImplementedException();
		}

		#endregion rFactory
	}


}
