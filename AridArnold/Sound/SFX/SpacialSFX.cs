using System.Collections.Generic;

namespace AridArnold
{
	internal class SpacialSFX : GameSFX
	{
		const float DISTANCE_CUTOFF = 230.0f;
		const float VOL_AT_CUTOFF = 0.1f;

		Vector2 mPosition;
		Vector2 mVelocity;
		AudioPositionInfo mClosestListener;
		float mDistanceCutoff = DISTANCE_CUTOFF;

		public SpacialSFX(AridArnoldSFX effect, Vector2 position, float maxVol, float minPitch = 0, float maxPitch = 0) : base(effect, maxVol, minPitch, maxPitch)
		{
			mPosition = position;
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

			float t = (mDistanceCutoff - distToClosest) / mDistanceCutoff;
			
			float volMod = 1.0f;
			if (t > 0.0f)
			{
				// Linear fall off.
				volMod = MathHelper.Lerp(VOL_AT_CUTOFF, 1.0f, t);
			}
			else
			{
				// Slow fall off
				volMod = (VOL_AT_CUTOFF) / (1.0f + VOL_AT_CUTOFF * t);
			}

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

		public void SetDistanceCutoff(float distanceCutoff)
		{
			mDistanceCutoff = distanceCutoff;
		}
	}
}
