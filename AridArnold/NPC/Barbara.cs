namespace AridArnold
{
	internal class Barbara : NPC
	{
		#region rConstants

		const float TALK_DISTANCE = 30.0f;

		#endregion rConstants





		#region rMembers

		bool mTalking;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Put Barbara at a position
		/// </summary>
		public Barbara(Vector2 pos) : base(pos)
		{
			mTalking = false;
		}



		/// <summary>
		/// Load barbara textures.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mIdleTexture = content.Load<Texture2D>("NPC/Barbara/Idle1");
			mTalkTexture = content.Load<Texture2D>("NPC/Barbara/Talk1");
			mAngryTexture = content.Load<Texture2D>("NPC/Barbara/Angry1");

			base.LoadContent(content);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float talkDistance = TALK_DISTANCE;
			if (mTalking)
			{
				talkDistance *= 2.0f;
			}

			if (EntityManager.I.AnyNearMe(talkDistance, this, typeof(Arnold)))
			{
				if (!mTalking)
				{
					DoNormalSpeak();
				}

				mTalking = true;
			}
			else
			{
				if (mTalking && IsTalking())
				{
					HecklePlayer();
				}
				mTalking = false;
			}

			base.Update(gameTime);
		}

		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mIdleTexture);
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

		#endregion rDraw





		#region rDialog

		/// <summary>
		/// Say something.
		/// </summary>
		void DoNormalSpeak()
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
		void HecklePlayer()
		{
			AddDialogBox("NPC.Barbara.Heckle");
		}

		#endregion rDialog
	}
}
