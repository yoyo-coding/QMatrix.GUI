using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace QMatrix.GUI;

public sealed partial class MainPage : Page
{
    public ChatViewModel ViewModel { get; private set; } = null!;
    public HistoryViewModel HistoryViewModel { get; private set; } = null!;

    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var app = (App)App.Current;
            if (app?.Host?.Services != null)
            {
                ViewModel = app.Host.Services.GetRequiredService<ChatViewModel>();
                HistoryViewModel = app.Host.Services.GetRequiredService<HistoryViewModel>();
                
                // 加载历史记录
                _ = HistoryViewModel.LoadHistoryCommand.ExecuteAsync(null);
                
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                HistoryViewModel.ItemSelected += HistoryViewModel_ItemSelected;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"初始化页面失败：{ex.Message}");
        }
    }

    private void HistoryViewModel_ItemSelected(object? sender, QMHistoryItem e)
    {
        ViewModel?.LoadConversation(e.Id);
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChatViewModel.Messages))
        {
            ScrollToBottom();
        }
    }

    private void ScrollToBottom()
    {
        if (MessagesScrollViewer != null)
        {
            MessagesScrollViewer.ChangeView(0, MessagesScrollViewer.ScrollableHeight, null);
        }
    }

    private void InputTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel != null && !ViewModel.IsSending)
            {
                ViewModel.SendMessageCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void NewConversation_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.LoadConversation(Guid.NewGuid().ToString());
    }
}
