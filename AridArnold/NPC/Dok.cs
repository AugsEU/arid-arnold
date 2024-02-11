namespace AridArnold
{
	internal class Dok : SimpleTalkNPC
	{
		#region rInitialisation

		/// <summary>
		/// Put Dok at a position
		/// </summary>
		public Dok(Vector2 pos) : base(pos)
		{
			mStyle.mScrollSpeed = 0.55f;
			mStyle.mFramesPerLetter = 21;
		}



		/// <summary>
		/// Load Dok textures.
		/// </summary>
		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Dok/Idle1", 1.2f));

			Animator breatheOut = new Animator(Animator.PlayType.OneShot,
												("NPC/Dok/Idle2", 0.8f));

			Animator stickSmack = new Animator(Animator.PlayType.OneShot,
												("NPC/Dok/Idle3", 0.3f),
												("NPC/Dok/Idle1", 0.8f),
												("NPC/Dok/Idle3", 0.3f));

			mIdleAnimation = new IdleAnimator(idleAnim, 90.0f, breatheOut, stickSmack);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Dok/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Dok/Angry1");
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Dok/Idle1");

			base.LoadContent();
		}

		#endregion rInitialisation
	}
}
