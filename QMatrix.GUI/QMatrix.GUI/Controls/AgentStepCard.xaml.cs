using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using QMatrix.GUI.Models;

namespace QMatrix.GUI.Controls;

public sealed partial class AgentStepCard : UserControl
{
    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(nameof(Step), typeof(QMAgentStep), typeof(AgentStepCard), new PropertyMetadata(null, OnStepChanged));

    public QMAgentStep Step
    {
        get => (QMAgentStep)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public AgentStepCard()
    {
        InitializeComponent();
    }

    private static void OnStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AgentStepCard card && e.NewValue is QMAgentStep step)
        {
            card.UpdateStep(step);
        }
    }

    private void UpdateStep(QMAgentStep step)
    {
        StepContentText.Text = step.Content;
        
        switch (step.StepType)
        {
            case "Thought":
                StepIcon.Glyph = "\uE9CD";
                StepIcon.Foreground = new SolidColorBrush(Colors.ForestGreen);
                StepTypeText.Text = "思考";
                StepTypeText.Foreground = new SolidColorBrush(Colors.ForestGreen);
                break;
            case "Action":
                StepIcon.Glyph = "\uE9F5";
                StepIcon.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                StepTypeText.Text = "行动";
                StepTypeText.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                break;
            case "Observation":
                StepIcon.Glyph = "\uE894";
                StepIcon.Foreground = new SolidColorBrush(Colors.Orange);
                StepTypeText.Text = "观察";
                StepTypeText.Foreground = new SolidColorBrush(Colors.Orange);
                break;
        }
    }
}
