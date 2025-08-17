using GlaDOS_v1_0.Enums;
using GlaDOS_v1_0.Helper;
using GlaDOS_v1_0.Logs;
using System.Globalization;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace GlaDOS_v1_0.Service_TTS.Microsoft
{
	public class TextToSpeechService
	{
		SpeechSynthesizer _synth;
		string _culture;

		public TextToSpeechService(SpeechCulture culture)
		{
			_synth = new SpeechSynthesizer();
			_culture = SpeechCultureExtensions.ToCultureCode(culture);
		}

		public void SpeakAsync(string message)
		{
			try
			{
				_synth.SetOutputToDefaultAudioDevice();
				Prompt prompt;
				if (_culture == SpeechCultureExtensions.ToCultureCode(SpeechCulture.EnUS))
				{
					_synth.SelectVoice("Microsoft Zira Desktop");
				}
				else
				{
					var voice = _synth.GetInstalledVoices().FirstOrDefault(v => v.VoiceInfo.Culture.Name.Equals(_culture, StringComparison.OrdinalIgnoreCase));
					if (voice != null)
					{
						_synth.SelectVoice(voice?.VoiceInfo.Name);
					}
					else
					{
						_synth.SelectVoice("Microsoft Zira Desktop");
					}
				}

				_synth.Rate = 1;
				_synth.Volume = 100;
				prompt = _synth.SpeakAsync(message);


				while (!prompt.IsCompleted)
				{
					Thread.Sleep(500);
				}
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
			}
		}
	}
}



