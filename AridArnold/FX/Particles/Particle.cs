namespace AridArnold
{
	/// <summary>
	/// Information for a single particle. Should be 24 bytes
	/// </summary>
	struct Particle
	{
		public Particle(Color color, Vector2 pos, Vector2 vel)
		{
			mColor = color;
			mPosition = pos;
			mVelocity = vel;
			mFlags = 0;
			mTextureIndex = 0;
			mLifetime = 0;
		}

		public Particle(Color color, Vector2 pos, Vector2 vel, byte textureIndex, ushort lifetime)
		{
			mColor = color;
			mPosition = pos;
			mVelocity = vel;
			mFlags = 0;
			mTextureIndex = textureIndex;
			mLifetime = lifetime;
		}

		public Color mColor;
		public Vector2 mPosition;
		public Vector2 mVelocity;
		public byte mFlags;
		public byte mTextureIndex;
		public ushort mLifetime;
	}
}
