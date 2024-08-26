
namespace AridArnold
{
	internal class MoonBoots : PassiveItem
	{
		public MoonBoots() : base("Items.MoonBootsTitle", "Items.MoonBootsDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/MoonBoots/Boots");
		}

		public override int GetPrice()
		{
			return 10;
		}
	}
}
