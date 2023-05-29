namespace AridArnold
{
	/// <summary>
	/// Water bottle tile. Needed to complete most levels.
	/// </summary>
	class WaterBottleTile : TransientCollectableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public WaterBottleTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Bottle");
		}

		protected override TransientCollectable GetCollectableType()
		{
			return TransientCollectable.WaterBottle;
		}

		public override void OnCollect()
		{
			FXManager.I.AddAnimator(mPosition, "Tiles/BottleFade.max", DrawLayer.TileEffects);
		}

		public override float GetRotation() 
		{
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);

			return -gameCam.GetCurrentSpec().mRotation;
		}
	}
}
