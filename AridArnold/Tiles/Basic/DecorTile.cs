namespace AridArnold
{
	/// <summary>
	/// A tile that has no texture or collisions.
	/// </summary>
	class DecorTile : AirTile
	{
		string mTexPath;

		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public DecorTile(Vector2 position, CardinalDirection dir, string texturePath) : base(position, texturePath)
		{
			mTexPath = texturePath;
			mRotation = dir;
		}

		/// <summary>
		/// Get empty bounds
		/// </summary>
		/// <returns>Zero bounds</returns>
		protected override Rect2f CalculateBounds()
		{
			return new Rect2f(mPosition, sTILE_SIZE, sTILE_SIZE);
		}
	}
}
