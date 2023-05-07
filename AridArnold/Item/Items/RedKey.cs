namespace AridArnold
{
	internal class RedKey : Item
	{
		public RedKey()
		{
		}

		public override Animator GenerateAnimator()
		{
			return MonoData.I.LoadAnimator("Items/RedKey/Key");
		}

		public override int GetPrice()
		{
			return 3;
		}

		public override void UseItem()
		{
			EventManager.I.SendEvent(EventType.RedKeyUsed, new EArgs(this));
		}

		public override bool CanUseItem()
		{
			if(AnyRedLocksNearArnold() == false)
			{
				return false;
			}

			return base.CanUseItem();
		}

		bool AnyRedLocksNearArnold()
		{
			List<Entity> arnoldList = EntityManager.I.GetAllOfType(typeof(Arnold));

			foreach (Entity arnold in arnoldList)
			{
				Rectangle neighbours = TileManager.I.GetNbyN(arnold.GetCentrePos(), 3);
				for (int x = neighbours.X; x < neighbours.X + neighbours.Width; x++)
				{
					for (int y = neighbours.Y; y < neighbours.Y + neighbours.Height; y++)
					{
						Tile tile = TileManager.I.GetTile(x, y);
						if(tile is RedLockTile)
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
