// Misc types used by entities.

namespace AridArnold
{
	/// <summary>
	/// A direction we can walk in.
	/// </summary>
	enum WalkDirection
	{
		Left,
		Right,
		None,
	}



	/// <summary>
	/// Comparison class to sort Entity for the OrderedUpdate
	/// </summary>
	class EntityUpdateSorter : IComparer<Entity>
	{
		public int Compare(Entity a, Entity b)
		{
			return b.GetUpdateOrder().CompareTo(a.GetUpdateOrder());
		}
	}
}
