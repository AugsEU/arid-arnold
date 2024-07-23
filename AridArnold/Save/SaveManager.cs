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

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Initialise the save manager
		/// </summary>
		public SaveManager()
		{
			mGlobalSaveInfo = new GlobalSaveInfo(GLOBAL_SAVE_FILE_NAME);
			mPendingProfileSave = new ProfileSaveInfo(PROFILE_SAVE_DEFAULT_NAME);
		}

		#endregion rInit





		#region rGlobal

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
			string fileName = string.Format("{0}.bin", newProfileName);

			fileName = MonoData.SanitiseFileName(fileName);

			mPendingProfileSave = new ProfileSaveInfo(fileName);
			mPendingProfileSave.SetProfileName(newProfileName);
		}



		/// <summary>
		/// Save the current profile.
		/// </summary>
		public void SaveProfile()
		{
			mPendingProfileSave.Save();
		}

		#endregion rProfileCreation
	}
}
