
namespace AridArnold
{
	internal class ReverseWatch : Item
	{
		public ReverseWatch() : base("Items.ReverseWatchTitle", "Items.ReverseWatchDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/ReverseWatch/Watch");
		}

		public override int GetPrice()
		{
			return 7;
		}

		public override bool CanUseItem(Arnold arnold)
		{
			// It's a passive item.
			return false;
		}

		public override bool RegenerateAfterDeath()
		{
			return false;
		}
	}
}
