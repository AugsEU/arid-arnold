using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	struct VoiceInfo
	{
		public AridArnoldSFX mSFX;
		public float mVolume;
		public float mPitchMin;
		public float mPitchMax;

		public VoiceInfo(XmlNode node)
		{
			mSFX = MonoParse.GetEnum<AridArnoldSFX>(node["sfx"], AridArnoldSFX.Test);
			mVolume = MonoParse.GetFloat(node["vol"], 0.6f);
			mPitchMin = MonoParse.GetFloat(node["pitchMin"], 0.0f);
			mPitchMax = MonoParse.GetFloat(node["pitchMax"], 0.0f);
		}
	}

	/// <summary>
	/// Handles playing different sounds for different voices.
	/// </summary>
	internal class Voice
	{
		Dictionary<char, VoiceInfo> mCustomLetters = new Dictionary<char, VoiceInfo>();
		VoiceInfo? mDefaultLetter;

		public Voice(string path)
		{
			path = "Content/" + path;
			string extention = Path.GetExtension(path);
			MonoDebug.Assert(extention == ".xml");

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			XmlNode rootNode = xmlDoc.LastChild;

			XmlNodeList letterNodes = rootNode.SelectNodes("letter");

			foreach (XmlNode letterNode in letterNodes)
			{
				string charString = letterNode.Attributes["char"].Value.ToLower();
				MonoDebug.Assert(charString.Length == 1);

				char charReq = charString[0];

				VoiceInfo newInfo = new VoiceInfo(letterNode);
				mCustomLetters[charReq] = newInfo;
			}

			XmlNode defaultNode = rootNode["default"];
			if (defaultNode is not null)
			{
				mDefaultLetter = new VoiceInfo(defaultNode);
			}
		}

		public void PlaySoundFromLetter(Vector2 position, char letter)
		{
			letter = char.ToLower(letter);

			VoiceInfo info;
			if(mCustomLetters.TryGetValue(letter, out info))
			{

			}
			else if(!char.IsLetter(letter))
			{
				// Do not allow unless explicitly in the custom letters.
			}
			else if(mDefaultLetter.HasValue)
			{
				info = mDefaultLetter.Value;
			}

			SpacialSFX letterSFX = new SpacialSFX(info.mSFX, position, info.mVolume, info.mPitchMin, info.mPitchMax);

			SFXManager.I.PlaySFX(letterSFX);
		}
	}
}
