
namespace AridArnold
{
	/// <summary>
	/// Information when reaching a world
	/// </summary>
	struct WorldReachedInfo
	{
		public string mWorldRoot;

		public WorldReachedInfo(string worldName)
		{
			mWorldRoot = worldName;
		}

		public WorldReachedInfo(BinaryReader br)
		{
			mWorldRoot = br.ReadString();
		}

		public void WriteBinary(BinaryWriter bw)
		{
			bw.Write(mWorldRoot);
		}
	}

	/// <summary>
	/// A file containing the global save data
	/// </summary>
	class GlobalSaveInfo : MonoReadWriteFile
	{
		const string GLOBAL_SAVE_MAGIC = "gas";
		const int GLOBAL_SAVE_VER = 2;

		List<WorldReachedInfo> mWorldReachedList;

		public GlobalSaveInfo(string fileName) : base(fileName, true)
		{
			mWorldReachedList = new List<WorldReachedInfo>();
		}

		public List<WorldReachedInfo> GetWorldReachedList()
		{
			return mWorldReachedList;
		}

		public void NotifyWorld(string worldRoot)
		{
			for(int i = 0; i < mWorldReachedList.Count; i++)
			{
				if (mWorldReachedList[i].mWorldRoot == worldRoot)
				{
					// Already know about this one.
					return;
				}
			}
			mWorldReachedList.Add(new WorldReachedInfo(worldRoot));
		}

		protected override void ReadBinary(BinaryReader br)
		{
			string magic = br.ReadString();
			int version = br.ReadInt32();

			MonoDebug.Assert(magic == GLOBAL_SAVE_MAGIC);

			int numWorldsReached = br.ReadInt32();
			for(int i = 0; i < numWorldsReached; i++)
			{
				mWorldReachedList.Add(new WorldReachedInfo(br));
			}

			OptionsManager.I.ReadFromBinary(br, version);
			InputManager.I.ReadFromBinary(br);
		}

		protected override void WriteBinary(BinaryWriter bw)
		{
			bw.Write(GLOBAL_SAVE_MAGIC);
			bw.Write(GLOBAL_SAVE_VER);
			
			bw.Write((int)mWorldReachedList.Count);
			for(int i = 0; i < mWorldReachedList.Count; i++)
			{
				mWorldReachedList[i].WriteBinary(bw);
			}

			OptionsManager.I.WriteFromBinary(bw);
			InputManager.I.WriteFromBinary(bw);
		}

		protected override string GetRelativeFolder()
		{
			return "globalSave/";
		}
	}
}
