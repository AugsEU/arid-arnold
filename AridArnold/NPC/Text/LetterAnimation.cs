namespace AridArnold
{
	abstract class LetterAnimation
	{
		#region rAnimation

		/// <summary>
		/// Get current offset from the base position for this letter.
		/// </summary>
		public abstract Vector2 GetPosition(float lifeTime);

		/// <summary>
		/// Get current color for this letter.
		/// </summary>
		public abstract Color GetColor(float lifeTime);

		#endregion rAnimation
	}
}
