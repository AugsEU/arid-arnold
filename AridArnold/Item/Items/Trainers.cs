
namespace AridArnold
{
	internal class Trainers : PassiveItem
	{
		public Trainers(int price) : base("Items.TrainersTitle", "Items.TrainersDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/Trainers/Boots");
		}
	}
}
