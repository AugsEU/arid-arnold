namespace AridArnold
{
	/// <summary>
	/// Class to handle talking NPCs
	/// </summary>
	abstract class NPC : PlatformingEntity
	{
		#region rConstants

		public const double MOUTH_OPEN_TIME = 70.0;
		static Vector2 DIALOG_OFFSET = new Vector2(-13.0f, -7.0f);

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

			mStyle = SpeechBoxStyle.DefaultStyle;
			mPosition.X += 4.0f;
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
			mMouthTimer.Update(gameTime);

			foreach (SpeechBoxRenderer renderer in mTextBlocks)
			{
				renderer.Update(gameTime);
			}

			UntangleTextBoxes(gameTime);

			CheckForDeletion();

			if(EventManager.I.IsSignaled(EventType.TimeChanged))
			{
				mTextBlocks.Clear();
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Untangle Text Boxes
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
			return new Rect2f(mPosition, 8.0f, 16.0f);
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

			Texture2D idleTexture = GetIdleTexture();
			Texture2D textureToDraw = idleTexture;

			bool isTalking = IsTalking();
			bool mouthOpen = isTalking && MonoText.IsVowel(GetCurrentBlock().GetCurrentChar());

			if (mouthOpen)
			{
				mMouthTimer.FullReset();
				mMouthTimer.Start();
			}

			if (mMouthTimer.IsPlaying() && mMouthTimer.GetElapsedMs() < MOUTH_OPEN_TIME)
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
			else if (isTalking)
			{
				textureToDraw = GetMouthClosedTexture();
			}

			if (textureToDraw == null)
			{
				textureToDraw = idleTexture;
			}

			return textureToDraw;
		}


		/// Texture getters
		protected abstract Texture2D GetIdleTexture();
		protected abstract Texture2D GetNormalTalkTexture();
		protected abstract Texture2D GetExclaimTalkTexture();
		protected virtual Texture2D GetQuestionTalkTexture() { return GetNormalTalkTexture(); }
		protected abstract Texture2D GetMouthClosedTexture();

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

			Vector2 dialogPosition = mPosition + DIALOG_OFFSET;
			float maxX = FXManager.I.GetDrawableSize().X - mStyle.mWidth - 20.0f;
			dialogPosition.X = MathF.Min(dialogPosition.X, maxX);
			dialogPosition.Y -= ColliderBounds().Height * 0.5f;

			float spikeOffset = mPosition.X - dialogPosition.X + 17.0f;

			SpeechBoxStyle style = mStyle;

			bool grillVogelHack = this is GrillVogel;
			bool fastText = OptionsManager.I.GetFastText() || CampaignManager.I.IsSpeedrunMode();
			if (fastText && style.mFramesPerLetter > 2 && !grillVogelHack)
			{
				style.mFramesPerLetter -= 1;
			}

			mTextBlocks.Add(new SpeechBoxRenderer(stringID, dialogPosition, spikeOffset, style));
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
				return !GetCurrentBlock().IsTextFinished();
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
