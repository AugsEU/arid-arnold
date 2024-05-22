namespace GMTK2023
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

		/// <summary>
		/// Init all cameras(called once per launch)
		/// </summary>
		public void Init()
		{
			int numInstances = (int)CameraInstance.NumCameraInstances;
			mCameraList = new Camera[numInstances];
			for (int i = 0; i < numInstances; i++)
			{
				mCameraList[i] = new Camera();
			}
		}

		#endregion rInit



		#region rUpdate

		/// <summary>
		/// Update all the cameras
		/// </summary>
		public void UpdateAllCameras(GameTime gameTime)
		{
			foreach (Camera camera in mCameraList)
			{
				camera.Update(gameTime);
			}
		}

		#endregion rUpdate



		#region rUtil

		/// <summary>
		/// Get a camera of type
		/// </summary>
		public Camera GetCamera(CameraInstance camera)
		{
			return mCameraList[(int)camera];
		}



		/// <summary>
		/// Do any of our cameras want us to block the update?
		/// </summary>
		public bool BlockUpdateRequested()
		{
			foreach (Camera cam in mCameraList)
			{
				if (cam.ShouldBlockUpdate()) return true;
			}

			return false;
		}

		#endregion rUtil
	}
}
