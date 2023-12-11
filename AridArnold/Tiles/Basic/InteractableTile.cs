namespace AridArnold.Tiles.Basic
{
	/// <summary>
	/// A tile you can interact with. Such as collectables or spikes.
	/// </summary>
	class InteractableTile : AirTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public InteractableTile(Vector2 position) : base(position)
		{
		}


		/// <summary>
		/// Get normal bounds
		/// </summary>
		/// <returns>Square bounds</returns>
		protected override Rect2f CalculateBounds()
		{
			return new Rect2f(mPosition, mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE));
		}
	}
}
