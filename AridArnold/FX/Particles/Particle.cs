namespace AridArnold
{
	/// <summary>
	/// Information for a single particle. Should be 16 bytes
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
			mAge = 0;
		}

		public Particle(Color color, Vector2 pos, Vector2 vel, byte textureIndex)
		{
			mColor = color;
			mPosition = pos;
			mVelocity = vel;
			mFlags = 0;
			mTextureIndex = textureIndex;
			mAge = 0;
		}

		public Color mColor;
		public Vector2 mPosition;
		public Vector2 mVelocity;
		public byte mFlags;
		public byte mTextureIndex;
		public ushort mAge;
	}
}
