using GlaDOS_v1_0.Storage.DTOs;

namespace GlaDOS_v1_0.Storage.Repositories
{
	public interface IChatHistoryRepository
	{
		ChatHistoryDto Load();
		void Save(ChatHistoryDto history);
	}
}
