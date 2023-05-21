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

		Camera[] mCameraList;

		#endregion rMembers





		#region rInit

		public void Init()
		{
			int numInstances = (int)CameraInstance.NumCameraInstances;
			mCameraList = new Camera[numInstances];
			for(int i = 0; i < numInstances; i++)
			{
				mCameraList[i] = new Camera();
			}
		}

		#endregion rInit



		#region rUpdate

		public void UpdateAllCameras(GameTime gameTime)
		{
			foreach(Camera camera in mCameraList)
			{
				camera.Update(gameTime);
			}
		}

		#endregion rUpdate



		#region rUtil

		public Camera GetCamera(CameraInstance camera)
		{
			return mCameraList[(int)camera];
		}


		#endregion rUtil
	}
}
