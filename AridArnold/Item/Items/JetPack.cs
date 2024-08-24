using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	class JetPack : Item
	{
		const float MAX_SPEED = 50.0f;
		const double JET_PACK_TIME = 700.0;

		Animator mJetAnim;
		PercentageTimer mSpeedTimer;

		public JetPack() : base("Items.JetPackTitle", "Items.JetPackDesc")
		{
			mJetAnim = new Animator(Animator.PlayType.Repeat,
									("Items/JetPack/Pack1", 0.1f),
									("Items/JetPack/Pack2", 0.1f));
			mJetAnim.Play();

			mTexture = mJetAnim.GetCurrentTexture();

			mSpeedTimer = new PercentageTimer(JET_PACK_TIME);
		}


		public override void Begin()
		{
			mSpeedTimer.ResetStart();
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

				if (t >= 1.0f)
				{
					EndItem();
				}
			}

			base.ActOnArnold(gameTime, arnold);
		}

		public override int GetPrice()
		{
			return 2;
		}

		public override void DrawOnArnold(DrawInfo info, Arnold arnold)
		{
			Rect2f arnBounds = arnold.ColliderBounds();

			Vector2 pos = arnBounds.min;
			pos.X -= 10.0f;
			SpriteEffects spriteEffects = SpriteEffects.None;

			if(arnold.GetPrevWalkDirection() == WalkDirection.Left)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;

				pos = arnBounds.min;
				pos.X += arnBounds.Width;
				pos.X -= 8.0f;
			}

			MonoDraw.DrawTexture(info, mTexture, pos, null, Color.White, 0.0f, Vector2.Zero, 1.0f, spriteEffects, DrawLayer.SubEntity);

			base.DrawOnArnold(info, arnold);
		}
	}
}
