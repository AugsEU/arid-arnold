namespace AridArnold
{
	internal class Booker : SimpleTalkNPC
	{
		#region rInitialisation

		/// <summary>
		/// Put Booker at a position
		/// </summary>
		public Booker(Vector2 pos) : base(pos)
		{
			mStyle.mFillColor = new Color(223, 224, 210, 160);
			mStyle.mBorderColor = new Color(91, 16, 0);
		}



		/// <summary>
		/// Load Booker textures.
		/// </summary>
		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.Repeat,
												("NPC/Booker/Idle1", 0.3f),
												("NPC/Booker/Idle2", 0.5f),
												("NPC/Booker/Idle1", 0.3f),
												("NPC/Booker/Idle3", 0.5f));

			mIdleAnimation = new IdleAnimator(idleAnim, 0.0f);

			//Talk textures.
			mTalkTexture = null;
			mAngryTexture = null;
			mMouthClosedTexture = null;

			base.LoadContent();
		}

		#endregion rInitialisation
	}
}
