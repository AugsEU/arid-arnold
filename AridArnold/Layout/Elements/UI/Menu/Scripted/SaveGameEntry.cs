using System.Diagnostics;

namespace AridArnold
{
	class SaveGameEntry : MenuButton
	{
		ProfileSaveInfo mSaveFileRef;

		public SaveGameEntry(string id, Vector2 pos, Vector2 size, string fontID, Layout parent, ProfileSaveInfo saveFileRef) : base(id, pos, size, fontID, parent)
		{
			mSaveFileRef = saveFileRef;
		}

		public override void DoAction()
		{
			SaveManager.I.LoadGame(mSaveFileRef);
		}

		public override void Draw(DrawInfo info)
		{
			Rect2f area = GetRect2f();

			Vector2 namePosition = area.min + mSize * 0.5f;
			Vector2 timePosition = namePosition;

			namePosition.X -= area.Width * 0.1666666f;
			timePosition.X += area.Width * 0.1666666f;

			string nameStr = mSaveFileRef.GetProfileName();
			string saveTimeStr = mSaveFileRef.GetSaveTimeStr();

			MonoDraw.DrawStringCentredShadow(info, mFont, namePosition, GetButtonColor(), nameStr, 2.0f, GetDepth());
			MonoDraw.DrawStringCentredShadow(info, mFont, timePosition, GetButtonColor(), saveTimeStr, 2.0f, GetDepth());
		}
	}
}
