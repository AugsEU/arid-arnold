

using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	internal class Bouncer : NPC
	{
		#region rConstants

		const float BODY_BLOCK_RANGE = 48.0f;

		#endregion rConstants




		#region rMembers

		Texture2D mTalkTex;
		Texture2D mExclaimTex;
		Texture2D mJumpUpTex;
		Texture2D mJumpDownTex;

		#endregion rMembers




		#region rInit

		/// <summary>
		/// Create bouncer at position
		/// </summary>
		public Bouncer(Vector2 pos) : base(pos)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Bouncer/Default");
			mTalkTex = MonoData.I.MonoGameLoad<Texture2D>("NPC/Bouncer/TalkNormal");
			mExclaimTex = MonoData.I.MonoGameLoad<Texture2D>("NPC/Bouncer/TalkAngry");
			mJumpUpTex = MonoData.I.MonoGameLoad<Texture2D>("NPC/Bouncer/JumpUp");
			mJumpDownTex = MonoData.I.MonoGameLoad<Texture2D>("NPC/Bouncer/JumpDown");

			mVelocity.Y = 0.0f;
			mOnGround = true;

			mJumpSpeed = 23.0f;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update bouncer
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if(!LetArnoldThrough())
			{
				//Block player...
				Rect2f blockingRect = new Rect2f(new Vector2(mPosition.X, 0.0f), 10.0f, 5000.0f);
				EntityManager.I.AddColliderSubmission(new RectangleColliderSubmission(blockingRect));

				// Meet arnold's height.
				MeetArnoldHeight();
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Should we let arnold pass?
		/// </summary>
		bool LetArnoldThrough()
		{
			return true;
		}



		/// <summary>
		/// Match Arnold's height
		/// </summary>
		void MeetArnoldHeight()
		{
			Arnold arnold = EntityManager.I.FindArnold(); // Fine since we are in the hub...

			if(arnold is null)
			{
				return;
			}

			float xDiff = arnold.GetCentrePos().X - GetCentrePos().X;
			float yDiff = arnold.GetCentrePos().Y - GetCentrePos().Y;

			if (Math.Abs(xDiff) > BODY_BLOCK_RANGE)
			{
				// Don't do it.
				return;
			}

			if (yDiff < -16.0f && arnold.GetVelocity().Y < 0.0f && mOnGround)
			{
				Jump();
			}
		}


		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture to draw
		/// </summary>
		public override Texture2D GetDrawTexture()
		{
			float vecAlongGrav = Vector2.Dot(GravityVecNorm(), mVelocity);
			Texture2D texture = mTexture;
			if (mOnGround)
			{
				texture = GetTalkingDrawTexture();
			}
			else
			{
				if (vecAlongGrav > 0.04f)
				{
					texture = mJumpDownTex;
				}
				else
				{
					texture = mJumpUpTex;
				}
			}

			return texture;
		}


		protected override Texture2D GetExclaimTalkTexture()
		{
			return mExclaimTex;
		}

		protected override Texture2D GetIdleTexture()
		{
			return mTexture;
		}

		protected override Texture2D GetMouthClosedTexture()
		{
			return mTexture;
		}

		protected override Texture2D GetNormalTalkTexture()
		{
			return mTalkTex;
		}

		#endregion rDraw
	}
}
