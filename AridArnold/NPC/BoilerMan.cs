namespace AridArnold
{
	class BoilerMan : SimpleTalkNPC
	{
		public BoilerMan(Vector2 pos) : base(pos)
		{
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
