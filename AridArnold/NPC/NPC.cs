namespace AridArnold
{
	/// <summary>
	/// Class to handle talking NPCs
	/// </summary>
	abstract class NPC : PlatformingEntity
	{
		#region rConstants

		const double MOUTH_OPEN_TIME = 400.0f;
		static Vector2 DIALOG_OFFSET = new Vector2(-10.0f, -4.0f);

		#endregion rConstants





		#region rMembers

		protected SpeechBoxStyle mStyle;
		List<SpeechBoxRenderer> mTextBlocks;
		MonoTimer mMouthTimer;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct NPC at position
		/// </summary>
		public NPC(Vector2 pos) : base(pos)
		{
			mTextBlocks = new List<SpeechBoxRenderer>();
			mMouthTimer = new MonoTimer();

			mStyle = new SpeechBoxStyle();
			mStyle.mFont = FontManager.I.GetFont("Pixica-12");
			mStyle.mWidth = 230.1f;
			mStyle.mLeading = 8.0f;
			mStyle.mKerning = 1.0f;
			mStyle.mScrollSpeed = 0.6f;
			mStyle.mFramesPerLetter = 20;

			mStyle.mFillColor = new Color(0, 10, 20, 200);
			mStyle.mBorderColor = new Color(56, 89, 122);
		}



		/// <summary>
		/// Load base NPC content.
		/// </summary>
		public override void LoadContent()
		{
			mTexture = GetIdleTexture();
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
			float untangleDisp = -mStyle.mScrollSpeed * dt * 12.0f;
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



		/// <summary>
		/// Default collider
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, GetNormalTalkTexture());
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

			base.Draw(info);
		}



		/// <summary>
		/// Draw the NPC talking.
		/// </summary>
		protected Texture2D GetTalkingDrawTexture()
		{
			SmartTextBlock.TextMood textMood = GetMood();

			Texture2D textureToDraw = GetIdleTexture();

			bool mouthOpen = IsTalking() && MonoText.IsVowel(GetCurrentBlock().GetCurrentChar());

			if (mouthOpen)
			{
				mMouthTimer.FullReset();
				mMouthTimer.Start();
			}

			if (mMouthTimer.IsPlaying() && mMouthTimer.GetElapsedMs() < 90.0)
			{
				switch (textMood)
				{
					default:
					case SmartTextBlock.TextMood.Normal:
						textureToDraw = GetNormalTalkTexture();
						break;
					case SmartTextBlock.TextMood.Exclaim:
						textureToDraw = GetExclaimTalkTexture(); // TO DO: Add exclamation mark.
						break;
					case SmartTextBlock.TextMood.Question:
						textureToDraw = GetQuestionTalkTexture(); // TO DO: Add question mark
						break;
				}
			}

			return textureToDraw;
		}


		/// Texture getters
		protected abstract Texture2D GetIdleTexture();
		protected abstract Texture2D GetNormalTalkTexture();
		protected abstract Texture2D GetExclaimTalkTexture();
		protected virtual Texture2D GetQuestionTalkTexture() { return GetNormalTalkTexture(); }

		#endregion rDraw





		#region rDialog

		/// <summary>
		/// Add a dialog box.(Make the NPC say something)
		/// </summary>
		protected void AddDialogBox(string stringID)
		{
			if (stringID == "")
			{
				return;
			}

			if (HasAnyBoxes())
			{
				GetCurrentBlock().Stop();
			}

			mTextBlocks.Add(new SpeechBoxRenderer(stringID, mPosition + DIALOG_OFFSET, mStyle));
		}


		/// <summary>
		/// Append a string to the current dialog box.
		/// </summary>
		protected void AppendToDialog(string stringID)
		{
			if (HasAnyBoxes())
			{
				GetCurrentBlock().PushNewString(stringID);
			}
			else
			{
				AddDialogBox(stringID);
			}
		}



		/// <summary>
		/// Get current block we are using.
		/// </summary>
		protected SpeechBoxRenderer GetCurrentBlock()
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
