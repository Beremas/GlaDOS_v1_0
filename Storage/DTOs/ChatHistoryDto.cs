using LLama.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GlaDOS_v1_0.Storage.DTOs
{
	public class ChatMessageDto
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public AuthorRole Role { get; set; }
		public required string Content { get; set; }
		public DateTime Timestamp { get; set; }
	}

	public class ChatHistoryDto
	{
		public List<ChatMessageDto> Messages { get; set; } = new();
	}
}
