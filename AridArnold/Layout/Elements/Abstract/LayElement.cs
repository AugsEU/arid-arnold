namespace AridArnold
{
	/// <summary>
	/// Element in our layout
	/// </summary>
	abstract class LayElement
	{
		protected Layout mParent;
		protected string mID;
		protected Vector2 mPos;
		protected DrawLayer mDepth;
		protected Color mColor;
		protected float mScale;
		protected bool mVisible;
		protected bool mEnabled;

		public LayElement(XmlNode rootNode, Layout parent)
		{
			mPos = MonoParse.GetVector(rootNode);
			mDepth = MonoParse.GetDrawLayer(rootNode["depth"]);
			mScale = MonoParse.GetFloat(rootNode["scale"], 1.0f);
			mColor = MonoParse.GetColor(rootNode["color"], Color.White);

			mID = MonoParse.GetStringAttrib(rootNode, "id", "Null ID");

			mVisible = true;
			mEnabled = true;

			mParent = parent;
		}

		protected LayElement(string id, Vector2 pos, Layout parent)
		{
			mID = id;
			mPos = pos;
			mParent = parent;

			mDepth = DrawLayer.Default;
			mColor = Color.White;
			mScale = 1.0f;
			mVisible = true;
		}

		public virtual void Update(GameTime gameTime) { }

		public virtual void Draw(DrawInfo info) { }

		public DrawLayer GetDepth() { return mDepth; }

		public float GetScale() { return mScale; }

		public Vector2 GetPosition() { return mPos; }

		public Color GetColor() { return mColor; }

		public bool IsVisible() { return mVisible && mEnabled; }

		public void SetVisible(bool visible) { mVisible = visible; }

		public Layout GetParent() { return mParent; }

		public string GetID() { return mID; }

		public void SetPos(Vector2 pos) { mPos = MonoMath.Round(pos); }

		public void SetEnabled(bool enabled) { mEnabled = enabled; }

		public bool IsEnabled() { return mEnabled; }
	}
}
