namespace QMatrix.GUI.Models;

public class QMHistoryItem
{
    public QMHistoryItem()
    {
        Id = Guid.NewGuid().ToString();
        Title = "新对话";
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    public QMHistoryItem(string title)
        : this()
    {
        Title = title;
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MessageCount { get; set; }
    public string? Preview { get; set; }

    public void UpdatePreview(string preview)
    {
        Preview = preview;
        UpdatedAt = DateTime.Now;
    }

    public void IncrementMessageCount()
    {
        MessageCount++;
        UpdatedAt = DateTime.Now;
    }
}
