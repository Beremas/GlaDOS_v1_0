using GlaDOS_v1_0.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlaDOS_v1_0.Helper
{
	public static class SpeechCultureExtensions
	{
		public static string ToCultureCode(this SpeechCulture culture)
		{
			return culture switch
			{
				SpeechCulture.EnUS => "en-US",
				SpeechCulture.EnGB => "en-GB",
				SpeechCulture.EnAU => "en-AU",
				SpeechCulture.EnIN => "en-IN",
				SpeechCulture.EnCA => "en-CA",
				SpeechCulture.EnNZ => "en-NZ",

				SpeechCulture.ItIT => "it-IT",

				SpeechCulture.FrFR => "fr-FR",
				SpeechCulture.FrCA => "fr-CA",

				SpeechCulture.DeDE => "de-DE",

				SpeechCulture.EsES => "es-ES",
				SpeechCulture.EsMX => "es-MX",

				SpeechCulture.PtPT => "pt-PT",
				SpeechCulture.PtBR => "pt-BR",

				SpeechCulture.ZhCN => "zh-CN",
				SpeechCulture.ZhTW => "zh-TW",

				SpeechCulture.JaJP => "ja-JP",
				SpeechCulture.KoKR => "ko-KR",

				SpeechCulture.RuRU => "ru-RU",

				SpeechCulture.ArSA => "ar-SA",

				_ => "en-US"
			};
		}
	}

}
