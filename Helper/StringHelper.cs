namespace GlaDOS_v1_0.Helper
{
	public static class StringHelper
	{
		public static string CapitalizeFirstLetter(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			return char.ToUpper(input[0]) + input.Substring(1);
		}

		public static void CapitalizeAllStringProperties(object obj)
		{
			var stringProps = obj.GetType().GetProperties()
				.Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

			foreach (var prop in stringProps)
			{
				var value = (string?)prop.GetValue(obj);
				if (!string.IsNullOrWhiteSpace(value))
				{
					prop.SetValue(obj, char.ToUpper(value[0]) + value.Substring(1));
				}
			}
		}
	}
}
