
namespace AridArnold
{
	/// <summary>
	/// Effect of "ghost image" of an entity as it passes through a mirror.
	/// </summary>
	internal class MirrorLingerFX : FX
	{
		PercentageTimer mFadeTimer;
		Texture2D mTexture;
		Rect2f mCollider;
		Color mBaseColor;
		CardinalDirection mGravityDir;
		WalkDirection mPrevDir;
		DrawLayer mLayer;

		public MirrorLingerFX(PlatformingEntity platformingEntity, float fadeTime)
		{
			// Copy all this because we want to make sure it doesn't change as we linger.
			mTexture = platformingEntity.GetDrawTexture();
			mCollider = platformingEntity.ColliderBounds();
			mBaseColor = platformingEntity.GetDrawColor();
			mGravityDir = platformingEntity.GetGravityDir();
			mLayer = platformingEntity.GetDrawLayer();
			mPrevDir = platformingEntity.GetPrevWalkDirection();

			// Hack to provide separation between us and the mirror block...
			MoveDelta(-platformingEntity.GetVelocity() * 0.16f);

			mFadeTimer = new PercentageTimer(fadeTime);
			mFadeTimer.Start();
		}

		public void SetBaseColor(Color col)
		{
			mBaseColor = col;
		}

		public void MoveDelta(Vector2 delta)
		{
			mCollider.min += delta;
			mCollider.max += delta;
		}

		public override void Update(GameTime gameTime)
		{
			mFadeTimer.Update(gameTime);
		}

		public override bool Finished()
		{
			return mFadeTimer.GetPercentageF() >= 1.0f;
		}

		public override void Draw(DrawInfo info)
		{
			Color drawCol = mBaseColor * (1.0f - mFadeTimer.GetPercentageF());
			MonoDraw.DrawPlatformer(info, mCollider, mTexture, drawCol, mGravityDir, mPrevDir, mLayer);
		}

		
	}
}
