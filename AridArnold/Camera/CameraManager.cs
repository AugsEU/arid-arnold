namespace AridArnold
{
	internal class CameraManager : Singleton<CameraManager>
	{
		#region rTypes

		public enum CameraInstance
		{
			GameAreaCamera,
			ScreenCamera,
			GlobalCamera,
			NumCameraInstances
		}

		#endregion rTypes





		#region rMembers

		Camera[] mCameraDictionary;

		#endregion rMembers





		#region rInit

		public void Init()
		{
			int numInstances = (int)CameraInstance.NumCameraInstances;
			mCameraDictionary = new Camera[numInstances];
			for(int i = 0; i < numInstances; i++)
			{
				mCameraDictionary[i] = new Camera();
			}
		}

		#endregion rInit





		#region rUtil

		public Camera GetCamera(CameraInstance camera)
		{
			return mCameraDictionary[(int)camera];
		}


		#endregion rUtil
	}
}
