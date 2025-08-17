

namespace GlaDOS_v1_0.Service_Camera.EmotionRecognition
{
	public static class EmotionHandler
	{
		private static Dictionary<string, DateTime> _lastEmotionSentCooldown = new();

		public static bool ShouldSendEmotion(string emotion)
		{
			if (!emotion.Equals("neutral"))
			{
				if (!_lastEmotionSentCooldown.ContainsKey(emotion))
					return true;

				var lastSent = _lastEmotionSentCooldown[emotion];
				var now = DateTime.UtcNow;

				return (now - lastSent) > TimeSpan.FromMinutes(15);
			}
			return false;		
		}

		public static void TimeoutEmotion(string emotion)
		{
			_lastEmotionSentCooldown[emotion] = DateTime.UtcNow;
		}
		
	}

}
