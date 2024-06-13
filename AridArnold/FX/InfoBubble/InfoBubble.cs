namespace AridArnold
{
	public struct BubbleStyle
	{
		public Color mBorderColor;
		public Color mInnerColor;

		public static BubbleStyle DefaultPrompt
		{
			get
			{
				BubbleStyle style = new BubbleStyle();
				style.mInnerColor = new Color(20, 20, 20, 220);
				style.mBorderColor = new Color(150, 150, 150, 200);
				return style;
			}
		}
	}

	abstract class InfoBubble
	{
		#region rConstants

		const float DEFAULT_MOVE_SPEED = 30.1f;
		protected const float ANGULAR_SPEED = 0.7f;
		const float AMPLITUDE = 1.5f;
		const int BORDER_WIDTH = 2;

		#endregion rConstants





		#region rTypes

		enum BubbleState
		{
			Opening,
			Open,
			Closing,
			Closed
		}

		#endregion rTypes






		#region rMembers

		Vector2 mTargetSize;
		Vector2 mCurrentSize;

		BubbleState mState;

		Vector2 mBotCentre;
		Vector2 mOffset;

		BubbleStyle mStyle;

		float mWaveAngle;
		float mWaveSpeed;

		protected float mExpandSpeed = DEFAULT_MOVE_SPEED;

		#endregion rMembers





		#region rInit

		public InfoBubble(Vector2 botCentre, BubbleStyle style, float waveSpeed = ANGULAR_SPEED)
		{
			mWaveAngle = 0.0f;
			mCurrentSize = Vector2.Zero;
			mOffset = Vector2.Zero;
			mStyle = style;
			mBotCentre = botCentre;
			mState = BubbleState.Closed;
			mWaveSpeed = waveSpeed;
		}

		protected void SetTargetSize(int width, int height)
		{
			mTargetSize = new Vector2(width, height);
		}

		#endregion rInit





		#region rUpdate

		public void Update(GameTime gameTime, bool open)
		{
			float dt = Util.GetDeltaT(gameTime);

			if(open)
			{
				Open();
			}
			else
			{
				Close();
			}

			switch (mState)
			{
				case BubbleState.Opening:
					if (mCurrentSize.X < mTargetSize.X)
					{
						mCurrentSize.X += mExpandSpeed * dt;
					}
					else if (mCurrentSize.Y < mTargetSize.Y)
					{
						mCurrentSize.Y += mExpandSpeed * dt;
					}
					else
					{
						mState = BubbleState.Open;
					}

					mCurrentSize.X = MathF.Min(mCurrentSize.X, mTargetSize.X);
					mCurrentSize.Y = MathF.Min(mCurrentSize.Y, mTargetSize.Y);
					break;

				case BubbleState.Open:
					mWaveAngle += dt * mWaveSpeed;
					mOffset.Y = MathF.Sin(mWaveAngle) * AMPLITUDE;
					break;

				case BubbleState.Closing:
					if (mCurrentSize.Y > 0.0f)
					{
						mCurrentSize.Y -= mExpandSpeed * dt;
					}
					else if (mCurrentSize.X > 0.0f)
					{
						mCurrentSize.X -= mExpandSpeed * dt;
					}
					else
					{
						mState = BubbleState.Closed;
					}

					mCurrentSize.X = MathF.Max(mCurrentSize.X, 0.0f);
					mCurrentSize.Y = MathF.Max(mCurrentSize.Y, 0.0f);
					break;
			}


		}

		protected abstract void UpdateInternal();

		#endregion rUpdate


		#region rDraw

		public void Draw(DrawInfo info)
		{
			if (mState == BubbleState.Closed)
			{
				return;
			}

			Vector2 pos = mBotCentre + mOffset;
			Point innerOrigin = new Point((int)(pos.X - mCurrentSize.X / 2), (int)(pos.Y - mCurrentSize.Y));
			Rectangle innerRect = new Rectangle(innerOrigin.X, innerOrigin.Y, (int)mCurrentSize.X, (int)mCurrentSize.Y);

			MonoDraw.DrawRectDepth(info, innerRect, mStyle.mInnerColor, DrawLayer.Bubble);

			if (mState == BubbleState.Open)
			{
				DrawInner(info, innerRect);
			}

			// Draw borders
			Rectangle topRect = new Rectangle(innerOrigin.X - BORDER_WIDTH / 2, innerOrigin.Y - BORDER_WIDTH, innerRect.Width + BORDER_WIDTH, BORDER_WIDTH);
			Rectangle bottomRect = new Rectangle(innerOrigin.X - BORDER_WIDTH / 2, innerOrigin.Y + innerRect.Height, innerRect.Width + BORDER_WIDTH, BORDER_WIDTH);
			Rectangle leftRect = new Rectangle(innerOrigin.X - BORDER_WIDTH, innerOrigin.Y - BORDER_WIDTH / 2, BORDER_WIDTH, innerRect.Height + BORDER_WIDTH);
			Rectangle rightRect = new Rectangle(innerOrigin.X + innerRect.Width, innerOrigin.Y - BORDER_WIDTH / 2, BORDER_WIDTH, innerRect.Height + BORDER_WIDTH);

			MonoDraw.DrawRectDepth(info, topRect, mStyle.mBorderColor, DrawLayer.Bubble);
			MonoDraw.DrawRectDepth(info, bottomRect, mStyle.mBorderColor, DrawLayer.Bubble);
			MonoDraw.DrawRectDepth(info, leftRect, mStyle.mBorderColor, DrawLayer.Bubble);
			MonoDraw.DrawRectDepth(info, rightRect, mStyle.mBorderColor, DrawLayer.Bubble);
		}

		protected abstract void DrawInner(DrawInfo info, Rectangle area);

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Open this bubble
		/// </summary>
		private void Open()
		{
			if (mState != BubbleState.Open)
			{
				mState = BubbleState.Opening;
			}
		}


		/// <summary>
		/// Close this bubble
		/// </summary>
		private void Close()
		{
			if (mState != BubbleState.Closed)
			{
				mState = BubbleState.Closing;
			}
		}



		/// <summary>
		/// Is this finished closing?
		/// </summary>
		public bool IsClosed()
		{
			return mState == BubbleState.Closed;
		}

		#endregion rUtil
	}
}
