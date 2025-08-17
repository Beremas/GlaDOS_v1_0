using GlaDOS_v1_0.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GlaDOS_v1_0.Service_TTS.Glados
{
	public class TextToSpeechService
	{
		private readonly HttpClient _httpClient;
		private readonly string _url;
		public HttpStatusCode serviceStatusCode;
		public string error;

		public TextToSpeechService()
		{
			_httpClient = new();
			_url = "https://glados.c-net.org/generate";
		}

		public async Task GenerateGladosVoiceAsync(string text)
		{
			try
			{
				var serviceCheck = await IsGladosApiReachable();
				if (serviceCheck == null) return;
				if (!serviceCheck.IsSuccessStatusCode)
				{
					serviceStatusCode = serviceCheck.StatusCode;
					error = serviceCheck.ReasonPhrase ?? "";
					return;
				}

				string encodedText = HttpUtility.UrlEncode(text);

				using var response = await _httpClient.GetAsync($"{_url}?text={encodedText}");
				response.EnsureSuccessStatusCode();

				using var stream = await response.Content.ReadAsStreamAsync();
				using var memoryStream = new MemoryStream();
				await stream.CopyToAsync(memoryStream);
				memoryStream.Position = 0;

				using var player = new SoundPlayer(memoryStream);
				player.PlaySync(); // Use PlaySync to block until it finishes, or Play() to play in background
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
			}
		}

		private async Task<HttpResponseMessage?> IsGladosApiReachable()
		{
			try
			{
				using var testRequest = new HttpRequestMessage(HttpMethod.Head, _url);
				using var response = await _httpClient.SendAsync(testRequest);
				return response;
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
				return null;
			}
		}
	}
}
