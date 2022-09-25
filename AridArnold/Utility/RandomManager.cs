namespace AridArnold
{
	internal class MonoRandom
	{
		private const int A = 16807;
		private const int M = 2147483647;
		private const int Q = 127773;
		private const int R = 2836;
		private int mSeed;

		public MonoRandom()
		{
			mSeed = new Random().Next();
		}

		public MonoRandom(int _seed)
		{
			mSeed = _seed;
		}

		public void SetSeed(int _seed)
		{
			mSeed = _seed;
		}

		public void ChugNumber(int number)
		{
			mSeed += number;
			Next();
		}

		public int Next()
		{
			int hi = mSeed / Q;
			int lo = mSeed % Q;

			mSeed = (A * lo) - (R * hi);

			if (mSeed <= 0)
			{
				mSeed = mSeed + M;
			}

			return mSeed;
		}

		public int GetIntRange(int low, int high)
		{
			int num = Next();
			num = (num % (high - low + 1)) + low;

			return num;
		}

		public float GetUnitFloat()
		{
			int num = Next();

			return (num * 1.0f) / (float)M;
		}

		public float GetFloatRange(float low, float high)
		{
			return GetUnitFloat() * (high - low) + low;
		}

		public bool PercentChance(float percent)
		{
			return GetFloatRange(0.0f, 100.0f) < percent;
		}
	}

	internal class RandomManager : Singleton<RandomManager>
	{
		MonoRandom mWorldRandom = new MonoRandom();

		public MonoRandom GetWorld()
		{
			return mWorldRandom;
		}
	}
}
