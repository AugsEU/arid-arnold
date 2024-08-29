using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	class JetPack : Item
	{
		const float MAX_SPEED = 50.0f;
		const double JET_PACK_TIME = 700.0;

		Animator mJetAnim;
		PercentageTimer mSpeedTimer;
		GameSFX mTravelSFX;

		public JetPack(int price) : base("Items.JetPackTitle", "Items.JetPackDesc", price)
		{
			mJetAnim = new Animator(Animator.PlayType.Repeat,
									("Items/JetPack/Pack1", 0.1f),
									("Items/JetPack/Pack2", 0.1f));
			mJetAnim.Play();

			mTexture = mJetAnim.GetCurrentTexture();

			mSpeedTimer = new PercentageTimer(JET_PACK_TIME);
			mTravelSFX = null;
		}


		public override void Begin()
		{
			mSpeedTimer.ResetStart();
			mTravelSFX = new GameSFX(AridArnoldSFX.FireTravel, 0.6f, 0.6f);
			SFXManager.I.PlaySFX(mTravelSFX, 100.0f);
			base.Begin();
		}

		public override void Update(GameTime gameTime)
		{
			mSpeedTimer.Update(gameTime);
			mJetAnim.Update(gameTime);
			mTexture = mJetAnim.GetCurrentTexture();
			base.Update(gameTime);
		}

		public override void ActOnArnold(GameTime gameTime, Arnold arnold)
		{
			if(mSpeedTimer.IsPlaying())
			{
				float t = mSpeedTimer.GetPercentageF();
				float speedT = MathHelper.Lerp(0.0f, MAX_SPEED, t);

				Vector2 speedVec = new Vector2(0.0f, -speedT);

				arnold.ResetAllJumpHelpers();
				arnold.OverrideVelocity(speedVec);
				arnold.SetWalkDirection(WalkDirection.None);

				Vector2 smokePos = GetPos(arnold);
				smokePos.X += mTexture.Width * 0.5f;
				smokePos.Y += mTexture.Height;

				DustUtil.EmitDust(smokePos, new Vector2(0.0f, 1.0f));

				if (t >= 1.0f)
				{
					mTravelSFX.Stop(20.0f);
					EndItem();
				}
			}

			base.ActOnArnold(gameTime, arnold);
		}

		public override void DrawOnArnold(DrawInfo info, Arnold arnold)
		{
			Vector2 pos = GetPos(arnold);
			SpriteEffects spriteEffects = SpriteEffects.None;

			if(arnold.GetPrevWalkDirection() == WalkDirection.Left)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}

			MonoDraw.DrawTexture(info, mTexture, pos, null, Color.White, 0.0f, Vector2.Zero, 1.0f, spriteEffects, DrawLayer.SubEntity);

			base.DrawOnArnold(info, arnold);
		}

		private Vector2 GetPos(Arnold arnold)
		{
			Rect2f arnBounds = arnold.ColliderBounds();
			Vector2 pos = arnBounds.min;
			pos.X -= 10.0f;

			if (arnold.GetPrevWalkDirection() == WalkDirection.Left)
			{
				pos = arnBounds.min;
				pos.X += arnBounds.Width;
				pos.X -= 8.0f;
			}

			return pos;
		}
	}
}
