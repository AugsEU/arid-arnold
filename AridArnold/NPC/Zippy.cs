namespace AridArnold
{
	/// <summary>
	/// A racoon NPC
	/// </summary>
	internal class Zippy : SimpleTalkNPC
	{
		#region rInitialisation

		/// <summary>
		/// Put Zippy at a position
		/// </summary>
		public Zippy(Vector2 pos) : base(pos)
		{
			mStyle.mScrollSpeed = 1.1f;
			mStyle.mFramesPerLetter = 11;
		}



		/// <summary>
		/// Load Zippy textures.
		/// </summary>
		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Zippy/Idle1", 0.3f));

			Animator breatheOut = new Animator(Animator.PlayType.OneShot,
												("NPC/Zippy/Idle2", 0.8f));

			Animator sniffAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Zippy/Idle3", 0.2f),
												("NPC/Zippy/Idle1", 0.8f),
												("NPC/Zippy/Idle3", 0.2f));

			Animator tailAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Zippy/Idle4", 0.3f),
												("NPC/Zippy/Idle5", 0.4f),
												("NPC/Zippy/Idle4", 0.2f),
												("NPC/Zippy/Idle1", 0.8f),
												("NPC/Zippy/Idle4", 0.25f),
												("NPC/Zippy/Idle5", 0.3f),
												("NPC/Zippy/Idle4", 0.2f));

			mIdleAnimation = new IdleAnimator(idleAnim, 90.0f, breatheOut, tailAnim, sniffAnim);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Zippy/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Zippy/Angry1");
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Zippy/Idle1");

			base.LoadContent();
		}

		#endregion rInitialisation
	}
}
