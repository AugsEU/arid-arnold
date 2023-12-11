using System.IO.Compression;

namespace AridArnold
{
	/// <summary>
	/// Simple class for file manipulation
	/// </summary>
	abstract class MonoFile
	{
		#region rMembers

		protected string mFileName;
		protected bool mIsCompressed;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create mono file from name
		/// </summary>
		/// <param name="mFilename">File name only(not path)</param>
		public MonoFile(string fileName, bool isCompressed)
		{
			mFileName = fileName;
			mIsCompressed = isCompressed;
		}

		#endregion rInitialisation





		#region rFileSystem

		/// <summary>
		/// Get file name
		/// </summary>
		/// <returns>Relative file path</returns>
		public string GetFilename()
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
		protected virtual string GetBaseFolder()
		{
			return Directory.GetCurrentDirectory() + "\\";
		}



		/// <summary>
		/// Get folder which this is storred in
		/// </summary>
		/// <returns>Full folder directory</returns>
		protected string GetFullFolder()
		{
			return GetBaseFolder() + GetRelativeFolder();
		}



		/// <summary>
		/// Get folder and file path which this is storred in
		/// </summary>
		/// <returns>Full file path directory</returns>
		protected string GetFullFilePath()
		{
			return GetFullFolder() + GetFilename();
		}



		/// <summary>
		/// Is this a compressed file?
		/// </summary>
		public bool IsCompressed()
		{
			return mIsCompressed;
		}

		#endregion rFileSystem
	}





	/// <summary>
	/// File for reading.
	/// </summary>
	abstract class MonoReadFile : MonoFile
	{
		#region rInitialisation

		/// <summary>
		/// Create mono file from name
		/// </summary>
		public MonoReadFile(string fileName, bool compressed) : base(fileName, compressed)
		{
		}

		#endregion rInitialisation





		#region rRead

		/// <summary>
		/// Load data from file into buffers
		/// </summary>
		public void Load()
		{
			string filePath = GetFullFilePath();

			if (!File.Exists(filePath))
			{
				return;
			}

			bool delFile = false;

			if (mIsCompressed)
			{
				using (FileStream stream = File.OpenRead(filePath))
				using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
				using (BinaryReader br = new BinaryReader(decompressionStream))
				{
					try
					{
						ReadBinary(br);
					}
					catch (Exception ex)
					{
						AbortRead();
						delFile = true;
						MonoDebug.Log("Exception: " + ex.ToString());
					}
				}
			}
			else
			{
				using (FileStream stream = File.OpenRead(filePath))
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
						MonoDebug.Log("Exception: " + ex.ToString());
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

		#endregion rRead





		#region rFile

		protected override string GetRelativeFolder()
		{
			return "Content\\";
		}

		#endregion rFile
	}





	/// <summary>
	/// File for read/write.
	/// </summary>
	abstract class MonoReadWriteFile : MonoReadFile
	{
		#region rInitialisation

		/// <summary>
		/// Create mono file from name
		/// </summary>
		/// <param name="mFilename">File name only(not path)</param>
		public MonoReadWriteFile(string fileName, bool compressed) : base(fileName, compressed)
		{
		}

		#endregion rInitialisation





		#region rReadWrite

		/// <summary>
		/// Save data into the file
		/// </summary>
		public void Save()
		{
			string filePath = GetFullFilePath();

			try
			{
				string directoryPath = Path.GetDirectoryName(filePath);

				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}

				if (!File.Exists(filePath))
				{
					File.Create(filePath).Close();
				}
			}
			catch (Exception ex)
			{
				MonoDebug.Log("Exception: " + ex.ToString());
				return;
			}


			if (mIsCompressed)
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
				using (GZipStream compressionStream = new GZipStream(fileStream, CompressionLevel.Optimal))
				using (BinaryWriter bw = new BinaryWriter(compressionStream))
				{
					WriteBinary(bw);
				}
			}
			else
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
				using (BinaryWriter bw = new BinaryWriter(fileStream))
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
		#endregion rReadWrite



		#region rFileSystem

		/// <summary>
		/// Get absolute director of data folder
		/// </summary>
		/// <returns>Full folder directory</returns>
		protected override string GetBaseFolder()
		{
			return Directory.GetCurrentDirectory() + "\\data\\";
		}

		#endregion rFileSystem
	}
}
