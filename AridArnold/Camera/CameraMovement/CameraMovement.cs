namespace AridArnold
{
	abstract class CameraMovement
	{
		protected CameraSpec mStartSpec;
		protected CameraSpec mCurrentSpec;

		public virtual void StartMovement(CameraSpec startingSpec)
		{
			mStartSpec = startingSpec;
			mCurrentSpec = startingSpec;
		}

		public virtual void Update(GameTime gameTime)
		{
		}

		abstract public CameraSpec EndMovementSpec();

		abstract public bool IsMovementOver();

		public virtual bool MovementBlocksUpdate() { return false; }

		public CameraSpec GetCurrentSpec() { return mCurrentSpec; }
	}
}
