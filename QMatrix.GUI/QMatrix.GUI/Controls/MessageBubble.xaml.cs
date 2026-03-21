using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using QMatrix.GUI.Models;

namespace QMatrix.GUI.Controls;

public sealed partial class MessageBubble : UserControl
{
    private static readonly Windows.UI.Color UserColor = Windows.UI.Color.FromArgb(255, 0, 120, 212);
    private static readonly Windows.UI.Color AgentColor = Windows.UI.Color.FromArgb(255, 243, 243, 243);

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(QMMessage), typeof(MessageBubble), new PropertyMetadata(null, OnMessageChanged));

    public QMMessage Message
    {
        get => (QMMessage)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public MessageBubble()
    {
        InitializeComponent();
    }

    private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MessageBubble bubble && e.NewValue is QMMessage message)
        {
            bubble.UpdateMessage(message);
        }
    }

    private void UpdateMessage(QMMessage message)
    {
        MessageText.Text = message.Content;
        TimestampText.Text = message.Timestamp.ToString("HH:mm");

        if (message.Role == MessageRole.User)
        {
            MessageBorder.Background = new SolidColorBrush(UserColor);
            MessageText.Foreground = new SolidColorBrush(Colors.White);
            MessageBorder.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Right;
        }
        else
        {
            MessageBorder.Background = new SolidColorBrush(AgentColor);
            MessageText.Foreground = new SolidColorBrush(Colors.Black);
            MessageBorder.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left;
        }

        // 清理之前的 AgentStepCard
        var parent = MessageText.Parent as StackPanel;
        if (parent != null)
        {
            // 移除除了 MessageText 和 TimestampText 之外的所有子元素
            var childrenToRemove = parent.Children.Where(child => child != MessageText && child != TimestampText).ToList();
            foreach (var child in childrenToRemove)
            {
                parent.Children.Remove(child);
            }
        }

        if (message.Type == MessageType.AgentStep && message.AgentSteps != null && message.AgentSteps.Count > 0)
        {
            var stackPanel = new StackPanel();
            foreach (var step in message.AgentSteps)
            {
                var stepCard = new AgentStepCard { Step = step };
                stackPanel.Children.Add(stepCard);
            }
            MessageText.Visibility = Visibility.Collapsed;
            if (parent != null)
            {
                parent.Children.Insert(0, stackPanel);
            }
        }
        else
        {
            MessageText.Visibility = Visibility.Visible;
        }
    }
}
