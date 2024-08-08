using System.Reflection;

namespace AridArnold
{
	static class MonoEnum
	{
		/// <summary>
		/// Get an enum from a string
		/// </summary>
		static public T GetEnumFromString<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}



		/// <summary>
		/// Get number of enums
		/// </summary>
		static public int EnumLength(Type enumType)
		{
			return Enum.GetNames(enumType).Length;
		}



		/// <summary>
		/// Get file path attribute
		/// </summary>
		public static string GetFilePath(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());

			FilePathAttribute attribute = field.GetCustomAttribute<FilePathAttribute>();

			return attribute?.mPath;
		}
	}

	/// <summary>
	/// File path attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	sealed class FilePathAttribute : Attribute
	{
		public readonly string mPath;

		public FilePathAttribute(string path)
		{
			mPath = path;
		}
	}
}
