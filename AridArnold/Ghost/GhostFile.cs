namespace AridArnold
{
	enum GhostSkin
	{
		kYoung,
		kOld,
		kRobot,
		kHorse
	}

	/// <summary>
	/// A file that stores ghost information
	/// </summary>
	internal class GhostFile : MonoReadWriteFile
	{
		#region rConstants

		const int MAX_FRAMES = 60 * 60 * 20; // 20 minutes of recording.
		readonly char[] FILE_MAGIC = { 'G', 'H', 'T' };

		#endregion rConstants





		#region rMembers

		List<List<GhostInfo>> mGhostInfos;

		#endregion rMembers





		#region rInitialisation

		public GhostFile(Level level) : base(level.GetImagePath() + ".ght", true)
		{
			mGhostInfos = new List<List<GhostInfo>>(MAX_FRAMES);
		}

		public GhostFile(string relativePath) : base(relativePath, true)
		{
			mGhostInfos = new List<List<GhostInfo>>(MAX_FRAMES);
		}

		#endregion rInitialisation





		#region rFileOperations

		/// <summary>
		/// Write binary into file
		/// </summary>
		/// <param name="bw">Binary writer=</param>
		protected override void WriteBinary(BinaryWriter bw)
		{
			//Write header.
			bw.Write(FILE_MAGIC);
			bw.Write(mGhostInfos.Count);

			//Write data.
			for (int i = 0; i < mGhostInfos.Count; i++)
			{
				bw.Write(mGhostInfos[i].Count);
				foreach (GhostInfo info in mGhostInfos[i])
				{
					bw.Write(info.mEnabled);
					bw.Write(info.mPosition.X); bw.Write(info.mPosition.Y);
					bw.Write(info.mVelocity.X); bw.Write(info.mVelocity.Y);
					bw.Write(info.mGrounded);
					bw.Write((int)info.mGravity);
					bw.Write((int)info.mWalkDirection);
					bw.Write((int)info.mPrevWalkDirection);
					bw.Write((int)info.mSkin);
				}
			}
		}



		/// <summary>
		/// Write binary into file
		/// </summary>
		/// <param name="br">Binary reader</param>
		protected override void ReadBinary(BinaryReader br)
		{
			//Read header.
			br.ReadChars(FILE_MAGIC.Length);
			int count = br.ReadInt32();

			//Read data.
			for (int i = 0; i < count; i++)
			{
				int ghostNumber = br.ReadInt32();

				mGhostInfos.Add(new List<GhostInfo>());

				for (int j = 0; j < ghostNumber; j++)
				{
					GhostInfo info;
					info.mEnabled = br.ReadBoolean();
					info.mPosition.X = br.ReadSingle(); info.mPosition.Y = br.ReadSingle();
					info.mVelocity.X = br.ReadSingle(); info.mVelocity.Y = br.ReadSingle();
					info.mGrounded = br.ReadBoolean();
					info.mGravity = (CardinalDirection)br.ReadInt32();
					info.mWalkDirection = (WalkDirection)br.ReadInt32();
					info.mPrevWalkDirection = (WalkDirection)br.ReadInt32();
					info.mSkin = (GhostSkin)br.ReadInt32();
					mGhostInfos[i].Add(info);
				}
			}
		}



		/// <summary>
		/// Called when read fails mid-way through
		/// </summary>
		protected override void AbortRead()
		{
			mGhostInfos.Clear();
		}



		/// <summary>
		/// Close this file.
		/// </summary>
		public void Close()
		{
			mGhostInfos.Clear();
		}

		#endregion rFileOperations





		#region rReadWrite

		/// <summary>
		/// Record a frame into the buffer
		/// </summary>
		/// <param name="arnold">Entity to record</param>
		/// <param name="frame">Frame number</param>
		public bool RecordFrame(Arnold arnold, int frame)
		{
			if (frame >= MAX_FRAMES)
			{
				return false;
			}

			if (mGhostInfos.Count - 1 < frame)
			{
				mGhostInfos.Add(new List<GhostInfo>());
			}

			GhostInfo info;
			info.mEnabled = arnold.IsEnabled();
			info.mPosition = arnold.GetPos();
			info.mVelocity = arnold.GetVelocity();
			info.mGrounded = arnold.OnGround();
			info.mGravity = arnold.GetGravityDir();
			info.mWalkDirection = arnold.GetWalkDirection();
			info.mPrevWalkDirection = arnold.GetPrevWalkDirection();
			info.mSkin = arnold.GetGhostSkin();

			mGhostInfos[frame].Add(info);
			return true;
		}



		/// <summary>
		/// Read a frame of information.
		/// </summary>
		/// <param name="frame">Frame number to read</param>
		/// <returns>List of ghosts that were recorded on that frame</returns>
		public List<GhostInfo> ReadFrame(int frame)
		{
			frame = Math.Min(frame, mGhostInfos.Count - 1);
			return mGhostInfos[frame];
		}

		#endregion rReadWrite





		#region rUtility

		/// <summary>
		/// Is this file empty?
		/// </summary>
		/// <returns>True if file is empty</returns>
		public bool IsEmpty()
		{
			return mGhostInfos.Count == 0;
		}



		/// <summary>
		/// Get number of recorded frames
		/// </summary>
		/// <returns>Number of frames in the buffer</returns>
		public int GetFrameCount()
		{
			return mGhostInfos.Count;
		}



		/// <summary>
		/// Get relative folder which this is storred in
		/// </summary>
		/// <returns>Folder name</returns>
		protected override string GetRelativeFolder()
		{
			return "ghostData\\";
		}

		#endregion rUtility
	}
}
