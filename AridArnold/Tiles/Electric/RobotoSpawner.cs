namespace AridArnold
{
	internal class RobotoSpawner : SquareTile
	{
		#region rConstants

		const float EXIT_VELOCITY_X = 10.0f;
		const float EXIT_VELOCITY_Y = 12.0f;
		const double SPAWN_TIME = 2000.0;
		const double CHARGE_UP_TIME = 800.0;

		#endregion rConstants





		#region rMembers

		Animator mOnAnim;
		Texture2D mPoweredTexture;
		PercentageTimer mSpawnTimer;
		bool mIsOn;

		#endregion rMembers


		#region rInit

		/// <summary>
		/// Create roboto spawner
		/// </summary>
		public RobotoSpawner(CardinalDirection rotation, Vector2 position) : base(position)
		{
			mIsOn = false;
			mRotation = rotation;
			mSpawnTimer = new PercentageTimer(SPAWN_TIME);
		}



		/// <summary>
		/// Load roboto spawner content
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/RobotoSpawnerOff");
			mPoweredTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/RobotoSpawnerPower");
			mOnAnim = new Animator(Animator.PlayType.Repeat,
				("Tiles/Lab/RobotoSpawnerOn1", 0.25f),
				("Tiles/Lab/RobotoSpawnerOn2", 0.25f));
			mOnAnim.Play();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update tile
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mOnAnim.Update(gameTime);
			mSpawnTimer.Update(gameTime);

			//Check surrounding tiles.
			EMField.ScanResults scan = TileManager.I.GetEMField().ScanAdjacent(mTileMapIndex);

			if (mSpawnTimer.GetPercentageF() > 0.02f || scan.mTotalPositiveElectric > 0.75f)
			{
				mIsOn = true;
			}
			else if (scan.mTotalPositiveElectric < 0.65f)
			{
				mIsOn = false;
			}

			if (mIsOn)
			{
				mSpawnTimer.Start();
				if (mSpawnTimer.GetElapsedMs() > SPAWN_TIME + CHARGE_UP_TIME)
				{
					SpawnRobot();
					mSpawnTimer.FullReset();
				}
			}
			else
			{
				mSpawnTimer.Stop();
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Spawn the robot
		/// </summary>
		void SpawnRobot()
		{
			Vector2 spawnDirection = Util.GetNormal(mRotation);

			// Spawn
			Roboto newRobot = new Roboto(Vector2.Zero);
			newRobot.PowerOn();
			EntityManager.I.RegisterEntity(newRobot);

			// Set Gravity
			CardinalDirection gravityCard = GravityOrb.GetEffectiveGravity();
			newRobot.SetGravity(gravityCard);

			// Set Velocity
			Vector2 spawnVel = spawnDirection;
			spawnVel.X *= EXIT_VELOCITY_X;
			spawnVel.Y *= EXIT_VELOCITY_Y;
			if (mRotation == gravityCard)
			{
				spawnVel.Y = 4.0f;
			}
			newRobot.SetVelocity(spawnVel);
			newRobot.SetPrevWalkDirFromVelocity();

			// Set Position
			Vector2 spawnPos = GetCentre() + spawnDirection * sTILE_SIZE * 0.75f;
			if (mRotation != gravityCard && Util.InvertDirection(mRotation) != gravityCard)
			{
				// Botch position a bit
				spawnPos -= Util.GetNormal(gravityCard);
			}
			newRobot.SetCentrePos(spawnPos);
		}


		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Texture2D GetTexture()
		{
			if (mIsOn)
			{
				return mSpawnTimer.GetElapsedMs() > SPAWN_TIME ? mOnAnim.GetCurrentTexture() : mPoweredTexture;
			}

			return mTexture;
		}

		#endregion rDraw
	}
}
