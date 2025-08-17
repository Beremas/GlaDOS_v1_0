using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlaDOS_v1_0.Enums
{
	[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public enum Personality
	{
		Neutral,
		Glados,
		Chappy
	}
}
