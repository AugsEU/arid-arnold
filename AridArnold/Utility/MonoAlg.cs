﻿using static AridArnold.EMField;

namespace AridArnold
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
		/// Find index of item in list
		/// </summary>
		public static int GetIndex<T>(List<T> list, T toFind)
		{
			for (int i = 0; i < list.Count; ++i)
			{
				if (object.ReferenceEquals(list[i], toFind))
				{
					return i;
				}
			}

			return -1;
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



		/// <summary>
		/// Shuffle a list
		/// </summary>
		public static void ShuffleList<T>(ref List<T> list, ref MonoRandom rng)
		{
			int n = list.Count;

			while (n > 0)
			{
				n--;
				int newIdx = rng.GetIntRange(0, n);
				T temp = list[newIdx];
				list[newIdx] = list[n];
				list[n] = temp;
			}
		}



		/// <summary>
		/// Test if a value contains a flag.
		/// </summary>
		static public bool TestFlag<T>(T value, params T[] flags) where T : Enum
		{
			UInt64 value64 = Convert.ToUInt64(value);
			foreach (T flag in flags)
			{
				UInt64 flag64 = Convert.ToUInt64(flag);
				if ((value64 & flag64) != 0)
				{
					return true;
				}
			}
			return false;
		}



		/// <summary>
		/// Add value to existing flag(or them). You have to do the casting yourself since generics suck
		/// </summary>
		static public UInt64 AddFlags<T>(T original, params T[] flags) where T : Enum
		{
			UInt64 value64 = Convert.ToUInt64(original);
			foreach (T flag in flags)
			{
				UInt64 flag64 = Convert.ToUInt64(flag);
				value64 |= flag64;
			}

			return value64;
		}



		/// <summary>
		/// Remove values from existing flag(or them). You have to do the casting yourself since generics suck
		/// </summary>
		static public UInt64 SubFlags<T>(T original, params T[] flags) where T : Enum
		{
			UInt64 value64 = Convert.ToUInt64(original);
			foreach (T flag in flags)
			{
				UInt64 flag64 = Convert.ToUInt64(flag);
				value64 &= ~flag64;
			}

			return value64;
		}



		/// <summary>
		/// Check if two objects are of the same type.
		/// Null is not the same as anything
		/// </summary>
		public static bool TypeCompare(object a, object b)
		{
			if (a == null || b == null)
			{
				return false;
			}

			return a.GetType() == b.GetType();
		}



		/// <summary>
		/// Get subset of array
		/// </summary>
		public static T[] GetSubArray<T>(T[] source, int startIndex, int length)
		{
			T[] result = new T[length];
			Array.Copy(source, startIndex, result, 0, length);
			return result;
		}



		/// <summary>
		/// Get the first element in curr that isn't in prev. O(n^2)
		/// </summary>
		public static T GetFirstNewElement<T>(T[] curr, T[] prev)
		{
			foreach (T currElement in curr)
			{
				bool isNewElement = true;

				foreach (T prevElement in prev)
				{
					if (currElement.Equals(prevElement))
					{
						isNewElement = false;
						break;
					}
				}

				if (isNewElement)
				{
					return currElement;
				}
			}

			// Return default value for type T, typically null for reference types or default(T) for value types
			return default(T);
		}
	}

	/// <summary>
	/// Compares distances for sorting.
	/// </summary>
	public class DistanceComparer : IComparer<Vector2>
	{
		private readonly Vector2 mTarget;

		public DistanceComparer(Vector2 target)
		{
			mTarget = target;
		}

		public int Compare(Vector2 v1, Vector2 v2)
		{
			float dist1 = Vector2.DistanceSquared(v1, mTarget);
			float dist2 = Vector2.DistanceSquared(v2, mTarget);

			if (dist1 < dist2)
				return -1;
			else if (dist1 > dist2)
				return 1;
			else
				return 0;
		}
	}
}
