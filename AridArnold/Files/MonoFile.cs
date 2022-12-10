namespace AridArnold
{
	/// <summary>
	/// Simple class for file manipulation
	/// </summary>
	abstract class MonoFile
	{
		#region rMembers

		string mFileName;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create mono file from name
		/// </summary>
		/// <param name="mFilename">File name only(not path)</param>
		public MonoFile(string fileName)
		{
			mFileName = fileName;
		}

		#endregion rInitialisation

		#region rReadWrite

		/// <summary>
		/// Save data into the file
		/// </summary>
		public void Save()
		{
			string filePath = GetFilename();

			try
			{
				if (!Directory.Exists(GetFullFolder()))
				{
					Directory.CreateDirectory(GetFullFolder());
				}

				if (!File.Exists(filePath))
				{
					File.Create(filePath).Close();
				}
			}
			catch (Exception ex)
			{
				Util.Log("Exception: " + ex.ToString());
				return;
			}


			using (FileStream stream = File.OpenWrite(filePath))
			{
				using (BinaryWriter bw = new BinaryWriter(stream))
				{
					WriteBinary(bw);
				}
			}
		}



		/// <summary>
		/// Write binary into file
		/// </summary>
		/// <param name="bw">Binary writer=</param>
		protected abstract void WriteBinary(BinaryWriter bw);



		/// <summary>
		/// Load data from file into buffers
		/// </summary>
		public void Load()
		{
			string filePath = GetFilename();

			if (!File.Exists(filePath))
			{
				return;
			}

			bool delFile = false;

			using (FileStream stream = File.OpenRead(filePath))
			{
				using (BinaryReader br = new BinaryReader(stream))
				{
					try
					{
						ReadBinary(br);
					}
					catch (Exception ex)
					{
						AbortRead();
						delFile = true;
						Util.Log("Exception: " + ex.ToString());
					}
				}
			}

			if (delFile)
			{
				File.Delete(filePath);
			}
		}



		/// <summary>
		/// Write binary into file
		/// </summary>
		/// <param name="br">Binary reader</param>
		protected abstract void ReadBinary(BinaryReader br);



		/// <summary>
		/// Called when read fails mid-way through
		/// </summary>
		protected virtual void AbortRead() { }

		#endregion rReadWrite



		#region rFileSystem

		/// <summary>
		/// Get file name
		/// </summary>
		/// <returns>Full file path</returns>
		virtual protected string GetFilename()
		{
			return mFileName;
		}


		/// <summary>
		/// Get relative folder which this is storred in
		/// </summary>
		/// <returns>Folder name</returns>
		protected abstract string GetRelativeFolder();



		/// <summary>
		/// Get absolute director of data folder
		/// </summary>
		/// <returns>Full folder directory</returns>
		private string GetBaseFolder()
		{
			return Directory.GetCurrentDirectory() + "\\data\\";
		}



		/// <summary>
		/// Get folder which this is storred in
		/// </summary>
		/// <returns>Full folder directory</returns>
		private string GetFullFolder()
		{
			return GetBaseFolder() + GetRelativeFolder();
		}



		/// <summary>
		/// Get folder and file path which this is storred in
		/// </summary>
		/// <returns>Full file path directory</returns>
		private string GetFullFilePath()
		{
			return GetFullFolder() + GetFilename();
		}

		#endregion rFileSystem
	}
}
