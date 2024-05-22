namespace GMTK2023
{
	/// <summary>
	/// Color related utilities
	/// </summary>
	static class MonoColor
	{
		/// <summary>
		/// Get uint hex code from color.
		/// </summary>
		public static uint ColorToHEX(Color color)
		{
			return color.B + ((uint)(color.G) << 8) + +((uint)(color.R) << 16);
		}



		/// <summary>
		/// Get color from string hex code.
		/// </summary>
		public static Color HEXToColor(string hex)
		{
			byte r, g, b, a;

			r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			a = 255;

			return new Color(r, g, b, a);
		}



		/// <summary>
		/// Brighten colour linearly
		/// </summary>
		/// <param name="col">Initial colour, output value</param>
		/// <param name="bright">Brightness factor from 0 to 1</param>
		public static void BrightenColour(ref Color col, float bright)
		{
			col.R = (byte)((col.R * (1 - bright)) + (255 * bright));
			col.G = (byte)((col.G * (1 - bright)) + (255 * bright));
			col.B = (byte)((col.B * (1 - bright)) + (255 * bright));
		}


		/// <summary>
		/// Brighten colour linearly
		/// </summary>
		/// <param name="col">Initial colour, output value</param>
		/// <param name="bright">Brightness factor from 0 to 1</param>
		public static void DarkenColour(ref Color col, float bright)
		{
			col.R = (byte)((col.R * bright));
			col.G = (byte)((col.G * bright));
			col.B = (byte)((col.B * bright));
		}
	}
}
