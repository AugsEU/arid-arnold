namespace AridArnold
{
	internal class Nuke : SquareTile
	{
		#region rConstants

		const double READY_TO_BLOW_TIME = 1000.0;

		#endregion rConstants





		#region rMembers

		MonoTimer mExplodeTimer;
		Animator mReadyToBlowAnim;
		bool mExploded;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Make nuke
		/// </summary>
		public Nuke(Vector2 position) : base(position)
		{
			mExploded = false;
		}



		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/NukeOff");
			mReadyToBlowAnim = new Animator(Animator.PlayType.Repeat, ("Tiles/Lab/NukeOn", 0.2f), ("Tiles/Lab/NukeOff", 0.2f));
			mExplodeTimer = new MonoTimer();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update nuke
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mExplodeTimer.Update(gameTime);

			//Check surrounding tiles.
			EMField.ScanResults scan = TileManager.I.GetEMField().ScanAdjacent(mTileMapIndex);

			if (scan.mTotalPositiveElectric > 0.75f && !mExplodeTimer.IsPlaying())
			{
				mExplodeTimer.Start();
				mReadyToBlowAnim.Play();
			}

			if (mExplodeTimer.GetElapsedMs() > READY_TO_BLOW_TIME && !mExploded)
			{
				BlowUp();
			}

			mReadyToBlowAnim.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Explode
		/// </summary>
		public void BlowUp()
		{
			Camera gameCamera = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			gameCamera.QueueMovement(new CameraShake(100.0f, 7.0f, 166.0f));

			for (int i = 0; i < EntityManager.I.GetEntityNum(); i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				entity.Kill();
			}

			mExploded = true;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get the nuke texture
		/// </summary>
		public override Texture2D GetTexture()
		{
			if (mExplodeTimer.IsPlaying())
			{
				return mReadyToBlowAnim.GetCurrentTexture();
			}

			return mTexture;
		}

		#endregion rDraw
	}
}
