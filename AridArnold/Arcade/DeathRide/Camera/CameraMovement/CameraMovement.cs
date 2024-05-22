namespace GMTK2023
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

		abstract protected void EndMovementInternal(ref CameraSpec endSpec);

		public bool IsMovementOver(ref CameraSpec endSpec)
		{
			bool over = IsMovementOverInternal();
			if (over)
			{
				EndMovementInternal(ref endSpec);
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
