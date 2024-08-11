using System.ComponentModel;

namespace AridArnold
{
	/// <summary>
	/// Entity that can run and jump.
	/// </summary>
	abstract class PlatformingEntity : MovingEntity
	{
		#region rConstants

		const float DEFAULT_WALK_SPEED = 9.0f;
		protected const float DEFAULT_GRAVITY = 4.35f;
		const float DEFAULT_JUMP_SPEED = 25.0f;
		const float MAX_VELOCITY = 65.0f;
		const double DEATH_TIME = 500.0;

		protected const double FLASH_TIME = 100.0;
		protected const float NOT_MOVING_SPEED = 1.0f;


		#endregion rConstants





		#region rMembers

		protected bool mOnGround;
		int mAllowChangeDirFrames;

		//Magniture of motion vectors
		protected float mWalkSpeed;
		protected float mJumpSpeed;
		protected float mGravity;
		protected bool mUseRealPhysics;

		int mUpdatesSinceGrounded;
		int mUpdatesSinceJump;

		private CardinalDirection mGravityDirection;
		protected WalkDirection mWalkDirection;
		protected WalkDirection mPrevDirection;

		// Timers
		protected PercentageTimer mTimerSinceDeath;

		// Ice
		protected int mIceWalking;

		// Particles
		float mDustIntensity;

		// Sound
		float mJumpVolume = 0.4f;
		AridArnoldSFX? mJumpSound;
		SpacialSFX mWalkSound;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Platforming entity constructor
		/// </summary>
		/// <param name="pos">Starting position.</param>
		/// <param name="walkSpeed">Ground walk speed.</param>
		/// <param name="jumpSpeed">Initial upwards velocity when jumping.</param>
		/// <param name="gravity">Gravity acceleration.</param>
		public PlatformingEntity(Vector2 pos, float walkSpeed = DEFAULT_WALK_SPEED, float jumpSpeed = DEFAULT_JUMP_SPEED, float gravity = DEFAULT_GRAVITY) : base(pos)
		{
			mVelocity = Vector2.Zero;
			mWalkDirection = WalkDirection.None;
			mPrevDirection = mWalkDirection;
			mUseRealPhysics = false;

			mWalkSpeed = walkSpeed;
			mJumpSpeed = jumpSpeed;
			mGravity = gravity;

			mGravityDirection = CardinalDirection.Down;

			mUpdatesSinceGrounded = int.MaxValue;
			mUpdatesSinceJump = int.MaxValue;

			// Timers
			mTimerSinceDeath = new PercentageTimer(DEATH_TIME);

			// Ice
			mIceWalking = 0;

			// Allow things to make you change direction for a certain number of frames.
			mAllowChangeDirFrames = 0;

			// Default no dust
			mDustIntensity = 0.0f;

			// Opt into this by default
			LayerOptIn(InteractionLayer.kGravityOrb);
		}


		protected void LoadSFX(AridArnoldSFX? jumpType, float jumpVolume, AridArnoldSFX? walkType, float walkVolume)
		{
			mJumpSound = jumpType;
			mJumpVolume = jumpVolume;

			mWalkSound = new SpacialSFX(walkType.Value, mPosition, walkVolume);
			mWalkSound.GetBuffer().SetLoop(true);
			mWalkSound.SetMute(true);
			SFXManager.I.PlaySFX(mWalkSound);
		}

		#endregion





		#region rUpdate

		/// <summary>
		/// Update platforming entity
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			mTimerSinceDeath.Update(gameTime);

			//Death
			if (mTimerSinceDeath.IsPlaying())
			{
				if (mTimerSinceDeath.GetPercentage() == 1.0)
				{
					EntityManager.I.QueueDeleteEntity(this);
				}

				return;
			}

			bool walkMute = true;
			if (mOnGround)
			{
				mUpdatesSinceGrounded = 0;
				mUpdatesSinceJump = int.MaxValue;

				if (mWalkDirection != WalkDirection.None && mVelocity.LengthSquared() > 0.5f)
				{
					EmitWalkDust();
					walkMute = false;
				}
			}
			else if (mUpdatesSinceGrounded != int.MaxValue)
			{
				mUpdatesSinceGrounded++;
			}

			if (mUpdatesSinceJump != int.MaxValue)
			{
				mUpdatesSinceJump++;
			}

			if (mWalkSound is not null)
			{
				mWalkSound.SetMute(walkMute);
				mWalkSound.SetPosition(mPosition);
				mWalkSound.SetVelocity(mVelocity);
			}

			mIceWalking = Math.Max(mIceWalking-1, 0);

			if (mAllowChangeDirFrames > 0) mAllowChangeDirFrames--;

			base.Update(gameTime);
		}



		/// <summary>
		/// Physics stuff
		/// </summary>
		public override void OrderedUpdate(GameTime gameTime)
		{
			if (mTimerSinceDeath.IsPlaying())
			{
				return;
			}

			Vector2 downVec = GravityVecNorm();
			Vector2 sideVec = new Vector2(MathF.Abs(downVec.Y), MathF.Abs(downVec.X));
			float prevSideVel = Vector2.Dot(mPrevVelocity, sideVec);
			float currSideVel = Vector2.Dot(mVelocity, sideVec);

			if (!mUseRealPhysics)
			{
				WalkDirection motorDirection = WalkDirection.None;
				// Ice
				if (mIceWalking > 0 && IsGroundedSince(4) && MathF.Abs(prevSideVel) > 0.5f * mWalkSpeed)
				{
					motorDirection = mPrevDirection;
					MonoDebug.Assert(motorDirection != WalkDirection.None);
				}
				else
				{
					if (!mOnGround && !CanWalkDirChange())
					{
						SetWalkDirectionFromSideVel();
					}

					motorDirection = mWalkDirection;
				}
				
				SetSideVelocityFromDirection(motorDirection);
			}

			ApplyGravity(gameTime);

			if (!mUseRealPhysics)
			{
				mVelocity = new Vector2(MonoMath.ClampAbs(mVelocity.X, MAX_VELOCITY), MonoMath.ClampAbs(mVelocity.Y, MAX_VELOCITY));
			}

			mOnGround = false;
			base.OrderedUpdate(gameTime);
		}



		/// <summary>
		/// Set "motor" based on sideways velocity.
		/// </summary>
		private void SetWalkDirectionFromSideVel()
		{
			Vector2 downVec = GravityVecNorm();
			Vector2 sideVec = new Vector2(MathF.Abs(downVec.Y), MathF.Abs(downVec.X));
			float prevSideVel = Vector2.Dot(mPrevVelocity, sideVec);

			if (prevSideVel > mWalkSpeed * 0.85f)
			{
				mWalkDirection = WalkDirection.Right;
			}
			else if (prevSideVel < -mWalkSpeed * 0.85f)
			{
				mWalkDirection = WalkDirection.Left;
			}
		}



		/// <summary>
		/// Set side velocity from a direction
		/// </summary>
		private void SetSideVelocityFromDirection(WalkDirection dir)
		{
			Vector2 downVec = GravityVecNorm();
			Vector2 sideVec = new Vector2(MathF.Abs(downVec.Y), MathF.Abs(downVec.X));

			float component = Vector2.Dot(mVelocity, sideVec);
			float desiredComponent = component;
			switch (dir)
			{
				case WalkDirection.Left:
					desiredComponent = -mWalkSpeed;
					break;
				case WalkDirection.Right:
					desiredComponent = +mWalkSpeed;
					break;
				case WalkDirection.None:
					if (CanWalkDirChange())
					{
						desiredComponent = 0.0f;
					}
					break;
			}

			// Don't need motor already going fast enough
			if (CanWalkDirChange() || MathF.Abs(component) < MathF.Abs(desiredComponent) + 1.0f)
			{
				Vector2 velToAdd = MonoMath.TruncateSmall((desiredComponent - component) * sideVec);

				mVelocity += velToAdd;
			}
		}


		/// <summary>
		/// React to collision with a block.
		/// </summary>
		/// <param name="normal">Normal vector of collision.</param>
		public override void ReactToCollision(Vector2 normal)
		{
			CollisionType collisionType;

			float dp = Vector2.Dot(normal, GravityVecNorm());

			if (dp >= 0.95f)
			{
				collisionType = CollisionType.Ceiling;
			}
			else if (dp <= -0.95f)
			{
				collisionType = CollisionType.Ground;
			}
			else
			{
				collisionType = CollisionType.Wall;
			}

			ReactToCollision(collisionType);
		}



		/// <summary>
		/// React to collision with walls/ceilings/floors
		/// </summary>
		protected virtual void ReactToCollision(CollisionType collisionType)
		{
			switch (collisionType)
			{
				case CollisionType.Ground:
					mOnGround = true;

					float downVel = Vector2.Dot(mPrevVelocity, GravityVecNorm());
					if(mUpdatesSinceGrounded > 4 && downVel > mJumpSpeed * 0.9f)
					{
						EmitDustLand();
					}
					break;
				case CollisionType.Ceiling:
					if (mOnGround == false)
					{
						mVelocity = GravityVecNorm() * mGravity;
					}
					break;
			}
		}



		/// <summary>
		/// Move entity under the force of gravity
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		protected void ApplyGravity(GameTime gameTime)
		{
			float vecAlongGrav = Vector2.Dot(GravityVecNorm(), mVelocity);

			float mod = 1.0f;
			if (vecAlongGrav < 0.0f && !mUseRealPhysics)
			{
				mod = 2.0f;
			}

			float delta = mGravity * Util.GetDeltaT(gameTime) * mod;
			Vector2 deltaVec = GravityVecNorm() * delta;

			if (-delta < vecAlongGrav && vecAlongGrav < 0.0f && !mUseRealPhysics)
			{
				deltaVec = deltaVec / 3.5f;
			}

			mVelocity += deltaVec;
		}



		/// <summary>
		/// Get the update ordering. Higher value means update will happen first.
		/// </summary>
		protected override void CalculateUpdateOrder()
		{
			// If one entity is standing on another, and they both jump on the same frame,
			// we want the one who is standing on top to go first.
			Vector2 gravityVec = -GravityVecNorm();

			mUpdateOrder = Vector2.Dot(mPosition, gravityVec);
			mUpdateOrder = MonoMath.SquashToRange(mUpdateOrder, EntityManager.UPDATE_MENTITY_MIN, EntityManager.UPDATE_MENTITY_MAX);
		}



		/// <summary>
		/// Inform this entity it is on ice.
		/// </summary>
		public void SetIceWalking()
		{
			mIceWalking = 2;
		}



		/// <summary>
		/// Kill Entity
		/// </summary>
		public override void Kill()
		{
			mTimerSinceDeath.Start();
		}



		/// <summary>
		/// Are we dying?
		/// </summary>
		public bool IsDying()
		{
			return mTimerSinceDeath.IsPlaying();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the platformer.
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public override void Draw(DrawInfo info)
		{
			Texture2D textureToDraw = GetDrawTexture();
			Rect2f originalTextureRect = ColliderBounds();
			Color colorToDraw = GetDrawColor();
			CardinalDirection gravityDir = GetGravityDir();
			DrawLayer drawLayer = GetDrawLayer();

			MonoDraw.DrawPlatformer(info, originalTextureRect, textureToDraw, colorToDraw, gravityDir, mPrevDirection, drawLayer);
		}



		/// <summary>
		/// Get texture to draw for this platformer.
		/// </summary>
		public virtual Texture2D GetDrawTexture()
		{
			return mTexture;
		}



		/// <summary>
		/// Get color to apply when drawing.
		/// </summary>
		public virtual Color GetDrawColor()
		{
			if (mTimerSinceDeath.IsPlaying())
			{
				double timeSinceDeath = mTimerSinceDeath.GetElapsedMs();

				if ((int)(timeSinceDeath / FLASH_TIME) % 2 == 0)
				{
					return new Color(255, 51, 33);
				}
				else
				{
					return new Color(255, 128, 79);
				}
			}

			return Color.White;
		}



		/// <summary>
		/// Get the layer that this platformer should be drawn on.
		/// </summary>
		public virtual DrawLayer GetDrawLayer()
		{
			return DrawLayer.Default;
		}

		#endregion rDraw



		#region rParticles

		/// <summary>
		/// Set out of 100 how many dust particles to spawn
		/// </summary>
		protected void SetDustIntensity(float intensity)
		{
			mDustIntensity = intensity;
		}



		/// <summary>
		/// Spawn dust from walking.
		/// </summary>
		void EmitWalkDust()
		{
			if (!RandomManager.I.GetDraw().PercentChance(mDustIntensity))
			{
				return;
			}

			Vector2[] footPositions = GetFeetCheckPoints();
			footPositions[1] -= GravityVecNorm() * 1.5f;

			DustUtil.EmitDust(footPositions[1], -GravityVecNorm());
		}



		/// <summary>
		/// Spawn dust cloud at arnold's feet for big landings
		/// </summary>
		void EmitDustLand()
		{
			if (mDustIntensity == 0.0f)
			{
				return;
			}

			Vector2[] footPositions = GetFeetCheckPoints(0.0f);

			for(int i = 0; i < footPositions.Length; i++)
			{
				Vector2 spot = footPositions[i];

				DustUtil.EmitDust(spot, -GravityVecNorm());
			}
		}


		/// <summary>
		/// Spawn dust cloud at arnold's feet for big landings
		/// </summary>
		void EmitDustJump()
		{
			if (mDustIntensity == 0.0f)
			{
				return;
			}
			EmitDustLand();

			Vector2[] footPositions = GetFeetCheckPoints(1.0f);

			for (int i = 0; i < footPositions.Length; i++)
			{
				if (i == 1) continue;
				Vector2 spot = footPositions[i];
				spot -= GravityVecNorm();
				DustUtil.EmitDust(spot, -GravityVecNorm());
			}
		}

		#endregion rParticles



		#region rUtility

		/// <summary>
		/// Set gravity direction
		/// </summary>
		/// <param name="dir">New gravity direction</param>
		public void SetGravity(CardinalDirection dir)
		{
			mGravityDirection = dir;
		}

		/// <summary>
		/// Get current gravity direction
		/// </summary>
		/// <returns>Current gravity direction</returns>
		public CardinalDirection GetGravityDir()
		{
			return mGravityDirection;
		}



		/// <summary>
		/// Are we using realistic physics, without modfications for control?
		/// </summary>
		public bool IsUsingRealPhysics()
		{
			return mUseRealPhysics;
		}


		/// <summary>
		/// Get gravity normal.
		/// </summary>
		/// <returns>Get vector along gravity direction</returns>
		/// <exception cref="NotImplementedException">Must be a cardinal direction</exception>
		public Vector2 GravityVecNorm()
		{
			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
					return new Vector2(0.0f, 1.0f);
				case CardinalDirection.Up:
					return new Vector2(0.0f, -1.0f);
				case CardinalDirection.Left:
					return new Vector2(-1.0f, 0.0f);
				case CardinalDirection.Right:
					return new Vector2(1.0f, 0.0f);
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Set walk direction
		/// </summary>
		/// <param name="dir">New walk direction</param>
		public WalkDirection GetWalkDirection()
		{
			return mWalkDirection;
		}



		/// <summary>
		/// Get velocity relative to "down"
		/// </summary>
		public Vector2 GetVelocityRelativeToGravity()
		{
			float currentYVelocity = -Vector2.Dot(GravityVecNorm(), mVelocity);
			float currentXVelocity = -Vector2.Dot(MonoMath.Perpendicular(GravityVecNorm()), mVelocity);

			return new Vector2(currentXVelocity, currentYVelocity);
		}



		/// <summary>
		/// Set walk direction
		/// </summary>
		/// <param name="dir">New walk direction</param>
		public void SetWalkDirection(WalkDirection dir)
		{
			mWalkDirection = dir;
		}



		/// <summary>
		/// Set the "previous" walk direction
		/// </summary>
		/// <param name="dir">New walk direction</param>
		public WalkDirection GetPrevWalkDirection()
		{
			return mPrevDirection;
		}



		/// <summary>
		/// Set the "previous" walk direction
		/// </summary>
		/// <param name="dir">New walk direction</param>
		public void SetPrevWalkDirection(WalkDirection dir)
		{
			mPrevDirection = dir;
		}



		/// <summary>
		/// Change walk direction to be opposite.
		/// </summary>
		public void ReverseWalkDirection()
		{
			switch (mWalkDirection)
			{
				case WalkDirection.Left:
					mWalkDirection = WalkDirection.Right;
					break;
				case WalkDirection.Right:
					mWalkDirection = WalkDirection.Left;
					break;
				case WalkDirection.None:
				default:
					break;
			}
		}


		/// <summary>
		/// Set previous walk direction from the velocity we have. Prevents jumping backwards with mirrors.
		/// </summary>
		public void SetPrevWalkDirFromVelocity()
		{
			const float AIR_DIR_THRESH = 0.25f;
			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
				case CardinalDirection.Up:
					if (mVelocity.X > AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Right;
					}
					else if (mVelocity.X < -AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Left;
					}
					break;
				case CardinalDirection.Right:
				case CardinalDirection.Left:
					if (mVelocity.Y > AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Right;
					}
					else if (mVelocity.Y < -AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Left;
					}
					break;
			}
		}



		/// <summary>
		/// Make entity jump.
		/// </summary>
		protected void Jump()
		{
			Vector2 gravVelocity = GetVelocityRelativeToGravity();

			if (mIceWalking > 0)
			{
				if (MathF.Abs(gravVelocity.X) >= NOT_MOVING_SPEED)
				{
					mWalkDirection = mPrevDirection;
					//mOnGround = false;
				}
				mIceWalking = 0;
			}

			if (gravVelocity.Y < mJumpSpeed)
			{
				mVelocity = -mJumpSpeed * GravityVecNorm();

				if (mUpdatesSinceJump > 10)
				{
					EmitDustJump();

					if (mJumpSound.HasValue)
					{
						SpacialSFX jumpSFX = new SpacialSFX(mJumpSound.Value, GetCentrePos(), mJumpVolume, -0.02f, 0.04f);
						SFXManager.I.PlaySFX(jumpSFX);
					}
				}
			}

			mUpdatesSinceJump = 0;
		}



		/// <summary>
		/// Have we been grounded in the last x frames?
		/// </summary>
		public bool IsGroundedSince(int frames)
		{
			if (mOnGround)
			{
				mUpdatesSinceGrounded = 0;
			}
			return mUpdatesSinceGrounded < frames;
		}



		/// <summary>
		/// Have we jumped in the last x frames?
		/// </summary>
		public bool HasJumpedInTheLast(int frames)
		{
			return mUpdatesSinceJump < frames;
		}



		/// <summary>
		/// Undo jump helper timers
		/// </summary>
		public void ResetAllJumpHelpers()
		{
			mUpdatesSinceJump = int.MaxValue;
			mUpdatesSinceGrounded = int.MaxValue;
		}


		/// <summary>
		/// Get collider based on texture, but reduced.
		/// Width is reduced centre-aligned
		/// Height is reduced from the top
		/// </summary>
		/// <param name="widthReduction">Amount of width to reduce</param>
		/// <param name="heightReduction">Amount of height to reduce</param>
		protected Rect2f GetReducedTextureCollider(float widthReduction, float heightReduction)
		{
			float height = mTexture.Height - heightReduction;
			float width = mTexture.Width - widthReduction;

			Vector2 effectivePosition;
			Vector2 effectiveSize;

			if (GetGravityDir() == CardinalDirection.Left || GetGravityDir() == CardinalDirection.Right)
			{
				effectivePosition = mPosition + new Vector2(widthReduction, heightReduction / 2.0f);
				effectiveSize = new Vector2(height, width);
			}
			else
			{
				effectivePosition = mPosition + new Vector2(widthReduction / 2.0f, heightReduction);
				effectiveSize = new Vector2(width, height);
			}

			return new Rect2f(effectivePosition, effectivePosition + effectiveSize);
		}



		/// <summary>
		/// Grounded property. 
		/// </summary>
		public bool OnGround()
		{
			return mOnGround;
		}



		/// <summary>
		/// Set grounded.
		/// </summary>
		public void SetGrounded(bool value)
		{
			mOnGround = value;
		}



		/// <summary>
		/// Overriding the velocity should
		/// </summary>
		public override void OverrideVelocity(Vector2 vel)
		{
			base.OverrideVelocity(vel);

			SetPrevWalkDirFromVelocity();
			mWalkDirection = WalkDirection.None;
		}



		/// <summary>
		/// Check if we died from going off-screen
		/// </summary>
		/// <returns>True if we should die</returns>
		/// <exception cref="NotImplementedException">Required valid cardinal direction</exception>
		public bool CheckOffScreenDeath()
		{
			switch (GetGravityDir())
			{
				case CardinalDirection.Up:
					return mPosition.Y < -mTexture.Height / 2.0f;
				case CardinalDirection.Right:
					return mPosition.X > GameScreen.GAME_AREA_WIDTH + 2.0f * mTexture.Width;
				case CardinalDirection.Down:
					return mPosition.Y > GameScreen.GAME_AREA_HEIGHT + mTexture.Height / 2.0f;
				case CardinalDirection.Left:
					return mPosition.X < -mTexture.Width / 2.0f;
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// If you were at the position, which way would you need to walk to get to me in my reference frame?
		/// </summary>
		public WalkDirection DirectionNeededToWalkToMe(Vector2 pos)
		{
			Vector2 down = GravityVecNorm();
			Vector2 toPosition = pos - GetCentrePos();

			float cross = down.X * toPosition.Y - down.Y * toPosition.X;

			if (cross > 0.0f)
			{
				return WalkDirection.Right;
			}
			else if (cross < 0.0f)
			{
				return WalkDirection.Left;
			}

			return WalkDirection.None;
		}


		/// <summary>
		/// Gets 3 points at our "feet" to check against.
		/// 0 - Left
		/// 1 - Middle
		/// 2 - Right
		/// </summary>
		public Vector2[] GetFeetCheckPoints(float footShift = -2.0f)
		{
			Vector2[] retValue = new Vector2[3];
			Rect2f collider = ColliderBounds();
			Vector2 gravityVec = GravityVecNorm();

			switch (mGravityDirection)
			{
				case CardinalDirection.Up:
					retValue[0] = new Vector2(collider.min.X + footShift, collider.min.Y);
					retValue[2] = new Vector2(collider.max.X - footShift, collider.min.Y);
					break;
				case CardinalDirection.Right:
					retValue[0] = new Vector2(collider.max.X, collider.max.Y - footShift);
					retValue[2] = new Vector2(collider.max.X, collider.min.Y + footShift);
					break;
				case CardinalDirection.Down:
					retValue[0] = new Vector2(collider.min.X + footShift, collider.max.Y);
					retValue[2] = new Vector2(collider.max.X - footShift, collider.max.Y);
					break;
				case CardinalDirection.Left:
					retValue[0] = new Vector2(collider.min.X, collider.min.Y + footShift);
					retValue[2] = new Vector2(collider.min.X, collider.max.Y - footShift);
					break;
				default:
					break;
			}

			retValue[1] = 0.5f * (retValue[0] + retValue[2]);

			for (int i = 0; i < 3; ++i)
			{
				retValue[i] += gravityVec;
			}

			return retValue;
		}



		/// <summary>
		/// Allow changes in walk direction for a number of frames
		/// </summary>
		/// <param name="frames"></param>
		public void AllowWalkChangeFor(int frames)
		{
			mAllowChangeDirFrames = frames;
		}



		/// <summary>
		/// Can we set our direction?
		/// </summary>
		protected bool CanWalkDirChange()
		{
			return mOnGround || mAllowChangeDirFrames > 0;
		}

		#endregion rUtility





		#region rSpacial

		/// <summary>
		/// Get a tile relative to our centre of mass.
		/// </summary>
		/// <param name="dx">Number of horizontal tiles from CoM</param>
		/// <param name="dy">Number of vertical tiles from CoM</param>
		/// <returns>Tile reference</returns>
		protected Tile GetNearbyTile(int dx, int dy)
		{
			Vector2 down = GravityVecNorm() * Tile.sTILE_SIZE;
			Vector2 right = MonoMath.Perpendicular(down);

			if (GetGravityDir() == CardinalDirection.Right || GetGravityDir() == CardinalDirection.Up)
			{
				right = -right;
			}

			Vector2 tilePos = mCentreOfMass + dx * right + dy * down;

			return TileManager.I.GetTile(tilePos);
		}



		/// <summary>
		/// Check if a tile is solid
		/// </summary>
		/// <param name="dx">Number of horizontal tiles from CoM</param>
		/// <param name="dy">Number of horizontal tiles from CoM</param>
		/// <returns>True if nearby tile is solid.</returns>
		protected bool CheckSolid(int dx, int dy)
		{
			return GetNearbyTile(dx, dy).IsSolid();
		}



		/// <summary>
		/// Are we touching a wall to the left
		/// </summary>
		protected bool IsTouchingLeftWall()
		{
			float colWidth = ColliderBounds().Width / 2.0f + 1.2f;

			return ProbeLateral(colWidth);
		}



		/// <summary>
		/// Are we touching a wall to the left
		/// </summary>
		protected bool IsTouchingRightWall()
		{
			float colWidth = ColliderBounds().Width / 2.0f + 1.2f;

			return ProbeLateral(-colWidth);
		}



		/// <summary>
		/// Probe laterally for solids
		/// </summary>
		private bool ProbeLateral(float dist)
		{
			Vector2 up = -GravityVecNorm();
			Vector2 left = MonoMath.Perpendicular(up);

			if (GetGravityDir() == CardinalDirection.Right || GetGravityDir() == CardinalDirection.Up)
			{
				left = -left;
			}

			Vector2 testPoint = mCentreOfMass + dist * left;

			Tile leftTile = TileManager.I.GetTile(testPoint);
			return leftTile is not null && leftTile.IsSolid();
		}

		#endregion rSpacial
	}
}
