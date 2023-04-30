namespace AridArnold
{
	class SequenceDoor : Entity
	{
		#region rConstant

		const float HITBOX_WIDTH = 16.0f;
		const float HITBOX_HEIGHT = 16.0f;

		#endregion rConstant





		#region rMembers

		Texture2D mOpenTexture;
		Texture2D mClosedTexture;
		Texture2D[] mNumberTextures;
		
		List<int> mLevelIDSequence;

		#endregion rMembers





		#region rInitialisation

		public SequenceDoor(Vector2 pos, int[] levelSequence) : base(pos)
		{
			mLevelIDSequence = new List<int>();
			for (int i = 0; i < levelSequence.Length; i++)
			{
				if(levelSequence[i] != 0)
				{
					mLevelIDSequence.Add(i);
				}
			}

			MonoDebug.Assert(mLevelIDSequence.Count > 0 && mLevelIDSequence.Count < 10);
		}

		public override void LoadContent()
		{
			mOpenTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorOpen");
			mClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorClosed");

			mNumberTextures = new Texture2D[10];
			mNumberTextures[0] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorZero");
			mNumberTextures[1] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorOne");
			mNumberTextures[2] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorTwo");
			mNumberTextures[3] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorThree");
			mNumberTextures[4] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorFour");
			mNumberTextures[5] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorFive");
			mNumberTextures[6] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorSix");
			mNumberTextures[7] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorSeven");
			mNumberTextures[8] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorEight");
			mNumberTextures[9] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorNine");

			mTexture = mClosedTexture;
		}

		#endregion rInitialisation





		#region rUpdate

		public override void Update(GameTime gameTime)
		{


			base.Update(gameTime);
		}

		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, HITBOX_WIDTH, HITBOX_HEIGHT);
		}

		#endregion rUpdate




		#region rDraw

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mClosedTexture, mPosition, DrawLayer.Default);
			MonoDraw.DrawTextureDepth(info, mNumberTextures[mLevelIDSequence.Count], mPosition, DrawLayer.Default);
		}

		#endregion rDraw
	}
}
