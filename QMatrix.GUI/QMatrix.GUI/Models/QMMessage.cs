namespace QMatrix.GUI.Models;

public enum MessageRole
{
    User,
    Agent
}

public enum MessageType
{
    Normal,
    AgentStep
}

public class QMMessage
{
    public QMMessage()
    {
        Id = Guid.NewGuid().ToString();
        Content = string.Empty;
        Timestamp = DateTime.Now;
        AgentSteps = new List<QMAgentStep>();
    }

    public QMMessage(MessageRole role, string content)
        : this()
    {
        Role = role;
        Content = content;
    }

    public string Id { get; set; }
    public MessageRole Role { get; set; }
    public MessageType Type { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public List<QMAgentStep> AgentSteps { get; set; }
    public bool IsStreaming { get; set; }
}

public class QMAgentStep
{
    public QMAgentStep()
    {
        StepType = "Thought";
        Content = string.Empty;
        Timestamp = DateTime.Now;
    }

    public QMAgentStep(string stepType, string content)
        : this()
    {
        StepType = stepType;
        Content = content;
    }

    public string StepType { get; set; }
    public string Content { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime Timestamp { get; set; }
}
