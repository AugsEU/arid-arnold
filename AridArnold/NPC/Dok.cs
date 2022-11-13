namespace AridArnold
{
	internal class Dok : SimpleTalkNPC
	{

		#region rMembers

		IdleAnimator mIdleAnimation;
		Texture2D mTalkTexture;
		Texture2D mAngryTexture;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Put Dok at a position
		/// </summary>
		public Dok(Vector2 pos) : base(pos)
		{
		}



		/// <summary>
		/// Load Dok textures.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			//Setup idle animation.
			Animator idleAnim = new Animator();
			idleAnim.LoadFrame(content, "NPC/Dok/Idle1", 0.8f);

			Animator breatheOut = new Animator();
			breatheOut.LoadFrame(content, "NPC/Dok/Idle2", 0.8f);

			Animator stickSmack = new Animator();
			stickSmack.LoadFrame(content, "NPC/Dok/Idle3", 0.3f);
			stickSmack.LoadFrame(content, "NPC/Dok/Idle1", 0.8f);
			stickSmack.LoadFrame(content, "NPC/Dok/Idle3", 0.3f);

			mIdleAnimation = new IdleAnimator(idleAnim, 90.0f);
			mIdleAnimation.AddVariation(breatheOut);
			mIdleAnimation.AddVariation(stickSmack);

			//Talk textures.
			mTalkTexture = content.Load<Texture2D>("NPC/Dok/Talk1");
			mAngryTexture = content.Load<Texture2D>("NPC/Dok/Angry1");

			base.LoadContent(content);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update Dok animations.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mIdleAnimation.Update(gameTime);

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw Dok
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			DrawTalking(info);

			base.Draw(info);
		}



		/// <summary>
		/// Get idle texture
		/// </summary>
		protected override Texture2D GetIdleTexture()
		{
			if (IsTalking())
			{
				return mIdleAnimation.GetIdleTexture();
			}

			return mIdleAnimation.GetCurrentTexture();
		}



		/// <summary>
		/// Get normal texture for talking.
		/// </summary>
		protected override Texture2D GetNormalTalkTexture()
		{
			return mTalkTexture;
		}



		/// <summary>
		/// Get exclaim texture.
		/// </summary>
		protected override Texture2D GetExclaimTalkTexture()
		{
			return mAngryTexture;
		}

		#endregion rDraw





		#region rDialog

		/// <summary>
		/// Say something.
		/// </summary>
		protected override void DoNormalSpeak()
		{
			LevelPoint curLevel = ProgressManager.I.GetLevelPoint();

			if (curLevel.mWorldIndex == 1)
			{
				switch (curLevel.mLevel)
				{
					case 0:
						AddDialogBox("NPC.Dok.Level10");
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}


		/// <summary>
		/// Shout at the player for leaving early.
		/// </summary>
		protected override void HecklePlayer()
		{
			AddDialogBox("NPC.Dok.Heckle");
		}

		#endregion rDialog
	}
}
