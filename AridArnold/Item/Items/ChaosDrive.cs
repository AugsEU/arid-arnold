

namespace AridArnold
{
	internal class ChaosDrive : PassiveItem
	{
		Animator mFlashAnim;

		public ChaosDrive() : base("Items.ChaosTitle", "Items.ChaosDesc")
		{
			mFlashAnim = new Animator(Animator.PlayType.Repeat, 
									("Items/ChaosDrive/Flash1", 0.05f),
									("Items/ChaosDrive/Flash2", 0.05f),
									("Items/ChaosDrive/Flash3", 0.05f));
			mFlashAnim.Play();

			mTexture = mFlashAnim.GetCurrentTexture();
		}

		public override void Update(GameTime gameTime)
		{
			mFlashAnim.Update(gameTime);
			mTexture = mFlashAnim.GetCurrentTexture();

			base.Update(gameTime);
		}

		public override int GetPrice()
		{
			return -3;
		}
	}
}
