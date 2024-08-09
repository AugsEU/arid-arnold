using System.Collections.Generic;

namespace AridArnold
{
	internal class SpacialSFX : GameSFX
	{
		const float DISTANCE_CUTOFF = 130.0f;

		Vector2 mPosition;
		Vector2 mVelocity;
		AudioPositionInfo mClosestListener;

		public SpacialSFX(AridArnoldSFX effect, Vector2 position, float maxVol, float minPitch = 0, float maxPitch = 0) : base(effect, maxVol, minPitch, maxPitch)
		{
			mVelocity = Vector2.Zero;
		}

		public override void UpdateListeners(List<AudioPositionInfo> listeners)
		{
			// Get emitters
			AudioPositionInfo emitterInfo = new AudioPositionInfo(mPosition, mVelocity);

			mBuffer.SetListeners(listeners.ToArray());
			mBuffer.SetEmitter(emitterInfo);

			mClosestListener = GetClosestListener(listeners);
		}

		AudioPositionInfo GetClosestListener(List<AudioPositionInfo> listeners)
		{
			if(listeners == null || listeners.Count == 0)
			{
				return new AudioPositionInfo(mPosition, mVelocity);
			}

			// Get clostest listener
			int minValueIdx = 0;

			Vector3 myPos3 = MonoMath.ToVec3(mPosition);
			for (int i = 1; i < listeners.Count; i++)
			{
				Vector3 temp = listeners[i].mPosition;

				if ((temp - myPos3).LengthSquared() < (listeners[minValueIdx].mPosition - myPos3).LengthSquared())
				{
					minValueIdx = i;
				}
			}

			return listeners[minValueIdx];
		}

		protected override float DecideVolume()
		{
			float baseVolume = base.DecideVolume();

			float distToClosest = (MonoMath.ToVec3(mPosition) - mClosestListener.mPosition).Length();

			float t = (DISTANCE_CUTOFF - distToClosest) / DISTANCE_CUTOFF;
			t = Math.Clamp(t, 0.0f, 1.0f);
			float volMod = MathHelper.Lerp(0.0f, 1.0f, t);

			return baseVolume * volMod;
		}

		public void SetPosition(Vector2 pos)
		{
			mPosition = pos;
		}

		public void SetVelocity(Vector2 velocity)
		{
			mVelocity = velocity;
		}
	}
}
