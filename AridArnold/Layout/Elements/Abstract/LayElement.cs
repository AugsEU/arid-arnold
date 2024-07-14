namespace AridArnold
{
	/// <summary>
	/// Element in our layout
	/// </summary>
	abstract class LayElement
	{
		Layout mParent;
		string mID;
		Vector2 mPos;
		DrawLayer mDepth;
		Color mColor;
		float mScale;
		bool mVisible;

		public LayElement(XmlNode rootNode, Layout parent)
		{
			mPos = MonoParse.GetVector(rootNode);
			mDepth = MonoParse.GetDrawLayer(rootNode["depth"]);
			mScale = MonoParse.GetFloat(rootNode["scale"], 1.0f);
			mColor = MonoParse.GetColor(rootNode["color"], Color.White);

			mID = MonoParse.GetStringAttrib(rootNode, "id", "Null ID");

			mVisible = true;
			
			mParent = parent;
		}

		public virtual void Update(GameTime gameTime) { }

		public virtual void Draw(DrawInfo info) { }

		public DrawLayer GetDepth() { return mDepth; }

		public float GetScale() { return mScale; }

		public Vector2 GetPosition() { return mPos; }

		public Color GetColor() { return mColor; }

		public bool IsVisible() { return mVisible; }

		public void SetVisible(bool visible) { mVisible = visible; }

		public Layout GetParent() { return mParent; }

		public string GetID() { return mID; }
	}
}
