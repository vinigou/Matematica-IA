using UnityEngine;
using UnityEngine.UIElements;

public class PerformanceDisplay : Menu
{
    private VisualTreeAsset QuestionTemplate => UIBehavior.instance.pdQuestionTreeAsset;
    private VisualTreeAsset AnswerTemplate => UIBehavior.instance.pdAnswerTreeAsset;
    private ScrollView _pDisplayScroll;
    private Label _pdSubjectLabel;
    private VisualElement _allQuestionsContainer;
    private Button _returnButton;
    private string _pdCorrectUnselected, _pdSelectedWrong, _pdSelectedCorrect;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.PerformanceDisplay;
        base.Initialize(root);

        container = root.Q("PDisplayContainer");
        _pDisplayScroll = root.Q("PDisplayScroll") as ScrollView;
        _pDisplayScroll.RegisterCallback<WheelEvent>((evt) =>
        {
            _pDisplayScroll.scrollOffset = new Vector2(0, _pDisplayScroll.scrollOffset.y + 1000f * evt.delta.y);
            evt.StopPropagation();
        });

        _allQuestionsContainer = container.Q("PDAllQuestionsContainer");
        _allQuestionsContainer.Clear();

        _pdSubjectLabel = container.Q("PDSubjectLabel") as Label;

        _returnButton = container.Q("PDisplayReturnButton") as Button;

        _returnButton.RegisterCallback<ClickEvent>(evt => OnReturnButtonPress());

        _pdCorrectUnselected = "pdAnswerNotSelectedCorrect";
        _pdSelectedWrong = "pdAnswerSelectedWrong";
        _pdSelectedCorrect = "pdAnswerSelectedCorrect";
    }

    public override void Show()
    {
        base.Show();

        container.RemoveFromClassList(screen_enter);
    }

    public override void Reset()
    {
        base.Reset();

        _allQuestionsContainer?.Clear();
        container.AddToClassList(screen_idle);
        container.AddToClassList(screen_enter);
    }

    public override void Hide()
    {
        base.Hide();
        container.AddToClassList(screen_leave);
        container.RegisterCallback<TransitionEndEvent>(OnHidden);
    }

    public void UpdateScreen(SubjectResults result)
    {
        string subject = result.index switch
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
        _pdSubjectLabel.text = subject;

        int count = 0;
        foreach (var question in result.quiz.questions)
        {
            VisualElement questionElement = QuestionTemplate.CloneTree();
            VisualElement questionContainer = questionElement.Q("PDQuestionContainer");
            Label number = questionContainer.Q("PDQuestionNumberLabel") as Label;
            Label questionText = questionContainer.Q("PDQuestionTextLabel") as Label;
            VisualElement answerContainer = questionContainer.Q("PDAnswerContainer");

            answerContainer.Clear();

            bool isCorrect = result.performance.correctAnswers[count];
            string isCorrectText;
            if (isCorrect)
                isCorrectText = "Correto";
            else
                isCorrectText = "Incorreto";

            number.text = $"Questão {count+1} - {isCorrectText}";

            questionText.text = result.quiz.questions[count].question;

            for (int i = 0; i < 4; i++)
            {
                VisualElement answerElement = AnswerTemplate.CloneTree();
                Label answer = answerElement.Q("PDAnswer") as Label;

                switch (i)
                {
                    case 0:
                        answer.text = result.quiz.questions[count].answer1;
                        break;
                    case 1:
                        answer.text = result.quiz.questions[count].answer2;
                        break;
                    case 2:
                        answer.text = result.quiz.questions[count].answer3;
                        break;
                    case 3:
                        answer.text = result.quiz.questions[count].answer4;
                        break;
                }

                bool correct = false;
                bool selected = false;

                if (i+1 == result.quiz.questions[count].correctAnswer)
                    correct = true;
                if (i+1 == result.performance.selectedAnswers[count])
                    selected = true;

                if (correct && selected)
                    answer.AddToClassList(_pdSelectedCorrect);
                else if (correct && !selected)
                    answer.AddToClassList(_pdCorrectUnselected);
                else if (!correct && selected)
                    answer.AddToClassList(_pdSelectedWrong);

                answerContainer.Add(answer);
            }

            _allQuestionsContainer.Add(questionContainer);
            count++;
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

            UIBehavior.instance.PerformanceSelect.Show();
        }
    }
}

