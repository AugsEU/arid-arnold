
namespace AridArnold
{
	internal class BorgChip : Item
	{
		public BorgChip() : base("Items.BorgChipTitle", "Items.BorgChipDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/BorgChip/Chip");
		}

		public override int GetPrice()
		{
			return 10;
		}

		public override bool CanUseItem(Arnold arnold)
		{
			// It's a passive item.
			return false;
		}
	}
}
