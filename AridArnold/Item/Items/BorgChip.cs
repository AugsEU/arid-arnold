
namespace AridArnold
{
	internal class BorgChip : PassiveItem
	{
		public BorgChip(int price) : base("Items.BorgChipTitle", "Items.BorgChipDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/BorgChip/Chip");
		}
	}
}
