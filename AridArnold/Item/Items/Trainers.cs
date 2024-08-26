
namespace AridArnold
{
	internal class Trainers : PassiveItem
	{
		public Trainers() : base("Items.TrainersTitle", "Items.TrainersDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/Trainers/Boots");
		}

		public override int GetPrice()
		{
			return 10;
		}
	}
}
