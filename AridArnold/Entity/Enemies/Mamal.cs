namespace AridArnold
{
	internal class Mamal : AIEntity
	{
		#region rTypes

		enum State
		{
			Wait,
			ChargeAtPlayer,
		}

		#endregion rTypes





		#region rConstants

		const float MAMAL_JUMP_SPEED = 0.0f;
		const float MAMAL_WALK_SPEED = 3.5f;

		#endregion rConstants






		#region rMembers

		StateMachine<State> mStateMachine;
		bool mIsAwake;

		#endregion rMembers


		#region rInitialisation

		/// <summary>
		/// Create mamal at position
		/// </summary>
		public Mamal(Vector2 pos) : base(pos, MAMAL_WALK_SPEED, MAMAL_JUMP_SPEED, 0.0f, 0.0f)
		{
			mPosition.Y -= 16.0f;
			mIsAwake = TimeZoneManager.I.GetCurrentTimeZone() == 1;
			mStateMachine = new StateMachine<State>(mIsAwake ? State.ChargeAtPlayer : State.Wait);
		}



		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
				("Enemies/Mamal/Anger1", 0.15f),
				("Enemies/Mamal/Anger2", 0.15f)
				);

			mStandAnimation = mRunningAnimation;
			mStandAnimation.Play();
			mRunningAnimation.Play();

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Mamal/Peace1");
		}

		#endregion rInitialisation



		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			mStateMachine.Update(gameTime);

			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));

			base.Update(gameTime);
		}



		/// <summary>
		/// Decide on action
		/// </summary>
		protected override void DecideActions()
		{
			if (mIsAwake)
			{
				mStateMachine.ForceGoToStateAndWait(State.ChargeAtPlayer, 0.0);
			}
			else
			{
				mStateMachine.ForceGoToStateAndWait(State.Wait, 0.0);
			}

			EnforceState();
		}



		/// <summary>
		/// Perform action indicated by the state.
		/// </summary>
		void EnforceState()
		{
			State currentState = mStateMachine.GetState();

			switch (currentState)
			{
				case State.ChargeAtPlayer:
					WalkToPlayer();
					break;
				case State.Wait:
					SetWalkDirection(WalkDirection.None);
					break;
				default:
					break;
			}
		}


		/// <summary>
		/// Walk towards the player, but not off ledges
		/// </summary>
		void WalkToPlayer()
		{
			Entity nearestPlayer = EntityManager.I.GetNearestEntity(GetCentrePos(), typeof(Arnold));
			WalkDirection dir = DirectionNeededToWalkToMe(nearestPlayer.GetCentrePos());
			dir = Util.InvertDirection(dir);

			Vector2 checkSpot = Util.GetNormal(Util.WalkDirectionToCardinal(dir, GetGravityDir()));
			checkSpot *= Tile.sTILE_SIZE * 0.45f;
			checkSpot += GravityVecNorm() * (Tile.sTILE_SIZE + 3.0f);
			checkSpot += GetCentrePos();

			bool canWalk = TileManager.I.GetTile(checkSpot).IsSolid();

			if (canWalk)
			{
				SetWalkDirection(dir);
			}
			else
			{
				SetWalkDirection(WalkDirection.None);
			}
		}



		/// <summary>
		/// Call back for when time is changed.
		/// </summary>
		protected override void OnTimeChange(GameTime gameTime)
		{
			mIsAwake = TimeZoneManager.I.GetCurrentTimeZone() == 1;
			SetWalkDirection(WalkDirection.None);
			SetVelocity(Vector2.Zero);
			mPrevVelocity = Vector2.Zero;
		}



		/// <summary>
		/// Kill if awake
		/// </summary>
		protected override bool ShouldKill()
		{
			return mIsAwake;
		}
		#endregion rUpdate





		#region rDraw

		public override Texture2D GetDrawTexture()
		{
			Texture2D tex = mTexture;

			if (mIsAwake)
			{
				tex = mRunningAnimation.GetCurrentTexture();
			}

			return tex;
		}

		#endregion rDraw
	}
}
