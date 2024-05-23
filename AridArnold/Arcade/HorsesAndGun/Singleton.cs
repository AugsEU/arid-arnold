namespace HorsesAndGun
{
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
