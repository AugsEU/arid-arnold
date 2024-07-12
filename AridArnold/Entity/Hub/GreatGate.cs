namespace AridArnold
{
	/// <summary>
	/// The big gate that blocks the fountain.
	/// </summary>
	internal class GreatGate : Entity
	{
		#region rConstants

		const double UNLOCK_TIME = 2500.0;
		static Vector2 BACK_OFFSET = new Vector2(0.0f, 25.0f);
		static Vector2 FRONT_OFFSET = new Vector2(16.0f, 0.0f);
		static Vector2 OPENING_OFFSET = new Vector2(24.0f, 33.0f);
		static Vector2 OPENING_OPEN_OFFSET = new Vector2(24.0f, 79.0f);

		static Vector2 BUBBLE_OFFSET = new Vector2(-24.0f, 50.0f);

		#endregion rConstants





		#region rMembers

		Texture2D mFrontLayer;
		Texture2D mBackLayer;
		Texture2D mOpening;

		PercentageTimer mUnlockDoorTimer;
		bool mIsUnlocked;

		TextInfoBubble mUseKeyInfoBubble;
		TextInfoBubble mGetKeyInfoBubble;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create great gate at position.
		/// </summary>
		public GreatGate(Vector2 pos) : base(pos)
		{
			mUnlockDoorTimer = new PercentageTimer(UNLOCK_TIME);
			mIsUnlocked = FlagsManager.I.CheckFlag(FlagCategory.kUnlockedGreatGate);
			mPosition.X += 2.0f; // Align to grid

			mUseKeyInfoBubble = new TextInfoBubble(mPosition + BUBBLE_OFFSET, "InGame.UseKey");
			mGetKeyInfoBubble = new TextInfoBubble(mPosition + BUBBLE_OFFSET, "InGame.GetKey");
		}

		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			mBackLayer = MonoData.I.MonoGameLoad<Texture2D>("GreatGate/GateBack");
			mFrontLayer = MonoData.I.MonoGameLoad<Texture2D>("GreatGate/GateFront");
			mOpening = MonoData.I.MonoGameLoad<Texture2D>("GreatGate/Opening");
			mTexture = mFrontLayer;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Upadte door.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			bool hasGreatKey = FlagsManager.I.CheckFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kGatewayKey);
			bool openFlag = FlagsManager.I.CheckFlag(FlagCategory.kUnlockedGreatGate);

			// Add default collider
			Rect2f collision = ColliderBounds();
			collision.min.X += 25.0f;

			if (mIsUnlocked)
			{
				// Raise collision to open up gate
				collision.max.Y -= 47.0f;
			}
			else
			{
				if(openFlag && !mUnlockDoorTimer.IsPlaying())
				{
					mUnlockDoorTimer.FullReset();
					mUnlockDoorTimer.Start();
				}
			}



			RectangleColliderSubmission submission = new RectangleColliderSubmission(collision);
			EntityManager.I.AddColliderSubmission(submission);

			if (mUnlockDoorTimer.IsPlaying() && mUnlockDoorTimer.GetPercentageF() >= 1.0f)
			{
				if(openFlag)
					mIsUnlocked = openFlag;
			}

			mUseKeyInfoBubble.Update(gameTime, IsPlayerNear() && hasGreatKey && !mIsUnlocked && !mUnlockDoorTimer.IsPlaying());
			mGetKeyInfoBubble.Update(gameTime, IsPlayerNear() && !hasGreatKey && !mIsUnlocked);

			base.Update(gameTime);
		}


		protected override void OnPlayerInteract()
		{
			bool hasGreatKey = FlagsManager.I.CheckFlag(FlagCategory.kKeyItems, (UInt32)KeyItemFlagType.kGatewayKey);
			if (hasGreatKey)
			{
				FlagsManager.I.SetFlag(FlagCategory.kUnlockedGreatGate, true);
			}
			base.OnPlayerInteract();
		}



		/// <summary>
		/// Collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, 62.0f, 80.0f);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the lock
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mBackLayer, mPosition + BACK_OFFSET, DrawLayer.Default);
			MonoDraw.DrawTextureDepth(info, mFrontLayer, mPosition + FRONT_OFFSET, DrawLayer.Tile);
			MonoDraw.DrawTextureDepth(info, mOpening, GetOpeningPosition(), DrawLayer.Default);

			mGetKeyInfoBubble.Draw(info);
			mUseKeyInfoBubble.Draw(info);
		}


		Vector2 GetOpeningPosition()
		{
			Vector2 basePos = mPosition + OPENING_OFFSET;
			Vector2 destPos = mPosition + OPENING_OPEN_OFFSET;

			float t = mIsUnlocked ? 1.0f : mUnlockDoorTimer.GetPercentageF();

			return MonoMath.Lerp(basePos, destPos, t);
		}

		#endregion rDraw
	}
}
