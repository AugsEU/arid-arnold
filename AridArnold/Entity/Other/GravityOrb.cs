namespace AridArnold
{
	internal class GravityOrb : Entity
	{
		#region rConstants

		const float ANGULAR_SPEED = 0.2f;
		const float AMPLITUDE = 1.0f;

		#endregion rConstants




		#region rMembers

		public static CardinalDirection sActiveDirection = CardinalDirection.Down;
		Texture2D mActiveTexture;
		Texture2D mInactiveTexture;
		Vector2 mPushDisplacement;
		Vector2 mPushVelocity;
		Vector2 mFloatDisplacement;
		float mFloatAngle;

		CardinalDirection mGravityDir;
		Entity mActivatingEntity;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create orb at position
		/// </summary>
		public GravityOrb(Vector2 pos, CardinalDirection gravityDir) : base(pos)
		{
			// Cheeky way to make sure no orb is active at the start of the level.
			sActiveDirection = CardinalDirection.Down;

			mPosition.X += 3.0f;
			mPosition.Y += 3.0f;

			mFloatAngle = RandomManager.I.GetDraw().GetFloatRange(0.0f, 6.34f);

			mGravityDir = gravityDir;
		}



		/// <summary>
		/// Load orb content
		/// </summary>
		public override void LoadContent()
		{
			switch (mGravityDir)
			{
				case CardinalDirection.Up:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbUpActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbUp");
					break;
				case CardinalDirection.Right:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbRightActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbRight");
					break;
				case CardinalDirection.Down:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbDownActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbDown");
					break;
				case CardinalDirection.Left:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbLeftActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbLeft");
					break;
				default:
					break;
			}

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbBase");
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update orb
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mFloatAngle += dt * ANGULAR_SPEED;
			mFloatDisplacement.Y = MathF.Sin(mFloatAngle) * AMPLITUDE;

			mPushVelocity -= (mPushDisplacement) * dt;
			mPushVelocity *= 0.99f;
			mPushDisplacement += dt * mPushVelocity;

			if (mActivatingEntity is not null)
			{
				Rect2f ourCollider = ColliderBounds();
				Rect2f theirCollider = mActivatingEntity.ColliderBounds();

				if(!Collision2D.BoxVsBox(ourCollider, theirCollider))
				{
					// Release the lockout
					mActivatingEntity = null;
				}
			}

		}



		/// <summary>
		/// Handle collision
		/// </summary>
		/// <param name="entity"></param>
		public override void OnCollideEntity(Entity entity)
		{
			if (entity is not Arnold && entity is not FallingFarry)
			{
				return;
			}

			MovingEntity movingEntity = (MovingEntity)entity;
			EntityPush(movingEntity);

			SetActive(movingEntity);
		}



		/// <summary>
		/// Make this orb the active one.
		/// </summary>
		void SetActive(Entity activatingEntity)
		{
			// Do not allow activation if the activating entity hasn't left our collider yet.
			if (!IsActive() && mActivatingEntity is null)
			{
				// SFX
				SFXManager.I.PlaySFX(AridArnoldSFX.Collect, 0.3f);

				DoGravityShift(mGravityDir);

				// Pull entity towards orb for consistency
				Vector2 orbCentrePos = mPosition + new Vector2(Tile.sTILE_SIZE) * 0.5f;
				Vector2 entityCentrePos = activatingEntity.GetCentrePos();
				Vector2 entityPos = activatingEntity.GetPos();
				activatingEntity.SetPos(entityPos + (orbCentrePos - entityCentrePos) * 0.5f);

				mActivatingEntity = activatingEntity;
			}

			sActiveDirection = mGravityDir;
		}


		public static void DoGravityShift(CardinalDirection newDir)
		{
			// Camera turn.
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);

			ShakeAndRotateTo turnMove = new ShakeAndRotateTo(10.0f, GetTurnToAngle(newDir));

			GameSFX travelSFX = new GameSFX(AridArnoldSFX.LibraryRotate, 0.5f);
			GameSFX finishSFX = new GameSFX(AridArnoldSFX.LibraryBlockLand, 0.7f, -0.7f, -0.8f);

			turnMove.LoadSFX(travelSFX, finishSFX);

			gameCam.QueueMovement(turnMove);

			DiminishCameraShake shakeMove = new DiminishCameraShake(6.0f, 5.0f, 100.0f);
			gameCam.QueueMovement(shakeMove);


			SetAllEntitiesGravity(newDir);
		}


		/// <summary>
		/// Set the gravity of all entities.
		/// </summary>
		static void SetAllEntitiesGravity(CardinalDirection newDir)
		{
			int entityNum = EntityManager.I.GetEntityNum();

			for (int i = 0; i < entityNum; i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				if (entity.OnInteractLayer(InteractionLayer.kGravityOrb))
				{
					PlatformingEntity platformingEntity = (PlatformingEntity)entity;

					if (platformingEntity.GetGravityDir() != newDir)
					{
						platformingEntity.SetGravity(newDir);

						if (!platformingEntity.IsUsingRealPhysics())
						{
							platformingEntity.SetPrevWalkDirFromVelocity();
							platformingEntity.SetWalkDirection(WalkDirection.None);
							platformingEntity.OverrideVelocity(Vector2.Zero);
							platformingEntity.SetGrounded(false);
							platformingEntity.ResetAllJumpHelpers();
						}
						TileManager.I.UntangleEntityFromTiles(platformingEntity);
					}
				}
			}
		}

		#endregion rUpadate





		#region rDraw

		/// <summary>
		/// Draw the orb
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Texture2D tex = IsActive() ? mActiveTexture : mInactiveTexture;
			Vector2 pos = mPosition + mFloatDisplacement + mPushDisplacement;

			MonoDraw.DrawTextureDepth(info, tex, pos, DrawLayer.Tile);
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Is this orb active?
		/// </summary>
		bool IsActive()
		{
			return mGravityDir == sActiveDirection;
		}



		/// <summary>
		/// React to entity "pushin" us
		/// </summary>
		void EntityPush(MovingEntity entity)
		{
			Vector2 newVel = entity.GetVelocity();

			if (newVel.LengthSquared() < mPushVelocity.LengthSquared())
			{
				return;
			}

			mPushVelocity = newVel;

			mPushVelocity.X = MonoMath.ClampAbs(mPushVelocity.X, 5.0f);
			mPushVelocity.Y = MonoMath.ClampAbs(mPushVelocity.Y, 5.0f);
		}



		/// <summary>
		/// Get angle we need to turn to.
		/// </summary>
		static float GetTurnToAngle(CardinalDirection newDir)
		{
			switch (newDir)
			{
				case CardinalDirection.Up:
					return MathF.PI;
				case CardinalDirection.Right:
					return MathF.PI * 0.5f;
				case CardinalDirection.Down:
					return 0.0f;
				case CardinalDirection.Left:
					return MathF.PI * 1.5f;
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Get gravity direction from orbs
		/// </summary>
		static public CardinalDirection GetEffectiveGravity()
		{
			return sActiveDirection;
		}

		#endregion rUtility
	}
}
