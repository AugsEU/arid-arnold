namespace AridArnold
{
	abstract class CameraMovement
	{
		CameraSpec mCurrentSpec;

		public void StartMovement(CameraSpec startingSpec)
		{
			mCurrentSpec = startingSpec;
			StartMovementInternal();
		}

		abstract protected void StartMovementInternal();

		abstract public bool IsMovementOver();

		public CameraSpec Update(GameTime gameTime)
		{
			UpdateInternal(gameTime);
			return mCurrentSpec;
		}

		protected abstract void UpdateInternal(GameTime gameTime);

	}
}
