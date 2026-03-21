using QMatrix.GUI.Models;

namespace QMatrix.GUI.Services;

public interface IQMatrixApiService
{
    Task<List<QMHistoryItem>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task<QMMessage[]> GetConversationAsync(string conversationId, CancellationToken cancellationToken = default);
    Task SendMessageAsync(string conversationId, string message, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> StreamResponseAsync(string conversationId, string message, CancellationToken cancellationToken = default);
    Task<QMAgentStep> GetAgentProgressAsync(string conversationId, CancellationToken cancellationToken = default);
}
