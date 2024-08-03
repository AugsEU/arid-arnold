namespace AridArnold
{
	internal class Fountain : Entity
	{
		#region rConstants

		static Vector2 WATER_OFFSET = new Vector2(16.0f, -24.0f);
		static Vector2 BUBBLE_OFFSET = new Vector2(47.0f, -30.0f);

		#endregion rConstants





		#region rMembers

		Animator mWaterAnim;
		TextInfoBubble mUsePrompt;

		#endregion rMembers




		#region rInit

		/// <summary>
		/// Create time machine at pos(bottom left)
		/// </summary>
		public Fountain(Vector2 pos) : base(pos)
		{
			mPosition += new Vector2(2.0f, 2.0f);
			mUsePrompt = new TextInfoBubble(GetPos() + BUBBLE_OFFSET, "InGame.Bathe");
		}



		/// <summary>
		/// Load content for time machine
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Fountain/Base");
			mWaterAnim = MonoData.I.LoadAnimator("Fountain/Water.max");
			mWaterAnim.Play();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mUsePrompt.Update(gameTime, IsPlayerNear());
			mWaterAnim.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// When player presses enter
		/// </summary>
		protected override void OnPlayerInteract()
		{
			// End game...
			EventManager.I.TriggerEvent(EventType.ShopDoorOpen);
			base.OnPlayerInteract();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the time machine
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.SubEntity);
			MonoDraw.DrawTextureDepth(info, mWaterAnim.GetCurrentTexture(), mPosition + WATER_OFFSET, DrawLayer.SubEntity);

			mUsePrompt.Draw(info);
		}

		#endregion rDraw
	}
}
