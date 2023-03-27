namespace AridArnold
{
	internal class LaserBomb : ProjectileEntity
	{
		#region rConstants

		const float LASER_BOMB_GRAVITY = 2.0f;
		const float TRACE_LEN_INCREASE = 30.0f;

		const float DEATH_RADIUS = 26.0f;
		const double DEATH_TIME_START = 120.0;
		const double DEATH_TIME_END = 380.0;

		static Color TRACE_COLOR = new Color(100, 100, 100);
		static Color TRACE_COLOR_SHADOW = new Color(30, 30, 30);

		static Vector2 TRACE_OFFSET = new Vector2(1.0f, 1.0f);

		#endregion rConstants





		#region rMembers

		Animator mBombAnim;
		float mTraceLength;
		MonoTimer mDeathTimer;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load laser bomb
		/// </summary>
		public LaserBomb(Vector2 position, Vector2 velocity) : base(position)
		{
			mVelocity = velocity;
			mTraceLength = 0.0f;
			mDeathTimer = new MonoTimer();
		}



		/// <summary>
		/// Load laser texture
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mBombAnim = new Animator(content, Animator.PlayType.Repeat, ("Enemies/Futron-Rocket/Bomb1", 0.1f)
																	  , ("Enemies/Futron-Rocket/Bomb2", 0.1f)
																	  , ("Enemies/Futron-Rocket/Bomb3", 0.1f)
																	  , ("Enemies/Futron-Rocket/Bomb4", 0.1f));
			mBombAnim.Play();

			const float EFT = 0.1f;
			mExplodingAnim = new Animator(content, Animator.PlayType.OneShot, ("Enemies/Futron-Rocket/Explosion1", EFT)
																			, ("Enemies/Futron-Rocket/Explosion2", EFT)
																			, ("Enemies/Futron-Rocket/Explosion3", EFT)
																			, ("Enemies/Futron-Rocket/Explosion4", EFT)
																			, ("Enemies/Futron-Rocket/Explosion5", EFT)
																			, ("Enemies/Futron-Rocket/Explosion6", EFT)
																			, ("Enemies/Futron-Rocket/Explosion7", EFT)
																			, ("Enemies/Futron-Rocket/Explosion8", EFT)
																			, ("Enemies/Futron-Rocket/Explosion9", EFT)
																			, ("Enemies/Futron-Rocket/Explosion10", EFT)
																			, ("Enemies/Futron-Rocket/Explosion11", EFT)
																			, ("Enemies/Futron-Rocket/Explosion12", EFT));
			mTexture = mBombAnim.GetTexture(0);
		}

		#endregion rInitialiation





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			// Free-body motion
			mPrevVelocity = mVelocity;
			mVelocity.Y += dt * LASER_BOMB_GRAVITY;

			//Draw
			mTraceLength += dt * TRACE_LEN_INCREASE;
			mBombAnim.Update(gameTime);

			//Check death
			if(mState == ProjectileState.Exploding)
			{
				mDeathTimer.Start();
				double elapsedTime = mDeathTimer.GetElapsedMs();
				if(DEATH_TIME_START < elapsedTime && elapsedTime < DEATH_TIME_END)
				{
					List<Entity> nearbyEntities = EntityManager.I.GetNearPos(DEATH_RADIUS, mExplosionCentre, typeof(Arnold), typeof(Androld));

					foreach (Entity entity in nearbyEntities)
					{
						if(entity.pPosition.Y < mExplosionCentre.Y)
						{
							EArgs args;
							args.sender = this;

							EventManager.I.SendEvent(EventType.KillPlayer, args);
							break;
						}
					}
				}
			}

			// Base
			base.Update(gameTime);
		}


		/// <summary>
		/// Collider bounds.
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mTexture);
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			switch (mState)
			{
				case ProjectileState.FreeMotion:
					MonoDraw.DrawTexture(info, mBombAnim.GetCurrentTexture(), MonoMath.Round(mPosition) - new Vector2(2.0f, 0.0f));
					break;
				case ProjectileState.Exploding:
					MonoDraw.DrawTexture(info, mExplodingAnim.GetCurrentTexture(), MonoMath.Round(mExplosionCentre) - new Vector2(28.0f, 25.0f));
					break;
				default:
					break;
			}

			DrawProjectedLine(info);
		}

		/// <summary>
		/// Draw projected line
		/// </summary>
		private void DrawProjectedLine(DrawInfo info)
		{
			if(mState != ProjectileState.FreeMotion)
			{
				return;
			}

			float traceLenRemaining = mTraceLength;
			GameTime timeStep = new GameTime(new TimeSpan(0), new TimeSpan(600000));
			FreeBodyEntity freeBodyEntity = new FreeBodyEntity(mPosition, mVelocity, LASER_BOMB_GRAVITY, mTexture.Width);

			List<EntityCollision> collisions = new List<EntityCollision>();
			TileManager.I.GatherCollisions(timeStep, freeBodyEntity, ref collisions);

			bool draw = false;
			while(collisions.Count == 0 && traceLenRemaining > 0)
			{
				Vector2 v1 = freeBodyEntity.pPosition + TRACE_OFFSET;
				freeBodyEntity.MoveTimeStep(timeStep);
				Vector2 v2 = freeBodyEntity.pPosition + TRACE_OFFSET;

				traceLenRemaining -= Vector2.Distance(v1, v2);

				if (draw)
				{
					MonoDraw.DrawLineShadow(info, v1, v2, TRACE_COLOR, TRACE_COLOR_SHADOW, 3.0f, 1.5f);
				}

				draw = !draw;
				TileManager.I.GatherCollisions(timeStep, freeBodyEntity, ref collisions);
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Enable/Disable entity
		/// </summary>
		public override void SetEnabled(bool enabled)
		{
			// When disabling, recude trace size. This is so when it re-appears the trace doesn't suddenly appear.
			if(enabled == false)
			{
				mTraceLength = 0.0f;
			}

			base.SetEnabled(enabled);
		}

		#endregion rUtility
	}
}
