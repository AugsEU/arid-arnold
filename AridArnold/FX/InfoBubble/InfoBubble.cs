﻿using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata.Ecma335;

namespace AridArnold
{
	abstract class InfoBubble
	{
		#region rConstants

		const float MOVE_SPEED = 0.1f;
		const float ANGULAR_SPEED = 0.1f;
		const float AMPLITUDE = 4.0f;
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

		public struct BubbleStyle
		{
			public Color mBorderColor;
			public Color mInnerColor;
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

		#endregion rMembers





		#region rInit

		public InfoBubble(Vector2 botCentre, BubbleStyle style)
		{
			mWaveAngle = 0.0f;
			mCurrentSize = Vector2.Zero;
			mOffset = Vector2.Zero;
			mStyle = style;
		}

		protected void SetTargetSize(int width, int height)
		{
			mTargetSize = new Vector2(width, height);
		}

		#endregion rInit





		#region rUpdate

		public void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			switch (mState)
			{
				case BubbleState.Opening:
					if(mCurrentSize.X < mTargetSize.X)
					{
						mCurrentSize.X += MOVE_SPEED * dt;
					}
					else if(mCurrentSize.Y < mTargetSize.Y)
					{
						mCurrentSize.Y += MOVE_SPEED * dt;
					}
					else
					{
						mState = BubbleState.Open;
					}

					mCurrentSize.X = MathF.Min(mCurrentSize.X, mTargetSize.X);
					mCurrentSize.Y = MathF.Min(mCurrentSize.Y, mTargetSize.Y);
					break;

				case BubbleState.Open:
					mWaveAngle += dt * ANGULAR_SPEED;
					mOffset.Y = MathF.Sin(mWaveAngle) * AMPLITUDE;
					break;

				case BubbleState.Closing:
					if (mCurrentSize.Y > 0.0f)
					{
						mCurrentSize.Y -= MOVE_SPEED * dt;
					}
					else if (mCurrentSize.X < mTargetSize.X)
					{
						mCurrentSize.X -= MOVE_SPEED * dt;
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
			Vector2 pos = mBotCentre + mOffset;
			Point innerOrigin = new Point((int)(pos.X - mCurrentSize.X / 2), (int)(pos.Y - mCurrentSize.Y));
			Rectangle innerRect = new Rectangle((int)innerOrigin.X, (int)innerOrigin.Y, (int)mCurrentSize.X, (int)mCurrentSize.Y);

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
		/// Close this bubble
		/// </summary>
		public void Close()
		{
			if(mState != BubbleState.Closed)
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