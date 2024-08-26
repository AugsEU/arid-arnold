
namespace AridArnold
{
	abstract class PassiveItem : Item
	{
		protected PassiveItem(string titleID, string descID) : base(titleID, descID)
		{
		}

		public override void Update(GameTime gameTime)
		{
			// Can't record when using a passive item.
			GhostManager.I.StopRecording();
			base.Update(gameTime);
		}

		public override bool CanUseItem(Arnold arnold)
		{
			// Passive items can't be used
			return false;
		}
	}
}
