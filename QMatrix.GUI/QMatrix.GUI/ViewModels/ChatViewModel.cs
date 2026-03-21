using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QMatrix.GUI.Models;
using QMatrix.GUI.Services;

namespace QMatrix.GUI.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly IQMatrixApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<QMMessage> _messages = new();

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isSending;

    [ObservableProperty]
    private string? _currentConversationId;

    [ObservableProperty]
    private bool _isStreaming;

    public ChatViewModel(IQMatrixApiService apiService)
    {
        _apiService = apiService;
    }

    public void LoadConversation(string conversationId)
    {
        CurrentConversationId = conversationId;
        Messages.Clear();
        _ = LoadMessagesAsync();
    }

    private async Task LoadMessagesAsync()
    {
        if (string.IsNullOrEmpty(CurrentConversationId))
            return;

        var messages = await _apiService.GetConversationAsync(CurrentConversationId);
        foreach (var message in messages)
        {
            Messages.Add(message);
        }
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsSending)
            return;

        IsSending = true;
        var userMessage = InputText;
        InputText = string.Empty;

        if (string.IsNullOrEmpty(CurrentConversationId))
        {
            CurrentConversationId = Guid.NewGuid().ToString();
        }

        var userMsg = new QMMessage
        {
            Role = MessageRole.User,
            Content = userMessage,
            Timestamp = DateTime.Now
        };
        Messages.Add(userMsg);

        await _apiService.SendMessageAsync(CurrentConversationId, userMessage);

        var agentMsg = new QMMessage
        {
            Role = MessageRole.Agent,
            Content = string.Empty,
            IsStreaming = true,
            Timestamp = DateTime.Now
        };
        Messages.Add(agentMsg);

        IsStreaming = true;
        var sb = new System.Text.StringBuilder();

        try
        {
            await foreach (var chunk in _apiService.StreamResponseAsync(CurrentConversationId, userMessage))
            {
                sb.Append(chunk);
                agentMsg.Content = sb.ToString();
                OnPropertyChanged(nameof(Messages));
            }
        }
        finally
        {
            IsStreaming = false;
            agentMsg.IsStreaming = false;
            OnPropertyChanged(nameof(Messages));
        }

        IsSending = false;
    }

    [RelayCommand]
    private void StopStreaming()
    {
        IsStreaming = false;
    }
}
