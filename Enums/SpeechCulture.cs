
using Newtonsoft.Json;

namespace GlaDOS_v1_0.Enums
{
	[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public enum SpeechCulture
	{
		EnUS,   // English (United States)
		EnGB,   // English (United Kingdom)
		EnAU,   // English (Australia)
		EnIN,   // English (India)
		EnCA,   // English (Canada)
		EnNZ,   // English (New Zealand)

		ItIT,   // Italian (Italy)

		FrFR,   // French (France)
		FrCA,   // French (Canada)

		DeDE,   // German (Germany)

		EsES,   // Spanish (Spain)
		EsMX,   // Spanish (Mexico)

		PtPT,   // Portuguese (Portugal)
		PtBR,   // Portuguese (Brazil)

		ZhCN,   // Chinese (Simplified, China)
		ZhTW,   // Chinese (Traditional, Taiwan)

		JaJP,   // Japanese (Japan)

		KoKR,   // Korean (Korea)

		RuRU,   // Russian (Russia)

		ArSA    // Arabic (Saudi Arabia)
	}

}
