
namespace AridArnold
{
	internal class ArmyBoots : PassiveItem
	{
		public ArmyBoots(int price) : base("Items.ArmyBootsTitle", "Items.ArmyBootsDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/Army/Boots");
		}
	}
}
