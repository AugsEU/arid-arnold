
namespace AridArnold
{
	internal class CC_PlayMusic : CinematicCommand
	{
		string mMusicID;

		public CC_PlayMusic(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mMusicID = MonoParse.GetString(cmdNode["id"], "");
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			MusicManager.I.RequestTrackPlay(mMusicID);
			base.Update(gameTime, currentFrame);
		}
	}
}
