namespace AridArnold
{
	internal class SpacialSFX : GameSFX
	{
		Vector2 mPositon;
		Vector2 mVelocity;

		public SpacialSFX(AridArnoldSFX effect, Vector2 position, float minPitch = 0, float maxPitch = 0) : base(effect, minPitch, maxPitch)
		{
			mVelocity = Vector2.Zero;
		}

		public override void UpdateListeners(List<Vector2> listeners)
		{
			// Get clostest listener
			Vector2 closestListener = MonoAlg.GetMin<Vector2>(ref listeners, new DistanceComparer(mPositon));

			AudioPositionInfo listenerInfo = new AudioPositionInfo(closestListener, mVelocity);
			AudioPositionInfo emitterInfo = new AudioPositionInfo(mPositon, Vector2.Zero);

			mBuffer.SetListener(listenerInfo);
			mBuffer.SetEmitter(emitterInfo);
		}

		public void SetPosition(Vector2 pos)
		{
			mPositon = pos;
		}

		public void SetVelocity(Vector2 velocity)
		{
			mVelocity = velocity;
		}
	}
}
