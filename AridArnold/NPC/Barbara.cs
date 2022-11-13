namespace AridArnold
{
	internal class Barbara : SimpleTalkNPC
	{

		#region rMembers

		IdleAnimator mIdleAnimation;
		Texture2D mTalkTexture;
		Texture2D mAngryTexture;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Put Barbara at a position
		/// </summary>
		public Barbara(Vector2 pos) : base(pos)
		{
		}



		/// <summary>
		/// Load barbara textures.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			//Setup idle animation.
			Animator idleAnim = new Animator();
			idleAnim.LoadFrame(content, "NPC/Barbara/Idle1", 1.0f);

			Animator blinkAnim = new Animator();
			blinkAnim.LoadFrame(content, "NPC/Barbara/Idle2", 0.2f);
			blinkAnim.LoadFrame(content, "NPC/Barbara/Idle1", 0.7f);
			blinkAnim.LoadFrame(content, "NPC/Barbara/Idle2", 0.2f);

			Animator footAnim = new Animator();
			footAnim.LoadFrame(content, "NPC/Barbara/Idle3", 0.7f);

			Animator scratchAnim = new Animator();
			scratchAnim.LoadFrame(content, "NPC/Barbara/Idle4", 0.2f);
			scratchAnim.LoadFrame(content, "NPC/Barbara/Idle1", 0.3f);
			scratchAnim.LoadFrame(content, "NPC/Barbara/Idle4", 0.2f);

			mIdleAnimation = new IdleAnimator(idleAnim, 29.0f);
			mIdleAnimation.AddVariation(blinkAnim);
			mIdleAnimation.AddVariation(footAnim);
			mIdleAnimation.AddVariation(scratchAnim);

			//Talk textures.
			mTalkTexture = content.Load<Texture2D>("NPC/Barbara/Talk1");
			mAngryTexture = content.Load<Texture2D>("NPC/Barbara/Angry1");

			base.LoadContent(content);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update barbara animations.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mIdleAnimation.Update(gameTime);

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw Barbara
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

			if (curLevel.mWorldIndex == 0)
			{
				switch (curLevel.mLevel)
				{
					case 0:
						AddDialogBox("NPC.Barbara.Level0");
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
			AddDialogBox("NPC.Barbara.Heckle");
		}

		#endregion rDialog
	}
}
