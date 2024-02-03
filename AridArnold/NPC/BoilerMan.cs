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
			Animator idleAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/BoilerMan/Idle1", 0.6f));

			Animator tightenBolt = new Animator(Animator.PlayType.OneShot,
												("NPC/BoilerMan/Idle1", 0.2f),
												("NPC/BoilerMan/Idle2", 0.2f),
												("NPC/BoilerMan/Idle1", 0.2f),
												("NPC/BoilerMan/Idle2", 0.2f));

			Animator rest = new Animator(Animator.PlayType.OneShot,
						("NPC/Barbara/Idle3", 1.2f));

			mIdleAnimation = new IdleAnimator(idleAnim, 50.0f, tightenBolt);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/BoilerMan/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/BoilerMan/Exclaim1");

			base.LoadContent();
		}
	}
}
