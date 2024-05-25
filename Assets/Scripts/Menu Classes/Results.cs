using UnityEngine;
using UnityEngine.UIElements;

public class Results : Menu
{
    private ScrollView _scrollView;
    private Label _resultsText, _resultsDescription;
    private Button _resultsReturnButton;
    private string _resultsTextGood, _resultsTextAverage, _resultsTextFailed;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.Results;
        base.Initialize(root);

        container = root.Q("ContainerResults");
        _scrollView = container.Q("ResultsScrollView") as ScrollView;
        _resultsText = container.Q("ResultsText") as Label;
        _resultsDescription = container.Q("ResultsDescription") as Label;
        _resultsReturnButton = container.Q("ResultsReturnButton") as Button;

        _resultsTextGood = "resultsTextGood";
        _resultsTextAverage = "resultsTextAverage";
        _resultsTextFailed = "resultsTextFailed";

        _scrollView.RegisterCallback<WheelEvent>((evt) =>
        {
            _scrollView.scrollOffset = new Vector2(0, _scrollView.scrollOffset.y + 1000f * evt.delta.y);
            evt.StopPropagation();
        });

        _resultsReturnButton.RegisterCallback<ClickEvent>(evt => OnReturnButtonPress());
        
    }

    public override void Show()
    {
        base.Show();
        DisplayResults();
        container.RemoveFromClassList(screen_enter);
    }

    public override void Reset()
    {
        base.Reset();
        _resultsText.ClearClassList();
        _resultsText.AddToClassList("resultsText");
        container.AddToClassList(screen_idle);
        container.AddToClassList(screen_enter);
        _scrollView.scrollOffset = new Vector2(0, 0);
    }

    public override void Hide()
    {
        base.Hide();
        container.AddToClassList(screen_leave);
        container.RegisterCallback<TransitionEndEvent>(OnHidden);
    }

    private void DisplayResults()
    {
        float correct = QuizManager.performance.totalCorrect;
        float total = QuizManager.performance.totalQuestions;
        float result = correct / total;

        _resultsText.text = $"{correct}/{total}";

        if (result >= 0.7)
        {
            _resultsText.AddToClassList(_resultsTextGood);
            _resultsDescription.text = "Parabéns, você arrasou!";
        }
        else if (result >= 0.5 && result < 0.7)
        {
            _resultsText.AddToClassList(_resultsTextAverage);
            _resultsDescription.text = "Atingiu a média!";
        }
        else if (result < 0.5)
        {
            _resultsText.AddToClassList(_resultsTextFailed);
            _resultsDescription.text = "Tente novamente...";
        }
    }

    private void OnReturnButtonPress()
    {
        base.OnButtonClick();

        Hide();
    }

    private void OnHidden(TransitionEndEvent evt)
    {
        if (evt.target == container && evt.stylePropertyNames.Contains("translate") && container.ClassListContains(screen_leave))
        {
            Reset();
            UIBehavior.instance.QuizSelection.Show();
        }
    }
}
