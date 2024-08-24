
namespace AridArnold
{
	/// <summary>
	/// Orb that flips gravity.
	/// </summary>
	class PocketOrb : OnceItem
	{
		public PocketOrb() : base("Items.PocketOrbTitle", "Items.PocketOrbDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/PocketOrb/Orb");
		}

		public override int GetPrice()
		{
			return 2;
		}

		protected override void DoEffect(Arnold arnold)
		{
			CardinalDirection currDir = GravityOrb.sActiveDirection;
			CardinalDirection newDir = Util.InvertDirection(currDir);

			GravityOrb.DoGravityShift(newDir);
		}
	}
}
