namespace AridArnold
{
	/// <summary>
	/// Layout element that we can navigate around with D-pad(for menus) and has a hitbox area.
	/// </summary>
	abstract class HitBoxNavElement : NavElement
	{
		protected Vector2 mSize;

		protected HitBoxNavElement(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mSize = new Vector2();
			mSize.X = MonoParse.GetFloat(rootNode["width"]);
			mSize.Y = MonoParse.GetFloat(rootNode["height"]);
		}

		public override void Update(GameTime gameTime)
		{
			Rect2f rect = new Rect2f(GetPosition(), mSize.X, mSize.Y);
			if(InputManager.I.MouseInRect(rect))
			{
				GetParent().SetSelectedElement(this);
			}

			base.Update(gameTime);
		}
	}
}
