using System.Net.Http;
using System.Text;
using System.Text.Json;
using QMatrix.GUI.Models;

namespace QMatrix.GUI.Services;

public class QMatrixApiService : IQMatrixApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5000/api";

    public QMatrixApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<QMHistoryItem>> GetHistoryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/history", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<List<QMHistoryItem>>(json);
                return result ?? new List<QMHistoryItem>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取历史失败：{ex.Message}");
        }

        return new List<QMHistoryItem>();
    }

    public async Task<QMMessage[]> GetConversationAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/history/{conversationId}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<QMMessage[]>(json);
                return result ?? Array.Empty<QMMessage>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取对话失败：{ex.Message}");
        }

        return Array.Empty<QMMessage>();
    }

    public async Task SendMessageAsync(string conversationId, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new { conversation_id = conversationId, message };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/dialogue", content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"发送消息失败：{response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发送消息异常：{ex.Message}");
        }
    }

    public async IAsyncEnumerable<string> StreamResponseAsync(string conversationId, string message, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 直接调用内部方法，错误处理在内部方法中
        await foreach (var chunk in StreamResponseInternalAsync(conversationId, message, cancellationToken))
        {
            yield return chunk;
        }
    }

    private async IAsyncEnumerable<string> StreamResponseInternalAsync(string conversationId, string message, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var payload = new { conversation_id = conversationId, message, stream = true };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/stream")
        {
            Content = content
        };

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null)
            {
                yield break;
            }
            if (!string.IsNullOrEmpty(line))
            {
                if (line.StartsWith("data: "))
                {
                    var data = line.Substring(6);
                    if (data == "[DONE]")
                        yield break;

                    yield return data;
                }
            }
        }
    }

    public async Task<QMAgentStep> GetAgentProgressAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/agent/progress/{conversationId}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<QMAgentStep>(json);
                return result ?? new QMAgentStep();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取 Agent 进度失败：{ex.Message}");
        }

        return new QMAgentStep();
    }
}
