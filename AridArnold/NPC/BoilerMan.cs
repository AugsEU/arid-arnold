namespace AridArnold
{
	class BoilerMan : SimpleTalkNPC
	{
		public BoilerMan(Vector2 pos) : base(pos)
		{
			mStyle.mFillColor = new Color(38, 11, 35, 160);
			mStyle.mBorderColor = new Color(22, 0, 20);
		}

		public override void LoadContent()
		{
			//Setup idle animation.
			mIdleAnimation = MonoData.I.LoadIdleAnimator("NPC/BoilerMan/Idle.mia");

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/BoilerMan/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/BoilerMan/Exclaim1");

			base.LoadContent();
		}
	}
}
