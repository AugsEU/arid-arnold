﻿namespace AridArnold
{
	/// <summary>
	/// Simple algorithms
	/// </summary>
	static class MonoAlg
	{
		/// <summary>
		/// Swap two objects
		/// </summary>
		/// <typeparam name="T">Type of objects to swap</typeparam>
		/// <param name="lhs">Object 1</param>
		/// <param name="rhs">Object 2</param>
		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T temp;
			temp = lhs;
			lhs = rhs;
			rhs = temp;
		}



		/// <summary>
		/// Finds the maximum in a list.
		/// </summary>
		public static T GetMax<T>(ref List<T> list, IComparer<T> comparer)
		{
			T maxValue = list[0];

			for (int i = 1; i < list.Count; i++)
			{
				T temp = list[i];
				if (comparer.Compare(temp, maxValue) > 0)
				{
					maxValue = temp;
				}
			}

			return maxValue;
		}



		/// <summary>
		/// Finds the maximum in a list.
		/// </summary>
		public static T GetMin<T>(ref List<T> list, IComparer<T> comparer)
		{
			T minValue = list[0];

			for (int i = 1; i < list.Count; i++)
			{
				T temp = list[i];
				if (comparer.Compare(temp, minValue) < 0)
				{
					minValue = temp;
				}
			}

			return minValue;
		}



		/// <summary>
		/// Take a subsection of an int.
		/// </summary>
		public static UInt32 IntSubString(UInt32 value, int start, int bits)
		{
			UInt32 result = value >> start;
			UInt32 mask = (UInt32)(1 << bits) - 1;

			return result & mask;
		}
	}
}