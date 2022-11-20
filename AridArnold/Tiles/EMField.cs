namespace AridArnold
{
	/// <summary>
	/// Represents ethereal field that tiles can affect.
	/// </summary>
	class EMField
	{
		#region rTypes

		/// <summary>
		/// A single value in the field.
		/// </summary>
		public struct EMValue
		{
			public float mElectric;
			public bool mConductive;

			public EMValue()
			{
				mElectric = 0.0f;
				mConductive = false;
			}

			public void Normalise()
			{
				mElectric = Math.Clamp(mElectric, 0.0f, 1.0f);
			}

			public void Reset()
			{
				mElectric = 0.0f;
			}
		}

		#endregion rTypes





		#region rMembers

		EMValue[,] mCurrentField;
		EMValue[,] mNextField;

		#endregion rMembers





		#region rInitialisation

		public EMField(int size)
		{
			mCurrentField = new EMValue[size, size];
			mNextField = new EMValue[size, size];
		}

		#endregion rInitialisation


		#region rUpdate

		/// <summary>
		/// Processes all the inputs given.
		/// </summary>
		public void ProcessUpdate()
		{
			//Next field is now the current one.
			Util.Swap(ref mCurrentField, ref mNextField);

			//Zero out next field.
			for(int x = 0; x < mNextField.GetLength(0); x++)
			{
				for(int y = 0; y < mNextField.GetLength(1); y++)
				{
					mNextField[x, y].Reset();
				}
			}
		}

		#endregion rUpdate





		#region rAccess

		/// <summary>
		/// Get value at a point in the EM field.
		/// </summary>
		public EMValue GetValue(Point index)
		{
			return mCurrentField[index.X, index.Y];
		}



		/// <summary>
		/// Get value at a point in the EM field with a delta.
		/// </summary>
		public EMValue GetValue(Point index, Point delta)
		{
			index += delta;
			if (!InEMField(index))
			{
				return new EMValue();
			}

			return GetValue(index);
		}



		/// <summary>
		/// Add electricity to a point
		/// </summary>
		public void SetElectricity(Point index, float elec)
		{
			mNextField[index.X, index.Y].mElectric = elec;
		}



		/// <summary>
		/// Add electricity to a point with a delta position
		/// </summary>
		public void SetElectricity(Point index, Point delta, float elec)
		{
			index += delta;
			if(!InEMField(index))
			{
				return;
			}

			SetElectricity(index, elec);
		}


		/// <summary>
		/// Register a point as conductive.
		/// </summary>
		public void RegisterConductive(Point index)
		{
			mNextField[index.X, index.Y].mConductive = true;
			mCurrentField[index.X, index.Y].mConductive = true;
		}



		/// <summary>
		/// Is this point conductive?
		/// </summary>
		public bool IsConductive(Point index)
		{
			return mCurrentField[index.X, index.Y].mConductive;
		}



		/// <summary>
		/// Add electricity to a point with a delta position
		/// </summary>
		public bool IsConductive(Point index, Point delta)
		{
			index += delta;
			if (!InEMField(index))
			{
				return false;
			}

			return IsConductive(index);
		}

		#endregion rAccess





		#region rUtil

		/// <summary>
		/// Is this point even in the field?
		/// </summary>
		bool InEMField(Point index)
		{
			if(index.X < 0 || index.Y < 0)
			{
				return false;
			}

			if(index.X >= mCurrentField.GetLength(0) || index.Y >= mCurrentField.GetLength(1))
			{
				return false;
			}

			return true;
		}

		#endregion rUtil
	}
}
