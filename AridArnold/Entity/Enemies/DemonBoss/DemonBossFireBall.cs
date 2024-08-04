
namespace AridArnold
{
	internal class DemonBossFireBall : Entity
	{
		const float LIFESPAN = 25000.0f;
		const float FIRE_BALL_SPEED = 6.0f;

		Animator mGrowAnim;
		Animator mShootingAnim;


		Entity mTarget;
		Vector2 mCurrentDir;

		PercentageTimer mLifespanTimer;

		public DemonBossFireBall(Vector2 pos) : base(pos)
		{
			mTarget = null;
			mLifespanTimer = new PercentageTimer(LIFESPAN);
			mLifespanTimer.Start();
		}

		public override void LoadContent()
		{
			mGrowAnim = new Animator(Animator.PlayType.OneShot,
				("DemonBoss/FireGrow1", 0.3f),
				("DemonBoss/FireGrow2", 0.3f),
				("DemonBoss/FireBall1", 0.3f));

			mShootingAnim = new Animator(Animator.PlayType.Repeat,
				("DemonBoss/FireBall1", 0.1f),
				("DemonBoss/FireBall2", 0.1f),
				("DemonBoss/FireBall3", 0.1f));

			mGrowAnim.Play();
			mShootingAnim.Play();

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("DemonBoss/FireBall1");
		}

		public override void Update(GameTime gameTime)
		{
			mLifespanTimer.Update(gameTime);
			mGrowAnim.Update(gameTime);
			mShootingAnim.Update(gameTime);

			if(mTarget is not null)
			{
				HomeInOnTarget(mTarget, gameTime);
			}

			if(mPosition.X < -10.0f || mPosition.X > GameScreen.GAME_AREA_WIDTH + 10.0f
				|| mPosition.Y < -10.0f || mPosition.Y > GameScreen.GAME_AREA_HEIGHT + 10.0f
				|| mLifespanTimer.GetPercentageF() >= 1.0f)
			{
				EntityManager.I.QueueDeleteEntity(this);
			}

			base.Update(gameTime);
		}

		void HomeInOnTarget(Entity target, GameTime gameTime)
		{
			Vector2 desiredDir = target.GetCentrePos() - GetCentrePos();
			if (desiredDir.LengthSquared() == 0.0f)
			{
				return;
			}
			desiredDir.Normalize();

			Vector2 averageDir = MonoMath.Lerp(desiredDir, mCurrentDir, 0.96f);
			if (averageDir.LengthSquared() == 0.0f)
			{
				averageDir = desiredDir;
			}
			else
			{
				averageDir.Normalize();
			}

			mCurrentDir = averageDir;

			float dt = Util.GetDeltaT(gameTime);
			mPosition += dt * FIRE_BALL_SPEED * mCurrentDir;
		}

		public override void OnCollideEntity(Entity entity)
		{
			if(entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				entity.Kill();
			}
			base.OnCollideEntity(entity);
		}

		public bool IsHoming()
		{
			return mTarget is not null;
		}

		public void TargetEntity(Entity entity)
		{
			mTarget = entity;
		}

		public override void Draw(DrawInfo info)
		{
			if(mGrowAnim.IsPlaying())
			{
				MonoDraw.DrawTextureDepth(info, mGrowAnim.GetCurrentTexture(), GetPos(), DrawLayer.TileEffects);
			}
			else
			{
				MonoDraw.DrawTextureDepth(info, mShootingAnim.GetCurrentTexture(), GetPos(), DrawLayer.TileEffects);
			}
		}

	}
}
