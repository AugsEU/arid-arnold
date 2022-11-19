namespace AridArnold
{
	internal class LabWall : AnimWallTile
	{
		/// <summary>
		/// Platform tile constructor
		/// </summary>
		/// <param name="rotation"></param>
		public LabWall(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mAnimation = new Animator(content, Animator.PlayType.Repeat,
											("Tiles/Lab/lab-wall0", 0.454f),
											("Tiles/Lab/lab-wall1", 0.392f),
											("Tiles/Lab/lab-wall2", 0.421f),
											("Tiles/Lab/lab-wall3", 0.478f));
			mAnimation.Play(RandomManager.I.GetDraw().GetUnitFloat());
		}
	}
}
