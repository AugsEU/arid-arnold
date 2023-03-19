using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
{
	abstract class ProjectileEntity : Entity
	{
		public ProjectileEntity(Vector2 pos) : base(pos)
		{

		}

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			Rect2f collider = ColliderBounds();
			bool shouldDelete = CheckHitSolid(ref collider) || CheckOffScreen(ref collider);

			base.Update(gameTime);

			if (shouldDelete) EntityManager.I.QueueDeleteEntity(this);
		}

		/// <summary>
		/// Returns true if we are off screen.
		/// </summary>
		bool CheckOffScreen(ref Rect2f collider)
		{
			// Check X
			if (collider.max.X < -Tile.sTILE_SIZE || collider.min.X > TileManager.I.GetDrawWidth() + Tile.sTILE_SIZE * 4.0f)
			{
				return true;
			}

			// Check Y
			if (collider.max.Y < -Tile.sTILE_SIZE || collider.min.Y > TileManager.I.GetDrawHeight() + Tile.sTILE_SIZE * 4.0f)
			{
				return true;
			}

			return false;
		}



		/// <summary>
		/// Returns true if we are touching a solid tile.
		/// </summary>
		bool CheckHitSolid(ref Rect2f collider)
		{
			return TileManager.I.DoesRectTouchTiles(collider);
		}
	}
}
