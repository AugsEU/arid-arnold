using System;

namespace HorsesAndGun
{
	internal class RandomManager : Singleton<RandomManager>
	{
		Random mRandom = new Random();

		public int GetIntInRange(int low, int high)
		{
			return mRandom.Next(low, high + 1);
		}

		public bool GetRandBool(int truePercent)
		{
			return mRandom.Next(0, 100) < truePercent;
		}

	}
}
