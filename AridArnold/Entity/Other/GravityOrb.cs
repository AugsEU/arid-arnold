namespace AridArnold
{
	internal class GravityOrb : Entity
	{
		#region rConstants

		const float ANGULAR_SPEED = 0.2f;
		const float AMPLITUDE = 1.0f;

		#endregion rConstants




		#region rMembers

		static GravityOrb sActiveOrb = null;
		Texture2D mActiveTexture;
		Texture2D mInactiveTexture;
		Vector2 mPushDisplacement;
		Vector2 mPushVelocity;
		Vector2 mFloatDisplacement;
		float mFloatAngle;

		CardinalDirection mGravityDir;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create orb at position
		/// </summary>
		public GravityOrb(Vector2 pos, CardinalDirection gravityDir) : base(pos)
		{
			// Cheeky way to make sure no orb is active at the start of the level.
			sActiveOrb = null;

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
			if (!IsActive())
			{
				Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
				gameCam.QueueMovement(new ShakeAndRotateTo(10.0f, GetTurnToAngle()));
				SetAllEntitiesGravity();

				// Pull entity towards orb for consistency
				Vector2 orbCentrePos = mPosition + new Vector2(Tile.sTILE_SIZE) * 0.5f;
				Vector2 entityCentrePos = activatingEntity.GetCentrePos();
				Vector2 entityPos = activatingEntity.GetPos();
				activatingEntity.SetPos(entityPos + (orbCentrePos - entityCentrePos) * 0.5f);
			}
			sActiveOrb = this;
		}



		/// <summary>
		/// Set the gravity of all entities.
		/// </summary>
		void SetAllEntitiesGravity()
		{
			int entityNum = EntityManager.I.GetEntityNum();

			for (int i = 0; i < entityNum; i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				if (entity is PlatformingEntity)
				{
					PlatformingEntity platformingEntity = (PlatformingEntity)entity;

					if (platformingEntity.GetGravityDir() != mGravityDir)
					{
						platformingEntity.SetGravity(mGravityDir);

						if (!platformingEntity.GetIsUsingRealPhysics())
						{
							platformingEntity.SetPrevWalkDirFromVelocity();
							platformingEntity.SetWalkDirection(WalkDirection.None);
							platformingEntity.SetVelocity(Vector2.Zero);
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
			return object.ReferenceEquals(this, sActiveOrb);
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
		float GetTurnToAngle()
		{
			switch (mGravityDir)
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

		#endregion rUtility
	}
}
