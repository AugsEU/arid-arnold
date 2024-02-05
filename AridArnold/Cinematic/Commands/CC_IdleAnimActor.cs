namespace AridArnold
{
	internal class CC_IdleAnimActor : CC_ActorCommand
	{
		IdleAnimator mAnimation;

		public CC_IdleAnimActor(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mAnimation = MonoData.I.LoadIdleAnimator(cmdNode["anim"].InnerText);
		}


		public override void Update(GameTime gameTime, int currentFrame)
		{
			mAnimation.Update(gameTime);
			mTargetActor.SetDrawTexture(mAnimation.GetCurrentTexture());
		}
	}
}
