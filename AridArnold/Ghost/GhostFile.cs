namespace AridArnold
{
	/// <summary>
	/// A file that stores ghost information
	/// </summary>
	internal class GhostFile : MonoReadWriteFile
	{
		#region rConstants

		const int MAX_FRAMES = 60 * 60 * 10;//10 minutes of recording.
		readonly char[] FILE_MAGIC = { 'G', 'H', 'T' };

		#endregion rConstants





		#region rMembers

		Level mLevel;
		List<List<GhostInfo>> mGhostInfos;

		#endregion rMembers





		#region rInitialisation

		public GhostFile(Level level) : base(level.pName + ".ght")
		{
			mGhostInfos = new List<List<GhostInfo>>(MAX_FRAMES);
			mLevel = level;
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
					bw.Write(info.position.X); bw.Write(info.position.Y);
					bw.Write(info.velocity.X); bw.Write(info.velocity.Y);
					bw.Write(info.grounded);
					bw.Write((int)info.gravity);
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

					info.position.X = br.ReadSingle(); info.position.Y = br.ReadSingle();
					info.velocity.X = br.ReadSingle(); info.velocity.Y = br.ReadSingle();
					info.grounded = br.ReadBoolean();
					info.gravity = (CardinalDirection)br.ReadInt32();

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
			mLevel = null;
		}

		#endregion rFileOperations





		#region rReadWrite

		/// <summary>
		/// Record a frame into the buffer
		/// </summary>
		/// <param name="entity">Entity to record</param>
		/// <param name="frame">Frame number</param>
		public void RecordFrame(PlatformingEntity entity, int frame)
		{
			if (frame < MAX_FRAMES)
			{
				if (mGhostInfos.Count - 1 < frame)
				{
					mGhostInfos.Add(new List<GhostInfo>());
				}

				GhostInfo info;
				info.position = entity.GetPos();
				info.velocity = entity.GetVelocity();
				info.grounded = entity.OnGround();
				info.gravity = entity.GetGravityDir();

				mGhostInfos[frame].Add(info);
			}
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
