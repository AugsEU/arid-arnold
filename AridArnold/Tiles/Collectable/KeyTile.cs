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
			base.LoadContent();
		}

		protected override PermanentCollectable GetCollectableType()
		{
			return PermanentCollectable.Key;
		}

		protected override void OnCollect()
		{
			EventManager.I.SendEvent(EventType.KeyCollect, new EArgs(this));
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				if (mIsGhost == false)
				{
					CollectableManager.I.CollectTentativeItem(mTileMapIndex, GetItemType());
				}
				mEnabled = false;
				OnCollect();
			}
		}
	}
}
