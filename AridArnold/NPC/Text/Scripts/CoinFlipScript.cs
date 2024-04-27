
namespace AridArnold
{
	internal class CoinFlipScript : OneShotScript
	{
		public CoinFlipScript(SmartTextBlock parentBlock) : base(parentBlock)
		{
		}

		protected override void DoOneShot()
		{
			MonoRandom rng = RandomManager.I.GetWorld();
			bool heads = rng.PercentChance(50.0f);

			string toAppend = heads ? "Heads!" : "Tails!";
			GetSmartTextBlock().AppendText(toAppend);
		}
	}
}
