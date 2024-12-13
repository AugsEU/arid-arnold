namespace AridArnold
{
	/// <summary>
	/// Manages saving/loading and stuff
	/// </summary>
	class SaveManager : Singleton<SaveManager>
	{
		#region rConstants

		const string GLOBAL_SAVE_FILE_NAME = "globalSave.bin";
		const string PROFILE_SAVE_DEFAULT_NAME = "temp.bin";

		#endregion rConstants





		#region rMembers

		GlobalSaveInfo mGlobalSaveInfo;
		
		// Current profile that's loaded.
		ProfileSaveInfo mPendingProfileSave;

		List<ProfileSaveInfo> mExistingProfiles;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Initialise the save manager
		/// </summary>
		public SaveManager()
		{
			mGlobalSaveInfo = new GlobalSaveInfo(GLOBAL_SAVE_FILE_NAME);
			mPendingProfileSave = new ProfileSaveInfo(PROFILE_SAVE_DEFAULT_NAME);

			mExistingProfiles = new List<ProfileSaveInfo>();
		}



		/// <summary>
		/// Scan the profiles folder for the profiles.
		/// </summary>
		public void ScanExistingProfiles()
		{
			mExistingProfiles.Clear();

			string baseDirectory = Path.Join("data/", ProfileSaveInfo.PROFILE_SAVE_FOLDER);
			if(!Directory.Exists(baseDirectory))
			{
				return;
			}
			string[] files = Directory.GetFiles(baseDirectory, "*.bin", SearchOption.AllDirectories);

			foreach (string fileDir in files)
			{
				string relativePath = Path.GetRelativePath(baseDirectory, fileDir);

				ProfileSaveInfo profileSaveInfo = new ProfileSaveInfo(relativePath);
				profileSaveInfo.SetReadMode(ProfileSaveInfo.ReadMode.kMetaOnly);
				profileSaveInfo.Load();
				mExistingProfiles.Add(profileSaveInfo);
			}

			mExistingProfiles.Sort();
		}



		/// <summary>
		/// Get list of save files.
		/// </summary>
		public List<ProfileSaveInfo> GetSaveFileList()
		{
			return mExistingProfiles;
		}

		#endregion rInit





		#region rGlobal

		public GlobalSaveInfo GetGlobalSaveInfo()
		{
			return mGlobalSaveInfo;
		}

		/// <summary>
		/// Save the global settings
		/// </summary>
		public void SaveGlobalSettings()
		{
			mGlobalSaveInfo.Save();
		}



		/// <summary>
		/// Load the global settings from the disc
		/// </summary>
		public void LoadGlobalSettings()
		{
			mGlobalSaveInfo.Load();
		}

		#endregion rGlobal





		#region rProfile

		/// <summary>
		/// Load a new potential profile name
		/// </summary>
		public void NewProfileName(string newProfileName)
		{
			string fileName = ProfileNameToFileName(newProfileName);
			int numberTries = 1;

			string modProf = newProfileName;

			while (IsProfileUsed(fileName))
			{
				string chopProfileName = newProfileName.Substring(0, newProfileName.Length - 1);
				modProf = string.Format("{0}{1}>", chopProfileName, numberTries);
				fileName = ProfileNameToFileName(modProf);
				numberTries++;

				MonoDebug.Assert(numberTries < 1000, "Too many profile names. Maybe try deleting some ya doofus.");
			}


			mPendingProfileSave = new ProfileSaveInfo(fileName);
			mPendingProfileSave.SetProfileName(modProf);
		}



		/// <summary>
		/// Check if a profile file name has been used or not
		/// </summary>
		private bool IsProfileUsed(string fileName)
		{
			string baseDirectory = Path.Join("data/", ProfileSaveInfo.PROFILE_SAVE_FOLDER);
			string fullPath = Path.Join(baseDirectory, fileName);

			return File.Exists(fullPath);
		}



		/// <summary>
		/// Convert profile name into file name, e.g. <Arnold> -> SArnoldS.bin
		/// </summary>
		private string ProfileNameToFileName(string profileName)
		{
			string fileName = string.Format("{0}.bin", profileName);
			return MonoData.SanitiseFileName(fileName);
		}



		/// <summary>
		/// Save the current profile.
		/// </summary>
		public void SaveProfile()
		{
			if(CampaignManager.I.IsSpeedrunMode())
			{
				// Can't save in speedruns.
				return;
			}

			mPendingProfileSave.Save();
		}


		/// <summary>
		/// Load a game from a save file.
		/// </summary>
		public void LoadGame(ProfileSaveInfo profileSaveInfo)
		{
			mPendingProfileSave = profileSaveInfo;
			mPendingProfileSave.SetReadMode(ProfileSaveInfo.ReadMode.kFull);
			Main.DefaultGameplayManagers();
			mPendingProfileSave.Load();
			ScreenManager.I.ActivateScreen(ScreenType.Game);
		}

		#endregion rProfileCreation
	}
}
