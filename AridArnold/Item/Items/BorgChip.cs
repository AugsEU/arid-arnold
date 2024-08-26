
namespace AridArnold
{
	internal class BorgChip : PassiveItem
	{
		public BorgChip() : base("Items.BorgChipTitle", "Items.BorgChipDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/BorgChip/Chip");
		}

		public override int GetPrice()
		{
			return 10;
		}
	}
}
