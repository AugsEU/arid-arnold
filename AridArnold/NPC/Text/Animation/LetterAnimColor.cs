namespace AridArnold
{
	/// <summary>
	/// Letter animation that does nothing.
	/// </summary>
	internal class LetterAnimColor : LetterAnimation
	{
		Color mColor;

		public LetterAnimColor(Color color)
		{
			mColor = color;
		}

		public override Color GetColor(float lifeTime)
		{
			return mColor;
		}

		public override Vector2 GetPosition(float lifeTime)
		{
			return Vector2.Zero;
		}
	}
}
