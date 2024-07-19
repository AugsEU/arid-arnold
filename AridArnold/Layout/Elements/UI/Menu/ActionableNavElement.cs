


namespace AridArnold
{
	/// <summary>
	/// Layout element that we can navigate around with D-pad(for menus) and interact with by clicking or hitten the enter button.
	/// </summary>
	abstract class ActionableNavElement : NavElement
	{
		protected Vector2 mSize;

		protected ActionableNavElement(XmlNode rootNode, Layout parent) : base(rootNode, parent)
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
				SetSelected(this);
			}

			base.Update(gameTime);
		}

		public abstract void DoAction();
	}
}
