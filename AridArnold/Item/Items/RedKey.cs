namespace AridArnold
{
	internal class RedKey : Item
	{
		public RedKey() : base("Items.RedKeyTitle", "Items.RedKeyDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/RedKey/Key");
		}

		public override int GetPrice()
		{
			return 3;
		}

		public override void UseItem(Arnold arnoldUsingItem)
		{
			EventManager.I.TriggerEvent(EventType.RedKeyUsed);
		}

		public override bool CanUseItem(Arnold arnoldUsingItem)
		{
			if (AnyRedLocksNearArnold(arnoldUsingItem) == false)
			{
				return false;
			}

			return base.CanUseItem(arnoldUsingItem);
		}

		bool AnyRedLocksNearArnold(Arnold arnoldUsingItem)
		{
			Rectangle neighbours = TileManager.I.GetNbyN(arnoldUsingItem.GetCentrePos(), 3);
			for (int x = neighbours.X; x < neighbours.X + neighbours.Width; x++)
			{
				for (int y = neighbours.Y; y < neighbours.Y + neighbours.Height; y++)
				{
					Tile tile = TileManager.I.GetTile(x, y);
					if (tile is RedLockTile)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
