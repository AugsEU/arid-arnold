
namespace AridArnold
{
	internal class MoonBoots : PassiveItem
	{
		public MoonBoots(int price) : base("Items.MoonBootsTitle", "Items.MoonBootsDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/MoonBoots/Boots");
		}
	}
}
