
namespace AridArnold
{
	/// <summary>
	/// Orb that flips gravity.
	/// </summary>
	class PocketOrb : OnceItem
	{
		public PocketOrb(int price) : base("Items.PocketOrbTitle", "Items.PocketOrbDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/PocketOrb/Orb");
		}

		protected override void DoEffect(Arnold arnold)
		{
			CardinalDirection currDir = GravityOrb.sActiveDirection;
			CardinalDirection newDir = Util.InvertDirection(currDir);

			GravityOrb.DoGravityShift(newDir);
		}
	}
}
