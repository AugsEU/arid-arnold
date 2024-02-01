namespace AridArnold
{
	struct MonoRange<T> where T : IComparable<T>
	{
		#region rMembers

		T mMinimum;
		T mMaximum;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Init range
		/// </summary>
		public MonoRange(T min, T max)
		{
			mMinimum = min;
			mMaximum = max;
		}

		#endregion rInit





		#region rUtil

		/// <summary>
		/// Set minimum value
		/// </summary>
		public T GetMin()
		{
			return mMinimum;
		}



		/// <summary>
		/// Set the minimum value
		/// </summary>
		public void SetMin(T min)
		{
			mMinimum = min;
		}



		/// <summary>
		/// Get maximum value
		/// </summary>
		public T GetMax()
		{
			return mMaximum;
		}



		/// <summary>
		/// Set maximum value
		/// </summary>
		public void SetMax(T max)
		{
			mMaximum = max;
		}



		/// <summary>
		/// Is valid range(minimum below max)
		/// </summary>
		public bool IsValid()
		{
			return mMinimum.CompareTo(mMaximum) <= 0;
		}



		/// <summary>
		/// Contains []
		/// </summary>
		public bool ContainsCC(T value)
		{
			return (mMinimum.CompareTo(value) <= 0) && (value.CompareTo(mMaximum) <= 0);
		}


		/// <summary>
		/// Contains (]
		/// </summary>
		public bool ContainsOC(T value)
		{
			return (mMinimum.CompareTo(value) < 0) && (value.CompareTo(mMaximum) <= 0);
		}



		/// <summary>
		/// Contains [)
		/// </summary>
		public bool ContainsCO(T value)
		{
			return (mMinimum.CompareTo(value) <= 0) && (value.CompareTo(mMaximum) < 0);
		}



		/// <summary>
		/// Contains ()
		/// </summary>
		public bool ContainsOO(T value)
		{
			return (mMinimum.CompareTo(value) < 0) && (value.CompareTo(mMaximum) < 0);
		}

		#endregion rUtil
	}
}
