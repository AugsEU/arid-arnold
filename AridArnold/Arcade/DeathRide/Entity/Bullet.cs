namespace GMTK2023
{
	internal class Bullet : Entity
	{
		const float BULLET_SPEED = 30.0f;

		AITeam mTeam;
		Vector2 mVelocity;

		public Bullet(Vector2 pos, EightDirection dir, AITeam team) : base(pos)
		{
			float angle = Util.GetAngleFromDirection(dir);
			mVelocity = new Vector2(BULLET_SPEED, 0.0f);
			mVelocity = MonoMath.Rotate(mVelocity, -angle);
			mTeam = team;

			switch (dir)
			{
				case EightDirection.UpLeft:
					mPosition.X += 4.0f;
					mPosition.Y += 2.0f;
					break;
				case EightDirection.Up:
					mPosition.X += 20.0f;
					mPosition.Y -= 8.0f;
					break;
				case EightDirection.UpRight:
					mPosition.X += 32.0f;
					mPosition.Y += 3.0f;
					break;
				case EightDirection.Left:
					mPosition.X -= 8.0f;
					mPosition.Y += 11.0f;
					break;
				case EightDirection.Right:
					mPosition.X += 32.0f;
					mPosition.Y += 11.0f;
					break;
				case EightDirection.DownLeft:
					mPosition.X -= 8.0f;
					mPosition.Y += 11.0f;
					break;
				case EightDirection.Down:
					mPosition.X += 4.0f;
					mPosition.Y += 32.0f;
					break;
				case EightDirection.DownRight:
					mPosition.X += 29.0f;
					mPosition.Y += 11.0f;
					break;
				default:
					break;
			}
		}

		public override void LoadContent()
		{
			switch (mTeam)
			{
				case AITeam.Ally:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Bullet/BulletAlly");
					break;
				case AITeam.Enemy:
					mTexture = MonoData.I.MonoGameLoad<Texture2D>("Bullet/BulletEnemy");
					break;
			}
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			mPosition += mVelocity * dt;

			if (mPosition.X < mTexture.Width || mPosition.X > Screen.SCREEN_WIDTH + mTexture.Width
				|| mPosition.Y < mTexture.Height || mPosition.Y > Screen.SCREEN_HEIGHT + mTexture.Height)
			{
				EntityManager.I.QueueDeleteEntity(this);
			}

			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.Bullets);
		}

		public override void OnCollideEntity(Entity entity)
		{
			bool hit = false;

			float dist = (entity.GetCentrePos() - mCentreOfMass).Length();

			if (dist < 12.0f)
			{
				if (entity is AIEntity)
				{
					AIEntity aiEntity = (AIEntity)entity;

					if (aiEntity.GetTeam() == AITeam.Ally && mTeam == AITeam.Enemy)
					{
						aiEntity.SetTeam(AITeam.Enemy);
						hit = true;
					}
					else if (aiEntity.GetTeam() == AITeam.Enemy && mTeam == AITeam.Ally)
					{
						aiEntity.Kill();
						hit = true;
					}
				}
				else if (entity is Player && mTeam != AITeam.Ally)
				{
					Player player = (Player)entity;

					player.AddHealth(-1);
					hit = true;
				}

				if (hit == true)
				{
					SetEnabled(false);
					EntityManager.I.QueueDeleteEntity(this);
				}
			}

			base.OnCollideEntity(entity);
		}
	}
}
