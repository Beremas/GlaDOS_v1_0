using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlaDOS_v1_0.Helper
{
	public static class TextExtractionHelper
	{
		public static string ExtractAfter(string input, string keyword)
		{
			var index = input.IndexOf(keyword);
			return index >= 0 ? input[(index + keyword.Length)..].Trim('.') : "";
		}

		public static string ExtractBetween(string input, string start, string end)
		{
			var startIndex = input.IndexOf(start);
			if (startIndex == -1) return "";
			startIndex += start.Length;
			var endIndex = input.IndexOf(end, startIndex);
			return endIndex == -1 ? "" : input[startIndex..endIndex].Trim();
		}
	}
}
