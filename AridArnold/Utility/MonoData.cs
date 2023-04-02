using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace AridArnold
{
	//class AnimationData
	//{
	//	Animator.PlayType mPlayType;
	//	(string, float)[] mTextures;

	//	public AnimationData()
	//	{
	//		mPlayType = Animator.PlayType.Repeat;
	//		mTextures = null;
	//	}

	//	public AnimationData(string XMLPath)
	//	{
	//		LoadFromXML(XMLPath);
	//	}

	//	private void LoadFromXML(string XMLPath)
	//	{

	//	}

	//	public Animator GenerateAnimator()
	//	{
	//		return new Animator();
	//	}
	//}

	/// <summary>
	/// Handles data mappings.
	/// </summary>
	class MonoData : Singleton<MonoData>
	{
		#region rMembers

		ContentManager mContentManager;
		Dictionary<string, string> mPathRemappings;
		//Dictionary<string, AnimationData> mAnimationDataCache;

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
		//Animator LoadAnimator(string path)
		//{
		//	path = GetRemappedPath(path);

		//	AnimationData animData = new AnimationData();

		//	if(mAnimationDataCache.TryGetValue(path, out animData))
		//	{
		//		// Loaded anim data easily.
		//	}
		//	else
		//	{

		//	}

		//	(string, float)[] mTextures;
		//	mTextures = new (string, float)[textureNodeList.Count];

		//	int idx = 0;
		//	foreach (XmlNode textureNode in textureNodeList)
		//	{
		//		XmlAttribute timeAttrib = textureNode.Attributes["time"];

		//		float time = 1.0f;

		//		if (textureNode.Attributes["time"] != null)
		//		{
		//			time = float.Parse(timeAttrib.Value);
		//		}

		//		mTextures[idx++] = ("Tiles/" + id + "/" + textureNode.InnerText, time);
		//	}
		//}

		#endregion rLoading





		#region rManifest



		#endregion rManifest





		#region rPaths

		/// <summary>
		/// Get remapped path
		/// </summary>
		string GetRemappedPath(string path)
		{
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
