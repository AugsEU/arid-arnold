namespace AridArnold
{
	/// <summary>
	/// Class to handle talking NPCs
	/// </summary>
	abstract class NPC : PlatformingEntity
	{
		#region rConstants

		const double MOUTH_OPEN_TIME = 400.0f;
		static Vector2 DIALOG_OFFSET = new Vector2(-10.0f, 0.0f);

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
			mStyle.mFont = FontManager.I.GetFont("Pixica-12");
			mStyle.mWidth = 230.1f;
			mStyle.mLeading = 8.0f;
			mStyle.mKerning = 1.0f;
			mStyle.mSpeed = 0.6f;

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
			foreach (SpeechBoxRenderer renderer in mTextBlocks)
			{
				renderer.Update(gameTime);
			}

			UntangleTextBoxes(gameTime);

			CheckForDeletion();

			base.Update(gameTime);
		}



		/// <summary>
		/// 
		/// </summary>
		void UntangleTextBoxes(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			float untangleDisp = -mStyle.mSpeed * dt * 12.0f;
			float totalDisp = 0.0f;

			for (int i = mTextBlocks.Count - 1; i > 0; i--)
			{
				SpeechBoxRenderer lowRenderer = mTextBlocks[i];
				SpeechBoxRenderer highRenderer = mTextBlocks[i - 1];

				Rect2f lowRect = lowRenderer.GetRectBounds();
				Rect2f highRect = highRenderer.GetRectBounds();

				if (Collision2D.BoxVsBox(lowRect, highRect))
				{
					totalDisp += untangleDisp;
					highRenderer.DisplaceVertically(totalDisp);
				}
				else
				{
					totalDisp = 0.0f;
				}
			}
		}

		/// <summary>
		/// Scan our text boxes and delete them when they are above the top of the screen.
		/// </summary>
		void CheckForDeletion()
		{
			while (mTextBlocks.Count > 0 && mTextBlocks[0].GetRectBounds().max.Y < 0.0f)
			{
				mTextBlocks.RemoveAt(0);
			}
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

			if (timerTalking && IsTalking())
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

			MonoDraw.DrawTexture(info, textureToDraw, mPosition);
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

			mTextBlocks.Add(new SpeechBoxRenderer(stringID, mPosition + DIALOG_OFFSET, mStyle));
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
			if (HasAnyBoxes())
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
