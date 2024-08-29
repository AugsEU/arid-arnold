
namespace AridArnold
{
	internal class ReverseWatch : Item
	{
		public ReverseWatch(int price) : base("Items.ReverseWatchTitle", "Items.ReverseWatchDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/ReverseWatch/Watch");
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
