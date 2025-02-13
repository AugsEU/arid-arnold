﻿namespace AridArnold
{
	internal class RedKey : OnceItem
	{
		public RedKey(int price) : base("Items.RedKeyTitle", "Items.RedKeyDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/RedKey/Key");
		}

		protected override void DoEffect(Arnold arnold)
		{
			EventManager.I.TriggerEvent(EventType.RedKeyUsed);
		}

		public override bool CanUseItem(Arnold arnold)
		{
			if (AnyRedLocksNearArnold(arnold) == false)
			{
				return false;
			}

			return base.CanUseItem(arnold);
		}

		bool AnyRedLocksNearArnold(Arnold arnold)
		{
			Rectangle neighbours = TileManager.I.GetNbyN(arnold.GetCentrePos(), 3);
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
