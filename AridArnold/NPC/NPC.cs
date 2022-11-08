namespace AridArnold
{
	/// <summary>
	/// Class to handle talking NPCs
	/// </summary>
	abstract class NPC : PlatformingEntity
	{
		#region rConstants

		const double MOUTH_OPEN_TIME = 400.0f;

		#endregion rConstants

		#region rMembers

		SpeechBoxStyle mStyle;
		List<SpeechBoxRenderer> mTextBlocks;
		MonoTimer mAnimTimer;

		protected Texture2D mIdleTexture;
		protected Texture2D mAngryTexture;
		protected Texture2D mTalkTexture;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct NPC at position
		/// </summary>
		public NPC(Vector2 pos) : base(pos)
		{
			mStyle = new SpeechBoxStyle();
			mStyle.mFont = FontManager.I.GetFont("Pixica Micro-24");
			mStyle.mWidth = 230.1f;
			mStyle.mLeading = 5.0f;
			mStyle.mKerning = -2.0f;
			mStyle.mSpeed = 1.0f;

			mTextBlocks = new List<SpeechBoxRenderer>();
			mAnimTimer = new MonoTimer();
			mAnimTimer.Start();
		}


		/// <summary>
		/// Load base NPC content.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mStyle.mSpikeTexture = content.Load<Texture2D>("NPC/Dialog/DialogSpike");
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update NPC
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			foreach(SpeechBoxRenderer renderer in mTextBlocks)
			{
				renderer.Update(gameTime);
			}

			base.Update(gameTime);
		}

		#endregion rUpdate




		#region rDraw

		/// <summary>
		/// Draw text boxes
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			foreach (SpeechBoxRenderer renderer in mTextBlocks)
			{
				renderer.Draw(info);
			}
		}

		/// <summary>
		/// Draw the NPC talking.
		/// </summary>
		protected void DrawTalking(DrawInfo info)
		{
			SmartTextBlock.TextMood textMood = GetMood();

			Texture2D textureToDraw = mIdleTexture;

			bool timerTalking = mAnimTimer.GetElapsedMs() % MOUTH_OPEN_TIME < (MOUTH_OPEN_TIME / 2.0);

			if(timerTalking && IsTalking())
			{
				switch (textMood)
				{
					default:
					case SmartTextBlock.TextMood.Normal:
						textureToDraw = mTalkTexture;
						break;
					case SmartTextBlock.TextMood.Exclaim:
						textureToDraw = mAngryTexture;
						break;
					case SmartTextBlock.TextMood.Question:
						textureToDraw = mTalkTexture; // TO DO: Add question mark?
						break;
				}
			}

			info.spriteBatch.Draw(textureToDraw, mPosition, Color.White);
		}

		#endregion rDraw





		#region rDialog

		/// <summary>
		/// Add a dialog box.(Make the NPC say something)
		/// </summary>
		protected void AddDialogBox(string stringID)
		{
			if (HasAnyBoxes())
			{
				GetCurrentBlock().Stop();
			}

			mTextBlocks.Add(new SpeechBoxRenderer(stringID, mPosition, mStyle));
		}



		/// <summary>
		/// Get current block we are using.
		/// </summary>
		SpeechBoxRenderer GetCurrentBlock()
		{
			return mTextBlocks[mTextBlocks.Count - 1];
		}



		/// <summary>
		/// Get current mood.
		/// </summary>
		SmartTextBlock.TextMood GetMood()
		{
			if (HasAnyBoxes())
			{
				return GetCurrentBlock().GetMood();
			}

			return SmartTextBlock.TextMood.Normal;
		}



		/// <summary>
		/// Are we currently saying something?
		/// </summary>
		protected bool IsTalking()
		{
			if(HasAnyBoxes())
			{
				return !GetCurrentBlock().IsStopped();
			}

			return false;
		}



		/// <summary>
		/// Do we have any dialog boxes?
		/// </summary>
		protected bool HasAnyBoxes()
		{
			return mTextBlocks.Count > 0;
		}
		#endregion rDialog
	}
}
