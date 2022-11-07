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
			mStyle.mWidth = 100.0f;
			mStyle.mLeading = 5.0f;
			mStyle.mKerning = -2.0f;
			mStyle.mSpeed = 10.0f;

			mTextBlocks = new List<SpeechBoxRenderer>();
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

		protected void DrawTalking(DrawInfo info)
		{
			SmartTextBlock.TextMood textMood = GetMood();

			Texture2D textureToDraw = mIdleTexture;
			if(mAnimTimer.GetElapsedMs() % MOUTH_OPEN_TIME < (MOUTH_OPEN_TIME / 2.0))
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
		void AddDialogBox(string stringID)
		{
			if (mTextBlocks.Count > 0)
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
			return GetCurrentBlock().GetMood();
		}
		#endregion rDialog
	}
}
