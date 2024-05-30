
namespace AridArnold
{
	class ArcadeBuilding : Entity
	{
		Animator mInteriorAnim;
		Animator mSignAnim;

		public ArcadeBuilding(Vector2 pos) : base(pos)
		{
		}


		public override void LoadContent()
		{
		}

		public override void Draw(DrawInfo info)
		{
			throw new NotImplementedException();
		}
	}
}
