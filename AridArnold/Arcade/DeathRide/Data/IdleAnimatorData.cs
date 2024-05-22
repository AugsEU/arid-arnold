namespace GMTK2023
{
	internal class IdleAnimatorData
	{
		string mIdleAnim;
		string[] mAnimationPaths;
		float mVariationChance;

		public IdleAnimatorData(string filePath)
		{
			LoadFromFile("Content/" + filePath);
		}

		private void LoadFromFile(string filePath)
		{
			MonoDebug.Assert(Path.GetExtension(filePath) == ".mia");

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filePath);
			XmlNode rootNode = xmlDoc.LastChild;

			string varChance = rootNode.Attributes["variation"].Value;

			mVariationChance = float.Parse(varChance);

			XmlNodeList animNodes = rootNode.SelectNodes("alt");

			mAnimationPaths = new string[animNodes.Count];

			for (int i = 0; i < animNodes.Count; i++)
			{
				mAnimationPaths[i] = animNodes[i].InnerText;
			}

			mIdleAnim = rootNode.SelectSingleNode("main").InnerText;
		}

		public IdleAnimator GenerateIdleAnimator()
		{
			Animator[] anims = new Animator[mAnimationPaths.Length];
			for (int i = 0; i < mAnimationPaths.Length; i++)
			{
				anims[i] = MonoData.I.LoadAnimator(mAnimationPaths[i]);
			}

			Animator mainAnim = MonoData.I.LoadAnimator(mIdleAnim);

			return new IdleAnimator(mainAnim, mVariationChance, anims);
		}
	}
}
