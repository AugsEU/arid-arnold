namespace AridArnold
{
	/// <summary>
	/// A tile you can collect
	/// </summary>
	abstract class TransientCollectableTile : InteractableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public TransientCollectableTile(Vector2 position) : base(position)
		{
		}

		protected abstract TransientCollectable GetCollectableType();

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				CollectableManager.I.CollectTransientItem(GetCollectableType());
				mEnabled = false;
				OnCollect();
			}
		}

		/// <summary>
		/// Called when the tile is collected.
		/// </summary>
		public virtual void OnCollect()
		{
		}
	}
}
