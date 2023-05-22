namespace AridArnold
{
	abstract class CameraMovement
	{
		protected CameraSpec mCurrentSpec;

		public void StartMovement(CameraSpec startingSpec)
		{
			mCurrentSpec = startingSpec;
			StartMovementInternal();
		}

		abstract protected void StartMovementInternal();

		abstract protected void EndMovementInternal();

		public bool IsMovementOver()
		{
			bool over = IsMovementOverInternal();
			if (over)
			{
				EndMovementInternal();
			}

			return over;
		}

		abstract protected bool IsMovementOverInternal();

		public CameraSpec Update(GameTime gameTime)
		{
			UpdateInternal(gameTime);
			return mCurrentSpec;
		}

		protected abstract void UpdateInternal(GameTime gameTime);

		public virtual bool MovementBlocksUpdate() { return false; }

	}
}
