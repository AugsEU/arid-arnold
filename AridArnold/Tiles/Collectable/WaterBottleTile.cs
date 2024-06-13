namespace AridArnold
{
	/// <summary>
	/// Water bottle tile. Needed to complete most levels.
	/// </summary>
	class WaterBottleTile : PermCollectSimpleTile
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
			mFullAnim = MonoData.I.LoadAnimator("Tiles/Bottle");
			mGhostAnim = MonoData.I.LoadAnimator("Tiles/BottleGhost");
		}

		protected override PermanentCollectable GetCollectableType()
		{
			return PermanentCollectable.WaterBottle;
		}

		protected override void OnCollect()
		{
			FXManager.I.AddAnimator(mPosition, "Tiles/BottleFade.max", DrawLayer.TileEffects);
			CollectableManager.I.CollectTransientItem(TransientCollectable.WaterBottle);
			base.OnCollect();
		}

		public override float GetRotation()
		{
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);

			return -gameCam.GetCurrentSpec().mRotation;
		}
	}
}
