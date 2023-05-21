namespace AridArnold
{
	internal class GravityOrb : Entity
	{
		#region rConstants

		const float ANGULAR_SPEED = 0.2f;
		const float AMPLITUDE = 1.0f;

		#endregion rConstants




		#region rMembers

		static GravityOrb sActiveOrb = null;
		Texture2D mActiveTexture;
		Texture2D mInactiveTexture;
		Vector2 mPushDisplacement;
		Vector2 mPushVelocity;
		Vector2 mFloatDisplacement;
		float mFloatAngle;

		CardinalDirection mGravityDir;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create orb at position
		/// </summary>
		public GravityOrb(Vector2 pos, CardinalDirection gravityDir) : base(pos)
		{
			mPosition.X += 3.0f;
			mPosition.Y += 3.0f;

			mFloatAngle = 0.0f;

			mGravityDir = gravityDir;
		}



		/// <summary>
		/// Load orb content
		/// </summary>
		public override void LoadContent()
		{
			switch (mGravityDir)
			{
				case CardinalDirection.Up:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbUpActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbUp");
					break;
				case CardinalDirection.Right:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbRightActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbRight");
					break;
				case CardinalDirection.Down:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbDownActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbDown");
					break;
				case CardinalDirection.Left:
					mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbLeftActive");
					mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbLeft");
					break;
				default:
					break;
			}
			
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("GravityOrb/OrbBase");
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update orb
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mFloatAngle += dt * ANGULAR_SPEED;
			mFloatDisplacement.Y = MathF.Sin(mFloatAngle) * AMPLITUDE;

			mPushVelocity -= (mPushDisplacement) * dt;
			mPushVelocity *= 0.99f;
			mPushDisplacement += dt * mPushVelocity;
		}



		/// <summary>
		/// Handle collision
		/// </summary>
		/// <param name="entity"></param>
		public override void OnCollideEntity(Entity entity)
		{
			if(entity is not Arnold)
			{
				return;
			}

			Arnold arnold = (Arnold)entity;
			EntityPush(arnold);

			SetActive();
		}


		/// <summary>
		/// Orb collider
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mTexture);
		}

		#endregion rUpadate





		#region rDraw

		/// <summary>
		/// Draw the orb
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Texture2D tex = IsActive() ? mActiveTexture : mInactiveTexture;
			Vector2 pos = mPosition + mFloatDisplacement + mPushDisplacement;

			MonoDraw.DrawTextureDepth(info, tex, pos, DrawLayer.Tile);
		}

		#endregion rDraw





		#region rUtility

		void SetActive()
		{
			sActiveOrb = this;
		}

		bool IsActive()
		{
			return object.ReferenceEquals(this, sActiveOrb);
		}

		void EntityPush(MovingEntity entity)
		{
			Vector2 newVel = entity.GetVelocity();

			if(newVel.LengthSquared() < mPushVelocity.LengthSquared())
			{
				return;
			}

			mPushVelocity = newVel;


			mPushVelocity.X = MonoMath.ClampAbs(mPushVelocity.X, 5.0f);
			mPushVelocity.Y = MonoMath.ClampAbs(mPushVelocity.Y, 5.0f);
		}

		#endregion rUtility





		#region rStatic

		public static void ResetActive()
		{
			sActiveOrb = null;
		}

		#endregion rStatic
	}
}
