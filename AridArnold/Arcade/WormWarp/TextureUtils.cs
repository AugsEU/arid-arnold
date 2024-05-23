using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WormWarp
{
	public static class TextureUtils
	{
		public static Color[] GetPixels(Texture2D texture)
		{
			Color[] colors1D = new Color[texture.Width * texture.Height];
			texture.GetData<Color>(colors1D);
			return colors1D;
		}

		public static Color[,] GetColourArray(Texture2D texture)
		{
			Color[] colors1D = GetPixels(texture);
			Color[,] colors2D = new Color[texture.Width, texture.Height];
			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					colors2D[x, y] = colors1D[x + y * texture.Width];
				}
			}
			return colors2D;
		}
	}
}
