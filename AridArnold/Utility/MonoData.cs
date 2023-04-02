using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography;
using System.Xml;

namespace AridArnold
{
	/// <summary>
	/// Utility class for loading animations from data
	/// </summary>
	class AnimationData
	{
		Animator.PlayType mPlayType;
		(string, float)[] mTextures;

		public AnimationData()
		{
			mPlayType = Animator.PlayType.Repeat;
			mTextures = null;
		}

		public AnimationData(string filePath)
		{
			LoadFromFile(filePath);
		}

		private void LoadFromFile(string filePath)
		{
			string extention = Path.GetExtension(filePath);
			switch (extention)
			{
				case "":
				{
					// Load as single texture
					mTextures = new (string, float)[] { (filePath.Substring(0, filePath.Length - 4), 1.0f) };
					mPlayType = Animator.PlayType.OneShot;
					break;
				}
				case "max": // Mono Animation XML
				{
					LoadFromXML(filePath)
					break;
				}
				default:
					throw new NotImplementedException();
			}
		}

		private void LoadFromXML(string XMLPath)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(XMLPath);
			XmlNode rootNode = xmlDoc.LastChild;

			string type = rootNode.Attributes["type"].Value.ToLower();

			mPlayType = type == "repeat" ? Animator.PlayType.Repeat : Animator.PlayType.OneShot;

			XmlNodeList textureNodes = rootNode.ChildNodes;

			mTextures = new (string, float)[textureNodes.Count];

			int idx = 0;
			foreach (XmlNode textureNode in textureNodes)
			{
				XmlAttribute timeAttrib = textureNode.Attributes["t"];
				float time = timeAttrib is not null ? float.Parse(timeAttrib.Value) : 1.0f;
				mTextures[idx++] = (textureNode.InnerText, time);
			}
		}

		public Animator GenerateAnimator()
		{
			return new Animator(mPlayType, mTextures);
		}
	}

	/// <summary>
	/// Handles data mappings.
	/// </summary>
	class MonoData : Singleton<MonoData>
	{
		#region rMembers

		ContentManager mContentManager;
		Dictionary<string, string> mPathRemappings;
		Dictionary<string, AnimationData> mAnimationDataCache;

		#endregion rMembers





		#region rInit

		public void Init(ContentManager content)
		{
			mContentManager = content;
			mPathRemappings = new Dictionary<string, string>();
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
		Animator LoadAnimator(string path)
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

		#endregion rLoading





		#region rManifest



		#endregion rManifest





		#region rPaths

		/// <summary>
		/// Get remapped path
		/// </summary>
		string GetRemappedPath(string path)
		{
			if(path.StartsWith("Content"))
			{
				path = path.Substring(8);
			}
#if DEBUG
			else if(path.Contains(":"))
			{
				throw new Exception("Trying to access path |" + path + "| is not valid. Make relative to the game.");
			}
#endif

			string newPath = path;
			while(mPathRemappings.TryGetValue(path, out newPath))
			{
				path = newPath;
			}

			return path;
		}

		#endregion rPaths
	}
}
