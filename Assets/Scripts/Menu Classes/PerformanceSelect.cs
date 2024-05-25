using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PerformanceSelect : Menu
{
    private VisualTreeAsset PerformanceSelectButtonTree => UIBehavior.instance.performanceSelectButtonTreeAsset;
    private List<Button> _performanceList;
    private ScrollView _scrollView;
    private VisualElement _buttonsContainer;
    private Label _noPerformancesLabel;
    private Button _returnButton;

    private List<SubjectResults> Results => GameManager.instance.results.subjectResults;

    private bool _performancePressed;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.PerformanceSelect;
        base.Initialize(root);

        container = root.Q("PerformanceSelectContainer");

        _scrollView = container.Q("PerformanceSelectView") as ScrollView;
        _scrollView.RegisterCallback<WheelEvent>((evt) =>
        {
            _scrollView.scrollOffset = new Vector2(0, _scrollView.scrollOffset.y + 1000f * evt.delta.y);
            evt.StopPropagation();
        });

        _buttonsContainer = _scrollView.Q("PerformanceViewContainer");

        Button button = container.Q("PerformanceSelectButton") as Button;
        _returnButton = container.Q("ReturnFromPerformanceSelectButton") as Button;
        _performanceList = new();

        _noPerformancesLabel = container.Q("NoPerformancesLabel") as Label;
        _noPerformancesLabel.style.display = DisplayStyle.None;

        _returnButton.RegisterCallback<ClickEvent>(evt => OnReturnButtonPress());

        _performancePressed = false;
    }

    public override void Show()
    {
        base.Show();

        container.RemoveFromClassList(screen_enter);
    }

    public override void Reset()
    {
        base.Reset();
        container.AddToClassList(screen_idle);
        container.AddToClassList(screen_enter);
    }

    public override void Hide()
    {
        base.Hide();
        container.AddToClassList(screen_leave);
        container.RegisterCallback<TransitionEndEvent>(OnHidden);
    }

    public void UpdateButtons()
    {
        ClearButtons();

        if (Results.Count > 0)
        {
            _scrollView.style.display = DisplayStyle.Flex;

            foreach (var result in Results)
            {
                VisualElement performanceButtonSelectElement = PerformanceSelectButtonTree.CloneTree();
                Button performanceSelectButton = performanceButtonSelectElement.Q("PerformanceSelectButton") as Button;
                string buttonText = result.index switch
                {
                    QuizIndex.Operacoes1 => $"Números e Operações (Simples) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Operacoes2 => $"Números e Operações (Avançado) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Geometria1 => $"Geometria (Simples) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Geometria2 => $"Geometria (Avançado) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Medidas1 => $"Grandezas e Medidas (Simples) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Medidas2 => $"Grandezas e Medidas (Avançado) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Probabilidade1 => $"Probabilidade e Estatística (Simples) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Probabilidade2 => $"Probabilidade e Estatística (Avançado) {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    QuizIndex.Algebra => $"Álgebra {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                    _ => $"Matemática {result.performance.totalCorrect}/{result.performance.totalQuestions}",
                };
                performanceSelectButton.text = buttonText;

                _buttonsContainer.Add(performanceSelectButton);
                performanceSelectButton.RegisterCallback<ClickEvent>(evt => OnPerformanceSelectButtonPress(result));

                _performanceList.Add(performanceSelectButton);
            }
        }
        else
        {
            _noPerformancesLabel.style.display = DisplayStyle.Flex;
            _scrollView.style.display = DisplayStyle.None;
        }
    }

    private void ClearButtons()
    {
        _buttonsContainer.Clear();
        _performanceList.Clear();
        _noPerformancesLabel.style.display = DisplayStyle.None;
    }

    private void OnPerformanceSelectButtonPress(SubjectResults result)
    {
        base.OnButtonClick();
        _performancePressed = true;

        UIBehavior.instance.PerformanceDisplay.UpdateScreen(result);

        Hide();
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

            if (_performancePressed)
            {
                UIBehavior.instance.PerformanceDisplay.Show();
            }
            else
            {
                UIBehavior.instance.Options.Show();
            }

            _performancePressed = false;
        }
    }
}

