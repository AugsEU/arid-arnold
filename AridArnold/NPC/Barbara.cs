namespace AridArnold
{
	internal class Barbara : NPC
	{
		#region rInitialisation

		/// <summary>
		/// Put Barbara at a position
		/// </summary>
		public Barbara(Vector2 pos) : base(pos)
		{
		}



		/// <summary>
		/// Load barbara textures.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mIdleTexture = content.Load<Texture2D>("NPC/Barbara/Idle1");
			mTalkTexture = content.Load<Texture2D>("NPC/Barbara/Talk1");
			mAngryTexture = content.Load<Texture2D>("NPC/Barbara/Angry1");
		}

		#endregion rInitialisation


		#region rUpdate

		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mIdleTexture);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw Barbara
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			DrawTalking(info);
		}

		#endregion rDraw
	}
}
