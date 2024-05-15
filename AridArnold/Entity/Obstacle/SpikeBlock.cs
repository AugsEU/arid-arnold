namespace AridArnold
{
	/// <summary>
	/// Spike ball that patrols and kills the player
	/// </summary>
	internal class SpikeBlock : Entity
	{
		#region rConstants

		const float HITBOX_REDUCTION = 2.0f;

		#endregion rConstants





		#region rMembers

		Animator mSpikeAnimation;
		RailTraveller mRail;

		#endregion rMembers


		#region rInitialisation

		/// <summary>
		/// Create spike block
		/// </summary>
		public SpikeBlock(RailTraveller rail) : base(rail.GetPosition())
		{
			mRail = rail;
		}



		/// <summary>
		/// Load content for the spike block
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/SawBlock1");
			mSpikeAnimation = new Animator(Animator.PlayType.Repeat,
											("Tiles/SawBlock1", 0.1f),
											("Tiles/SawBlock2", 0.1f));
			mSpikeAnimation.Play();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update the spike block
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mRail.Update(gameTime);
			mSpikeAnimation.Update(gameTime);
			mPosition = mRail.GetPosition();

			base.Update(gameTime);
		}



		/// <summary>
		/// Collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			Vector2 min = mPosition;
			min.X += HITBOX_REDUCTION;
			min.Y += HITBOX_REDUCTION;

			Vector2 max = mPosition;
			max.X += mTexture.Width - HITBOX_REDUCTION;
			max.Y += mTexture.Height - HITBOX_REDUCTION;

			return new Rect2f(min, max);
		}


		/// <summary>
		/// Kill player on contact with spikes
		/// </summary>
		public override void OnCollideEntity(Entity entity)
		{
			if (entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				entity.Kill();
			}

			base.OnCollideEntity(entity);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the spike block
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mSpikeAnimation.GetCurrentTexture(), mPosition, DrawLayer.Tile);

			mRail.Draw(info, new Vector2(8.0f, 8.0f));
		}

		#endregion rDraw
	}
}
