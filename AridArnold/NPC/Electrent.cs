namespace AridArnold
{
	/// <summary>
	/// Computer NPC
	/// </summary>
	internal class Electrent : SimpleTalkNPC
	{
		#region rInitialisation

		/// <summary>
		/// Put Zippy at a position
		/// </summary>
		public Electrent(Vector2 pos) : base(pos)
		{
			mStyle.mFillColor = new Color(0, 20, 5, 240);
			mStyle.mBorderColor = new Color(56, 122, 89);
		}



		/// <summary>
		/// Load Zippy textures.
		/// </summary>
		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Electrent/Idle1", 0.3f),
												("NPC/Electrent/Idle2", 0.3f));

			Animator textScroll = new Animator(Animator.PlayType.OneShot,
												("NPC/Electrent/TextScroll1", 0.1f),
												("NPC/Electrent/TextScroll2", 0.1f),
												("NPC/Electrent/TextScroll3", 0.1f),
												("NPC/Electrent/TextScroll4", 0.1f),
												("NPC/Electrent/TextScroll5", 0.1f),
												("NPC/Electrent/TextScroll6", 0.1f),
												("NPC/Electrent/TextScroll7", 0.1f),
												("NPC/Electrent/TextScroll8", 0.1f),
												("NPC/Electrent/TextScroll9", 0.1f),
												("NPC/Electrent/TextScroll10", 0.1f),
												("NPC/Electrent/TextScroll1", 0.5f));

			mIdleAnimation = new IdleAnimator(idleAnim, 10.0f, textScroll);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Electrent/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Electrent/Angry1");
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Electrent/Idle1");

			base.LoadContent();
		}

		#endregion rInitialisation
	}
}
