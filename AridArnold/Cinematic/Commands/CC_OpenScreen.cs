
namespace AridArnold
{
	internal class CC_OpenScreen : CinematicCommand
	{
		ScreenType mScreenType;

		public CC_OpenScreen(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mScreenType = MonoParse.GetEnum<ScreenType>(cmdNode["screen"]);
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			mParent.FullReset();
			SFXManager.I.EndAllSFX(180.0f);
			//MusicManager.I.StopMusic();

			ScreenManager.I.ActivateScreen(mScreenType);
			base.Update(gameTime, currentFrame);
		}

		public override void Draw(DrawInfo info, int currentFrame)
		{
		}
	}
}
