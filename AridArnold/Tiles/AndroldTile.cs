using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
{
	/// <summary>
	/// Tile that spawns an Androld when powered by electricity.
	/// </summary>
	internal class AndroldTile : SquareTile
	{
		#region rMembers

		bool mActivated;
		Animator mActivationAnim;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Initialise Androld at position
		/// </summary>
		public AndroldTile(Vector2 pos) : base(pos)
		{
			mActivated = false;
		}



		/// <summary>
		/// Load Androld content.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/Lab/androld-tile0");
			mActivationAnim = new Animator(content, Animator.PlayType.OneShot,
											("Tiles/Lab/androld-tile1", 0.5f),
											("Tiles/Lab/androld-tile2", 0.5f));
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Check for electricity to activate.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if (mActivated)
			{
				mActivationAnim.Update(gameTime);

				//Animation is finished, spawn Androld
				if(mActivationAnim.IsPlaying() == false)
				{
					Vector2 position = TileManager.I.RoundToTileCentre(mTileMapIndex);

					// To do: Don't load content here!
					EntityManager.I.RegisterEntity(new Androld(position), Main.GetMainContentManager());

					//Self delete.
					TileManager.I.RequestDelete(mTileMapIndex);
				}
			}
			else
			{
				EMField.ScanResults scan = TileManager.I.GetEMField().ScanAdjacent(mTileMapIndex);

				if (scan.mTotalElectric > 0.75f)
				{
					mActivated = true;
					mActivationAnim.Play();
				}
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw activation animation or static animation.
		/// </summary>
		public override Texture2D GetTexture()
		{
			if(mActivated)
			{
				return mActivationAnim.GetCurrentTexture();
			}
			
			return mTexture;
		}

		#endregion rDraw
	}
}
