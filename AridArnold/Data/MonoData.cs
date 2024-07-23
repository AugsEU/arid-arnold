using System.IO;
using System.Text.RegularExpressions;

namespace AridArnold
{
	/// <summary>
	/// Handles data mappings.
	/// </summary>
	class MonoData : Singleton<MonoData>
	{
		#region rMembers

		ContentManager mContentManager;
		Dictionary<string, string> mPathRemappings;
		Dictionary<string, AnimationData> mAnimationDataCache;
		Dictionary<string, IdleAnimatorData> mIdleAnimDataCache;

		#endregion rMembers





		#region rInit

		public void Init(ContentManager content)
		{
			mContentManager = content;
			mPathRemappings = new Dictionary<string, string>();
			mAnimationDataCache = new Dictionary<string, AnimationData>();
			mIdleAnimDataCache = new Dictionary<string, IdleAnimatorData>();
		}

		#endregion rInit





		#region rLoading

		/// <summary>
		/// Load basic monogame type.
		/// If this type has been loaded before, this won't incur a disc read.
		/// </summary>
		public T MonoGameLoad<T>(string path)
		{
			return mContentManager.Load<T>(GetRemappedPath(path));
		}



		/// <summary>
		/// Does file exist?
		/// </summary>
		public bool TextureFileExists(string path)
		{
			return File.Exists($@"Content\{GetRemappedPath(path)}.xnb");
		}



		/// <summary>
		/// Does file exist?
		/// </summary>
		public bool FileExists(string path)
		{
			return File.Exists($@"Content\{GetRemappedPath(path)}");
		}



		/// <summary>
		/// Generates a new animator from an XML file.
		/// </summary>
		public Animator LoadAnimator(string path)
		{
			AnimationData animData = LoadAnimatorData(path);

			return animData.GenerateAnimator();
		}



		/// <summary>
		/// Load animator data
		/// </summary>
		public AnimationData LoadAnimatorData(string path)
		{
			path = GetRemappedPath(path);

			AnimationData animData = null;

			if (mAnimationDataCache.TryGetValue(path, out animData))
			{
				// Loaded anim data easily.
			}
			else
			{
				animData = new AnimationData(path);

				// Add to the cache
				mAnimationDataCache.Add(path, animData);
			}

			return animData;
		}



		/// <summary>
		/// Generates a new animator from an XML file.
		/// </summary>
		public IdleAnimator LoadIdleAnimator(string path)
		{
			if(!path.EndsWith(".mia"))
			{
				path += ".mia";
			}

			path = GetRemappedPath(path);

			IdleAnimatorData animData = null;

			if (mIdleAnimDataCache.TryGetValue(path, out animData))
			{
				// Loaded anim data easily.
			}
			else
			{
				animData = new IdleAnimatorData(path);

				// Add to the cache
				mIdleAnimDataCache.Add(path, animData);
			}

			return animData.GenerateIdleAnimator();
		}



		/// <summary>
		/// Returns first string that is a real file in list
		/// </summary>
		public string FirstThatExists(List<string> paths)
		{
			foreach (string path in paths)
			{
				if (TextureFileExists(path)) return path;
			}

			return null;
		}



		/// <summary>
		/// Load first item in folder that exists
		/// </summary>
		public Texture2D LoadFirstThatExistsFolderTexture2D(string folder, List<string> pathsToTry)
		{
			foreach (string path in pathsToTry)
			{
				string fullPath = Path.Combine(folder, path);
				if (TextureFileExists(fullPath))
				{
					return MonoGameLoad<Texture2D>(fullPath);
				}
			}

			return null;
		}



		/// <summary>
		/// Load first item in folder that exists
		/// </summary>
		public IdleAnimator LoadFirstThatExistsFolderIdleAnimator(string folder, List<string> pathsToTry)
		{
			foreach (string path in pathsToTry)
			{
				string fullPath = Path.Combine(folder, path);
				if (!fullPath.EndsWith(".mia"))
				{
					fullPath += ".mia";
				}

				if (FileExists(fullPath))
				{
					return LoadIdleAnimator(fullPath);
				}
			}

			return null;
		}


		#endregion rLoading





		#region rManifest



		#endregion rManifest





		#region rPaths

		/// <summary>
		/// Get remapped path
		/// </summary>
		string GetRemappedPath(string path)
		{
			if (path.StartsWith("Content"))
			{
				path = path.Substring(8);
			}
#if DEBUG
			else if (path.Contains(":"))
			{
				throw new Exception("Trying to access path |" + path + "| is not valid. Make relative to the game.");
			}
#endif

			string newPath = path;
			while (mPathRemappings.TryGetValue(path, out newPath))
			{
				path = newPath;
			}

			return path;
		}



		/// <summary>
		/// Add remapping.
		/// Throws exception if it's already been remapped.
		/// </summary>
		public void AddPathRemap(string from, string to)
		{
			mPathRemappings.Add(from, to);
		}



		/// <summary>
		/// Remove a remapping.
		/// </summary>
		public void RemovePathRemap(string from)
		{
			mPathRemappings.Remove(from);
		}


		/// <summary>
		/// Strip a base path from a child path
		/// </summary>
		public string StripBasePath(string basePath, string childPath)
		{
			// Normalize paths to handle mixed separators
			basePath = Path.GetFullPath(basePath);
			childPath = Path.GetFullPath(childPath);

			// Make sure the fullPath starts with the commonBasePath
			if (childPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
			{
				// Extract the relative path
				string relativePath = childPath.Substring(basePath.Length).TrimStart('\\', '/');

				return relativePath;
			}

			throw new Exception("DATA ERROR: Child path not part of base path." + basePath + " | " + childPath);
		}


		public static string SanitiseFileName(string fileName)
		{
			// Remove invalid characters
			string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format(@"[{0}]+", invalidChars);

			return Regex.Replace(fileName, invalidRegStr, "S");
		}

		#endregion rPaths
	}
}
