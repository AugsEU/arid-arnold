namespace AridArnold
{
	/// <summary>
	/// Struct to specify the camera
	/// </summary>
	struct CameraSpec
	{
		public CameraSpec()
		{
			mPosition = Vector2.Zero;
			mRotation = 0.0f;
			mZoom = 1.0f;
		}

		public Vector2 mPosition;
		public float mRotation;
		public float mZoom;
	}


	/// <summary>
	/// Options for scaling and ordering
	/// </summary>
	struct SpriteBatchOptions
	{
		public SpriteBatchOptions()
		{
			mSortMode = SpriteSortMode.FrontToBack;
			mBlend = BlendState.AlphaBlend;
			mSamplerState = SamplerState.PointClamp;
			mDepthStencilState = DepthStencilState.Default;
			mRasterizerState = RasterizerState.CullNone;
		}

		public SpriteSortMode mSortMode;
		public BlendState mBlend;
		public SamplerState mSamplerState;
		public DepthStencilState mDepthStencilState;
		public RasterizerState mRasterizerState;
	}



	/// <summary>
	/// Camera for drawing.
	/// </summary>
	class Camera
	{
		#region rMembers

		CameraSpec mCurrentSpec;
		CameraMovement mCurrentCameraMovement;
		Queue<CameraMovement> mCameraMovements;
		RenderTarget2D mCurrentRenderTarget;
		RenderTarget2D mPrevRenderTarget;

		SpriteBatchOptions mSpriteBatchOptions;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create camera
		/// </summary>
		public Camera()
		{
			mCurrentSpec = new CameraSpec();
			mSpriteBatchOptions = new SpriteBatchOptions();
			mCameraMovements = new Queue<CameraMovement>();
			mCurrentCameraMovement = null;

			mPrevRenderTarget = null;
			mCurrentRenderTarget = null;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update camera
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update(GameTime gameTime)
		{
			CheckQueue();
			if (mCameraMovements.Count > 0)
			{
				mCurrentCameraMovement = mCameraMovements.Peek();
				mCurrentSpec = mCurrentCameraMovement.Update(gameTime);
			}
			else
			{
				mCurrentCameraMovement = null;
			}
		}



		/// <summary>
		/// Check movement queue.
		/// </summary>
		void CheckQueue()
		{
			if (mCameraMovements.Count == 0)
			{
				// Queue is emtpy; nothing to check.
				return;
			}

			CameraMovement topCamMove = mCameraMovements.Peek();
			CameraSpec endSpec = mCurrentSpec;

			if (mCurrentCameraMovement is null)
			{
				topCamMove.StartMovement(mCurrentSpec);
			}
			else if (mCurrentCameraMovement.IsMovementOver(ref endSpec))
			{
				mCurrentSpec = endSpec;
				mCameraMovements.Dequeue();
				if (mCameraMovements.Count > 0)
				{
					mCameraMovements.Peek().StartMovement(mCurrentSpec);
				}
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Start the sprite batch
		/// </summary>
		public void StartSpriteBatch(DrawInfo info, Vector2 viewPortSize, RenderTarget2D renderTarget2D, Color clearColor)
		{
			mPrevRenderTarget = null;
			mCurrentRenderTarget = renderTarget2D;

			info.device.SetRenderTarget(renderTarget2D);
			info.device.Clear(clearColor);
			info.spriteBatch.Begin(mSpriteBatchOptions.mSortMode,
									mSpriteBatchOptions.mBlend,
									mSpriteBatchOptions.mSamplerState,
									mSpriteBatchOptions.mDepthStencilState,
									mSpriteBatchOptions.mRasterizerState,
									null,
									CalculateMatrix(viewPortSize));


		}



		/// <summary>
		/// End the sprite batch
		/// </summary>
		/// <param name="info"></param>
		public void EndSpriteBatch(DrawInfo info)
		{
			info.spriteBatch.End();
			mPrevRenderTarget = mCurrentRenderTarget;
			mCurrentRenderTarget = null;
		}



		/// <summary>
		/// Caulate perspective matrix.
		/// </summary>
		Matrix CalculateMatrix(Vector2 viewPortSize)
		{
			Vector3 centrePoint3 = new Vector3(viewPortSize / 2.0f, 0.0f);

			return Matrix.CreateTranslation(-(int)mCurrentSpec.mPosition.X, -(int)mCurrentSpec.mPosition.Y, 0) *
				   Matrix.CreateTranslation(-centrePoint3) *
				   Matrix.CreateRotationZ(mCurrentSpec.mRotation) *
				   Matrix.CreateScale(new Vector3(mCurrentSpec.mZoom, mCurrentSpec.mZoom, 1)) *
				   Matrix.CreateTranslation(centrePoint3);
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Set sprite batch options
		/// </summary>
		public void SetOptions(SpriteBatchOptions options)
		{
			mSpriteBatchOptions = options;
		}


		/// <summary>
		/// Queue up a movement
		/// </summary>
		public void QueueMovement(CameraMovement movement)
		{
			mCameraMovements.Enqueue(movement);
		}


		/// <summary>
		/// Reset spec to default
		/// </summary>
		public void Reset()
		{
			mCameraMovements.Clear();
			mCurrentCameraMovement = null;
			mCurrentSpec = new CameraSpec();
		}


		/// <summary>
		/// Should we block the game update?
		/// </summary>
		public bool ShouldBlockUpdate()
		{
			if (mCurrentCameraMovement is not null)
			{
				return mCurrentCameraMovement.MovementBlocksUpdate();
			}

			return false;
		}



		/// <summary>
		/// Get the current spec
		/// </summary>
		public CameraSpec GetCurrentSpec()
		{
			return mCurrentSpec;
		}



		/// <summary>
		/// Set camera specifications
		/// </summary>
		public void ForceNewSpec(CameraSpec spec)
		{
			mCurrentSpec = spec;
		}



		/// <summary>
		/// Force camera to move to position
		/// </summary>
		public void ForcePosition(Vector2 position)
		{
			mCurrentSpec.mPosition = position;
		}



		/// <summary>
		/// Get copy of previous render target
		/// </summary>
		public RenderTarget2D GetPrevRenderTarget()
		{
			return mPrevRenderTarget;
		}

		#endregion rUtil
	}
}
