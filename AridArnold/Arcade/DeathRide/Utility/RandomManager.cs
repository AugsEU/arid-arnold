namespace GMTK2023
{
	/// <summary>
	/// Special random class we can control
	/// </summary>
	internal class MonoRandom
	{
		#region rConstants

		private const int A = 16807;
		private const int M = 2147483647;
		private const int Q = 127773;
		private const int R = 2836;

		#endregion rConstants





		#region rMembers

		private int mSeed;

		#endregion rMembers





		#region rSeedConfig

		/// <summary>
		/// Construct random from random variable
		/// </summary>
		public MonoRandom()
		{
			mSeed = new Random().Next();
		}



		/// <summary>
		/// Consctruct random from a seed
		/// </summary>
		/// <param name="_seed">Start seed</param>
		public MonoRandom(int _seed)
		{
			mSeed = _seed;
		}



		/// <summary>
		/// Set the random seed
		/// </summary>
		/// <param name="_seed">Seed to set</param>
		public void SetSeed(int _seed)
		{
			mSeed = _seed;
		}



		/// <summary>
		/// Incorperate number into our seed, changes seed in deterministic way based on the number and the seed.
		/// </summary>
		/// <param name="number">Number to chug</param>
		public void ChugNumber(int number)
		{
			mSeed += number;
			Next();
		}



		/// <summary>
		/// Get next random number
		/// </summary>
		/// <returns>Pseudo-random number</returns>
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

		#endregion rSeedConfig





		#region rAccessors

		/// <summary>
		/// Get integer within range(inclusive)
		/// </summary>
		/// <param name="low">Minimum value</param>
		/// <param name="high">Maximum value</param>
		/// <returns>Integer within range(inclusive)</returns>
		public int GetIntRange(int low, int high)
		{
			int num = Next();
			num = (num % (high - low + 1)) + low;

			return num;
		}

		/// <summary>
		/// Get float within the range 0-1
		/// </summary>
		/// <returns>Get float within the range (0,1]</returns>
		public float GetUnitFloat()
		{
			int num = Next();

			return (num * 1.0f) / M;
		}



		/// <summary>
		/// Get float within range(low,high]
		/// </summary>
		/// <param name="low">Minimum number</param>
		/// <param name="high">Maximum number</param>
		/// <returns>Float within range(low,high]</returns>
		public float GetFloatRange(float low, float high)
		{
			return GetUnitFloat() * (high - low) + low;
		}



		/// <summary>
		/// Random event with percent chance of being true.
		/// </summary>
		/// <param name="percent">Percent change of being true</param>
		/// <returns>True percent% of the time</returns>
		public bool PercentChance(float percent)
		{
			return GetFloatRange(0.0f, 100.0f) < percent;
		}


		public Vector2 GetRandomPoint(Rectangle area)
		{
			return new Vector2(GetFloatRange(area.X, area.X + area.Width), GetFloatRange(area.Y, area.Y + area.Height));
		}

		#endregion rAccessors
	}

	/// <summary>
	/// Simple manager to store a single random for all classes to use.
	/// </summary>
	internal class RandomManager : Singleton<RandomManager>
	{
		MonoRandom mWorldRandom = new MonoRandom();
		MonoRandom mDrawRandom = new MonoRandom();

		public MonoRandom GetWorld()
		{
			return mWorldRandom;
		}

		public MonoRandom GetDraw()
		{
			return mDrawRandom;
		}
	}
}
