using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
{
	internal class LabPlatform : AnimPlatformTile
	{
		/// <summary>
		/// Platform tile constructor
		/// </summary>
		/// <param name="rotation"></param>
		public LabPlatform(CardinalDirection rotation, Vector2 position) : base(rotation, position)
		{
			mRotation = rotation;
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mAnimation = new Animator(content, Animator.PlayType.Repeat,
											("Tiles/Lab/lab-plat0", 0.35f),
											("Tiles/Lab/lab-plat1", 0.3f),
											("Tiles/Lab/lab-plat2", 0.35f),
											("Tiles/Lab/lab-plat3", 0.35f));
			mAnimation.Play(RandomManager.I.GetDraw().GetUnitFloat());
		}
	}
}
