using AridArnold.Tiles.Basic;

namespace AridArnold
{
	abstract class ElectricButtonBase : InteractableTile
	{
		#region rMembers

		protected Texture2D mDownTexture;
		protected bool mIsPressed;
		protected bool mWasPressed;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct button at position
		/// </summary>
		public ElectricButtonBase(CardinalDirection rot, Vector2 position) : base(position)
		{
			mRotation = rot;
			mIsPressed = false;
			mWasPressed = false;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Entity intersect
		/// </summary>
		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is PlatformingEntity && entity is not ProjectileEntity)
			{
				mIsPressed = true;
			}
		}



		/// <summary>
		/// Update button
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mWasPressed = mIsPressed;

			float elec = mIsPressed ? 4.0f : -2.0f;
			TileManager.I.GetEMField().SetElectricity(mTileMapIndex, elec);

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get button texture.
		/// </summary>
		public override Texture2D GetTexture()
		{
			if (mWasPressed)
			{
				return mDownTexture;
			}

			return mTexture;
		}

		#endregion rDraw
	}





	/// <summary>
	/// Button that turns on electricity.
	/// </summary>
	internal class ElectricButton : ElectricButtonBase
	{
		public ElectricButton(CardinalDirection rot, Vector2 position) : base(rot, position)
		{
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/ButtonUp");
			mDownTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/ButtonDown");

			TileManager.I.GetEMField().RegisterConductive(mTileMapIndex);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			mIsPressed = false;
		}
	}





	/// <summary>
	/// Button that turns on electricity and stays down forever.
	/// </summary>
	internal class PermElectricButton : ElectricButtonBase
	{
		public PermElectricButton(CardinalDirection rot, Vector2 position) : base(rot, position)
		{
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/PermaUp");
			mDownTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/PermaDown");

			TileManager.I.GetEMField().RegisterConductive(mTileMapIndex);
		}
	}
}
