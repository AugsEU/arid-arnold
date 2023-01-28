using System.Xml;

namespace AridArnold
{
	/// <summary>
	/// A skin for a specific tile.
	/// </summary>
	class TileTheme
	{
		(string, float)[] mTextures;

		public TileTheme(XmlNodeList textureNodeList, string id)
		{
			mTextures = new (string, float)[textureNodeList.Count];

			int idx = 0;
			foreach (XmlNode textureNode in textureNodeList)
			{
				XmlAttribute timeAttrib = textureNode.Attributes["time"];

				float time = 1.0f;

				if (textureNode.Attributes["time"] != null)
				{
					time = float.Parse(timeAttrib.Value);
				}

				mTextures[idx++] = ("Tiles/" + id + "/" + textureNode.InnerText, time);
			}
		}

		public Animator GenerateAnimator(ContentManager content)
		{
			Animator newAnimator = new Animator(content, Animator.PlayType.Repeat, mTextures);
			newAnimator.Play(RandomManager.I.GetDraw().GetUnitFloat());

			return newAnimator;
		}
	}





	/// <summary>
	/// World theming, such as a background and tile texture swaps.
	/// </summary>
	internal class WorldTheme
	{
		#region rMembers

		Color mBGColor;
		TileTheme mWallTheme;
		TileTheme mPlatformTheme;

		#endregion rMembers





		#region rInitialisation

		public WorldTheme(XmlNode themeNode, string id)
		{
			mBGColor = MonoColor.HEXToColor(themeNode.SelectSingleNode("bgColor").InnerText);

			mWallTheme = new TileTheme(themeNode.SelectNodes("wallTexture"), id);
			mPlatformTheme = new TileTheme(themeNode.SelectNodes("platformTexture"), id);
		}

		#endregion rIntialisation





		#region rAccess

		/// <summary>
		/// Generate an animation object for a wall.
		/// </summary>
		public Animator GenerateWallAnimation(ContentManager content)
		{
			return mWallTheme.GenerateAnimator(content);
		}



		/// <summary>
		/// Generate an animation object for a platform.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public Animator GeneratePlatformAnimation(ContentManager content)
		{
			return mPlatformTheme.GenerateAnimator(content);
		}



		/// <summary>
		/// Get the background color.
		/// </summary>
		public Color GetBGColor()
		{
			return mBGColor;
		}

		#endregion rAccess
	}
}
