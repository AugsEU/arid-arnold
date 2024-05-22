namespace GMTK2023
{
	/// <summary>
	/// Simple singleton implementation. It uses lazy initialisation.
	/// </summary>
	/// <typeparam name="TClass">Use CRTP to make singleton of yourself</typeparam>
	abstract class Singleton<TClass> where TClass : class, new()
	{
		protected Singleton()
		{
		}

		public static TClass I { get { return Nested.instance; } }

		private class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested()
			{
			}

			internal static readonly TClass instance = new TClass();
		}
	}
}
