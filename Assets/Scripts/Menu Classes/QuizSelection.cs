using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public class QuizSelection : Menu
{
    private VisualElement _generationContainer, _errorContainer;
    private ScrollView _scrollView;
    private Button _buttonOperacoes1, _buttonGeometria1, _buttonMedidas1, _buttonProbabilidade1,
        _buttonOperacoes2, _buttonAlgebra, _buttonGeometria2, _buttonMedidas2, _buttonProbabilidade2,
        _buttonReturn;
    private bool _quizSelected, _returnScreen;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.QuizSelection;
        base.Initialize(root);

        container = root.Q("ContainerQuizSelect");
        _scrollView = root.Q("QuizButtonScrollView") as ScrollView;

        _buttonOperacoes1 = root.Q("ButtonOperacoes1") as Button;
        _buttonOperacoes2 = root.Q("ButtonOperacoes2") as Button;
        _buttonGeometria1 = root.Q("ButtonGeometria1") as Button;
        _buttonGeometria2 = root.Q("ButtonGeometria2") as Button;
        _buttonMedidas1 = root.Q("ButtonMedidas1") as Button;
        _buttonMedidas2 = root.Q("ButtonMedidas2") as Button;
        _buttonProbabilidade1 = root.Q("ButtonProbabilidade1") as Button;
        _buttonProbabilidade2 = root.Q("ButtonProbabilidade2") as Button;
        _buttonAlgebra = root.Q("ButtonAlgebra") as Button;

        _buttonReturn = root.Q("ReturnFromQuizSelectButton") as Button;

        _generationContainer = root.Q("QuizGenerationContainer");
        _errorContainer = root.Q("QuizSelectExceptionContainer");

        _scrollView.RegisterCallback<WheelEvent>((evt) =>
        {
            _scrollView.scrollOffset = new Vector2(0, _scrollView.scrollOffset.y + 1000f * evt.delta.y);
            evt.StopPropagation();
        });

        _buttonOperacoes1.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Operacoes1));
        _buttonOperacoes2.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Operacoes2));
        _buttonGeometria1.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Geometria1));
        _buttonGeometria2.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Geometria2));
        _buttonMedidas1.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Medidas1));
        _buttonMedidas2.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Medidas2));
        _buttonProbabilidade1.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Probabilidade1));
        _buttonProbabilidade2.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Probabilidade2));
        _buttonAlgebra.RegisterCallback<ClickEvent>(evt => OnQuizButtonPress(QuizIndex.Algebra));

        _buttonReturn.RegisterCallback<ClickEvent>(evt => OnReturnButtonPress());
    }

    public override void Show()
    {
        base.Show();

        _returnScreen = false;
        _quizSelected = false;

        container.RemoveFromClassList(screen_enter);
    }

    public override void Reset()
    {
        base.Reset();
        _generationContainer.style.display = DisplayStyle.None;
        _errorContainer.style.display = DisplayStyle.None;
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

    private void OnQuizButtonPress(QuizIndex index)
    {
        base.OnButtonClick();

        // GENERATE QUIZ
        // ONCE GENERATED, TRANSITION
        string subject;
        string difficulty;

        switch (index)
        {
            case QuizIndex.Operacoes1:
                subject = "Números e Operações";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Medium);
                break;
            case QuizIndex.Operacoes2:
                subject = "Números e Operações";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Hard);
                break;
            case QuizIndex.Geometria1:
                subject = "Geometria";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Medium);
                break;
            case QuizIndex.Geometria2:
                subject = "Geometria";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Hard);
                break;
            case QuizIndex.Medidas1:
                subject = "Grandezas e Medidas";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Medium);
                break;
            case QuizIndex.Medidas2:
                subject = "Grandezas e Medidas";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Hard);
                break;
            case QuizIndex.Probabilidade1:
                subject = "Probabilidade e Estatística";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Medium);
                break;
            case QuizIndex.Probabilidade2:
                subject = "Probabilidade e Estatística";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Hard);
                break;
            case QuizIndex.Algebra:
                subject = "Álgebra";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Medium);
                break;
            default:
                subject = "matemática";
                difficulty = GameManager.instance.AdjustQuizDifficulty(Difficulty.Medium);
                break;
        }

        AttemptQuizGenerate(subject, difficulty, index, OnQuizGenerated);
    }

    private async void AttemptQuizGenerate(string subject, string difficulty, QuizIndex index, Action<bool> OnSuccess)
    {
        try
        {
            _generationContainer.style.display = DisplayStyle.Flex;
            await GameManager.instance.GenerateQuiz(subject, difficulty, index, OnSuccess);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            OnSuccess?.Invoke(false);
        }
    }

    private void OnQuizGenerated(bool success)
    {
        _generationContainer.style.display = DisplayStyle.None;

        if (success)
        {
            QuizManager.StartQuiz();

            _quizSelected = true;
            Hide();
        }
        else
        {
            _quizSelected = false;

            WaitForSeconds waitForSeconds = new(5f);
            GameManager.instance.CoroutineStarter(DisplayError(waitForSeconds));
            Debug.Log("Failed");
        }
    }

    private IEnumerator DisplayError(WaitForSeconds seconds)
    {
        _errorContainer.style.display = DisplayStyle.Flex;
        yield return seconds;
        _errorContainer.style.display = DisplayStyle.None;
        yield break;
    }

    private void OnReturnButtonPress()
    {
        base.OnButtonClick();

        _returnScreen = true;
        Hide();
    }

    private void OnHidden(TransitionEndEvent evt)
    {
        if (evt.target == container && evt.stylePropertyNames.Contains("translate") && container.ClassListContains(screen_leave))
        {
            Reset();

            if (_quizSelected)
            {
                UIBehavior.instance.QuestionsMenu.Show();
                _quizSelected = false;
            }
            else if(_returnScreen)
            {
                UIBehavior.instance.Options.Show();
                _returnScreen = false;
            }
        }
    }
}
