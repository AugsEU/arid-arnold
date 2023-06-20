using System.Net.NetworkInformation;

namespace AridArnold
{
	internal class RobotoSpawner : SquareTile
	{
		#region rConstants

		const float EXIT_VELOCITY_X = 7.0f;
		const float EXIT_VELOCITY_Y = 12.0f;

		#endregion rConstants





		#region rMembers

		Animator mOnAnim;
		Roboto mActiveRobot;
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
			mActiveRobot = null;
		}



		/// <summary>
		/// Load roboto spawner content
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/RobotoSpawnerOff");
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

			//Check surrounding tiles.
			EMField.ScanResults scan = TileManager.I.GetEMField().ScanAdjacent(mTileMapIndex);

			if (scan.mTotalPositiveElectric > 0.55f)
			{
				mIsOn = true;
			}
			else if (scan.mTotalPositiveElectric < 0.45f)
			{
				mIsOn = false;
			}

			if(mIsOn)
			{
				if(mActiveRobot is null)
				{
					SpawnRobot();
				}
				else
				{
					if(mActiveRobot.CheckOffScreenDeath())
					{
						mActiveRobot = null;
					}
				}
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Spawn the robot
		/// </summary>
		void SpawnRobot()
		{
			if(mActiveRobot is not null)
			{
				EntityManager.I.QueueDeleteEntity(mActiveRobot);
			}

			Vector2 spawnDirection = Util.GetNormal(mRotation);
			Vector2 spawnVel = spawnDirection;
			spawnVel.X *= EXIT_VELOCITY_X;
			spawnVel.Y *= EXIT_VELOCITY_Y;
			if(mRotation == CardinalDirection.Down)
			{
				spawnVel.Y = 4.0f;
			}
			Vector2 spawnPos = mPosition + spawnDirection * sTILE_SIZE;
			spawnPos.X += 2.0f;
			spawnPos.Y -= sTILE_SIZE / 2.0f;

			Roboto newRobot = new Roboto(spawnPos);
			newRobot.PowerOn();
			newRobot.SetVelocity(spawnVel);
			newRobot.SetPrevWalkDirFromVelocity();
			mActiveRobot = newRobot;
			EntityManager.I.RegisterEntity(mActiveRobot);
		}


		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Texture2D GetTexture()
		{
			if(mIsOn)
			{
				return mOnAnim.GetCurrentTexture();
			}

			return mTexture;
		}

		#endregion rDraw
	}
}
