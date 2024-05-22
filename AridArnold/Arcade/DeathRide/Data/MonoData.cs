namespace GMTK2023
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
		/// Generates a new animator from an XML file.
		/// </summary>
		public Animator LoadAnimator(string path)
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

			return animData.GenerateAnimator();
		}



		/// <summary>
		/// Generates a new animator from an XML file.
		/// </summary>
		public IdleAnimator LoadIdleAnimator(string path)
		{
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

		#endregion rPaths
	}
}
