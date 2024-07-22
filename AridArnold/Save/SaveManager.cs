namespace AridArnold
{
	/// <summary>
	/// Manages saving/loading and stuff
	/// </summary>
	class SaveManager : Singleton<SaveManager>
	{
		const string GLOBAL_SAVE_FILE_NAME = "globalSave.bin";

		GlobalSaveInfo mGlobalSaveInfo;

		public SaveManager()
		{
			mGlobalSaveInfo = new GlobalSaveInfo(GLOBAL_SAVE_FILE_NAME);
		}

		public void SaveGlobalSettings()
		{
			mGlobalSaveInfo.Save();
		}

		public void LoadGlobalSettings()
		{
			mGlobalSaveInfo.Load();
		}
	}
}
