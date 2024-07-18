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

		const float TRACE_SEGMENT_LENGTH = 6.0f;

		#endregion rConstants





		#region rMembers

		Animator mBombAnim;
		float mTraceLength;
		MonoTimer mDeathTimer;
		Vector2 mPrevVelDirection;

		Vector2 mPrevPosition;
		float mDistanceTravelled;

		Texture2D mReticuleTexture;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load laser bomb
		/// </summary>
		public LaserBomb(Entity parent, Vector2 position, Vector2 velocity) : base(parent, position, LASER_BOMB_GRAVITY)
		{
			mVelocity = velocity;
			mTraceLength = 0.0f;
			mDistanceTravelled = 0.0f;
			mDeathTimer = new MonoTimer();

			mPrevPosition = position;
			mUseRealPhysics = true;
			mPrevVelDirection = velocity;

			LayerOptIn(InteractionLayer.kGravityOrb);
		}



		/// <summary>
		/// Load laser texture
		/// </summary>
		public override void LoadContent()
		{
			mBombAnim = new Animator(Animator.PlayType.Repeat, ("Enemies/Futron-Rocket/Bomb1", 0.1f)
															 , ("Enemies/Futron-Rocket/Bomb2", 0.1f)
															 , ("Enemies/Futron-Rocket/Bomb3", 0.1f)
															 , ("Enemies/Futron-Rocket/Bomb4", 0.1f));
			mBombAnim.Play();

			const float EFT = 0.1f;
			mExplodingAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Futron-Rocket/Explosion1", EFT)
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

			mReticuleTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Futron-Rocket/Reticule");
		}

		#endregion rInitialiation





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			//Draw
			mTraceLength += dt * TRACE_LEN_INCREASE;
			mBombAnim.Update(gameTime);

			//Check death (this is bad...)
			if (mState == ProjectileState.Exploding)
			{
				mDeathTimer.Start();
				double elapsedTime = mDeathTimer.GetElapsedMs();
				if (DEATH_TIME_START < elapsedTime && elapsedTime < DEATH_TIME_END)
				{
					List<Entity> nearbyEntities = EntityManager.I.GetNearPos(DEATH_RADIUS, mExplosionCentre);

					foreach (Entity entity in nearbyEntities)
					{
						MovingEntity movingEntity = entity as MovingEntity;

						if (movingEntity != null)
						{
							Vector2 toEntity = entity.GetCentrePos() - mExplosionCentre;
							if (Vector2.Dot(toEntity, mExplosionNormal) > 0.0f)
							{
								KillPlayer(entity);
							}
						}
					}
				}
			}

			// Base
			base.Update(gameTime);

			mDistanceTravelled += Vector2.Distance(mPrevPosition, mPosition);
			mPrevPosition = mPosition;

			// Detect big swings in velocity to then avoid the trace changing too quickly
			float dirDiff = MonoMath.VectorDiff(mPrevVelDirection, mVelocity);
			if (dirDiff > 0.5f)// && MonoMath.VectorDiff(GravityVecNorm(), mVelocity) < 0.5f && mVelocity.LengthSquared() > 10.0f)
			{
				mTraceLength = 0.0f;
			}
			mPrevVelDirection = mVelocity;

			mDeathTimer.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw bomb/explosion
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			switch (mState)
			{
				case ProjectileState.FreeMotion:
					MonoDraw.DrawTexture(info, mBombAnim.GetCurrentTexture(), MonoMath.Round(mPosition) - new Vector2(2.0f, 0.0f));
					break;
				case ProjectileState.Exploding:
					DrawExplosion(info);
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
			if (mState != ProjectileState.FreeMotion)
			{
				return;
			}

			float traceLenRemaining = mTraceLength;
			GameTime timeStep = new GameTime(new TimeSpan(0), new TimeSpan(TimeSpan.TicksPerSecond / 60));
			FreeBodyEntity freeBodyEntity = new FreeBodyEntity(mPosition, mVelocity, LASER_BOMB_GRAVITY, GetGravityDir(), mTexture.Width);

			List<EntityCollision> collisions = new List<EntityCollision>();
			TileManager.I.GatherCollisions(timeStep, freeBodyEntity, ref collisions);

			float tracerDistanceTravelled = mDistanceTravelled;
			Vector2 startPoint = freeBodyEntity.GetPos() + TRACE_OFFSET;

			while (collisions.Count == 0 && traceLenRemaining > 0)
			{
				int tracerSegmentPrev = (int)(tracerDistanceTravelled / TRACE_SEGMENT_LENGTH);

				Vector2 v1 = freeBodyEntity.GetPos() + TRACE_OFFSET;
				freeBodyEntity.MoveTimeStep(timeStep);
				Vector2 v2 = freeBodyEntity.GetPos() + TRACE_OFFSET;

				float tracerMoveDelta = Vector2.Distance(v1, v2);
				tracerDistanceTravelled += tracerMoveDelta;
				traceLenRemaining -= tracerMoveDelta;

				int tracerSegmentNew = (int)(tracerDistanceTravelled / TRACE_SEGMENT_LENGTH);

				if (tracerSegmentNew > tracerSegmentPrev)
				{
					if (tracerSegmentNew % 2 == 1)
					{
						// Calculate end point and draw
						float lerpT = 1.0f + (tracerSegmentNew * TRACE_SEGMENT_LENGTH - tracerDistanceTravelled) / tracerMoveDelta;
						Vector2 endPoint = MonoMath.Lerp(v1, v2, lerpT);

						MonoDraw.DrawLineShadow(info, startPoint, endPoint, TRACE_COLOR, TRACE_COLOR_SHADOW, 2.0f, 2.0f, DrawLayer.SubEntity);
					}
					else
					{
						// Calculate start point
						float lerpT = 1.0f + (tracerSegmentNew * TRACE_SEGMENT_LENGTH - tracerDistanceTravelled) / tracerMoveDelta;
						startPoint = MonoMath.Lerp(v1, v2, lerpT);
					}
				}

				TileManager.I.GatherCollisions(timeStep, freeBodyEntity, ref collisions);
			}

			if (collisions.Count != 0)
			{
				EntityCollision firstCollision = MonoAlg.GetMin(ref collisions, EntityCollision.COLLISION_SORTER);
				CollisionResults firstResults = firstCollision.GetResult();

				Vector2 target = freeBodyEntity.GetPos() + firstResults.t.Value * freeBodyEntity.VelocityToDisplacement(timeStep);

				target.X -= mReticuleTexture.Width / 2.0f;
				target.Y -= mReticuleTexture.Height / 2.0f;

				if (firstResults.normal.Y < 0.0f)
				{
					target.Y += freeBodyEntity.ColliderBounds().Height;
				}
				if (firstResults.normal.X < 0.0f)
				{
					target.X += freeBodyEntity.ColliderBounds().Width;
				}

				MonoDraw.DrawTextureDepth(info, mReticuleTexture, target, DrawLayer.SubEntity);
			}
		}


		/// <summary>
		/// Draw explosion
		/// </summary>
		private void DrawExplosion(DrawInfo info)
		{
			Texture2D texture = mExplodingAnim.GetCurrentTexture();
			float rotation = MathF.PI * 0.5f - MathF.Atan2(-mExplosionNormal.Y, mExplosionNormal.X);
			Vector2 position = MonoMath.Round(mExplosionCentre) + (mExplosionNormal * texture.Height) + (MonoMath.Perpendicular(mExplosionNormal) * texture.Width * 0.5f);

			MonoDraw.DrawTexture(info, texture, position, rotation);
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Enable/Disable entity
		/// </summary>
		public override void SetEnabled(bool enabled)
		{
			// When disabling, recude trace size. This is so when it re-appears the trace doesn't suddenly appear.
			if (enabled == false)
			{
				mTraceLength = 0.0f;
			}

			base.SetEnabled(enabled);
		}

		#endregion rUtility
	}
}
