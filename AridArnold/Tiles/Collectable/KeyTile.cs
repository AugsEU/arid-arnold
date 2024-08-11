namespace AridArnold
{
	class KeyTile : PermCollectSimpleTile
	{
		public KeyTile(Vector2 pos) : base(pos)
		{
		}

		public override void LoadContent()
		{
			mFullAnim = MonoData.I.LoadAnimator("Tiles/KeyFull");
			mGhostAnim = MonoData.I.LoadAnimator("Tiles/KeyGhost");
			LoadSFX(AridArnoldSFX.CollectKey, 0.0f);
			base.LoadContent();
		}

		protected override CollectableCategory GetCollectableType()
		{
			return CollectableCategory.Key;
		}

		protected override void OnCollect()
		{
			EventManager.I.TriggerEvent(EventType.KeyCollect);
			base.OnCollect();
		}
	}
}
