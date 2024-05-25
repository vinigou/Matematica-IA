using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestionsMenu : Menu
{
    private Label _question, _questionNumber;
    private ScrollView _questionScrollView, _answerScrollView;
    private Button _answerButton1, _answerButton2, _answerButton3, _answerButton4;
    private string _screenIdle, _screenEnter, _screenLeave;
    
    private bool _isLastQuestion;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.QuestionsMenu;
        base.Initialize(root);

        container = root.Q("QuestionsMenuContainer");
        _question = container.Q("Question") as Label;
        _questionNumber = container.Q("QuestionNumber") as Label;
        _questionScrollView = container.Q("QuestionScrollView") as ScrollView;
        _answerScrollView = container.Q("AnswersScroll") as ScrollView;
        _answerButton1 = container.Q("AnswerButton1") as Button;
        _answerButton2 = container.Q("AnswerButton2") as Button;
        _answerButton3 = container.Q("AnswerButton3") as Button;
        _answerButton4 = container.Q("AnswerButton4") as Button;

        _screenIdle = "screen-idle";
        _screenEnter = "screen-enter";
        _screenLeave = "screen-leave";

        _questionScrollView.RegisterCallback<WheelEvent>((evt) =>
        {
            _questionScrollView.scrollOffset = new Vector2(0, _questionScrollView.scrollOffset.y + 1000f * evt.delta.y);
            evt.StopPropagation();
        });
        _answerScrollView.RegisterCallback<WheelEvent>((evt) =>
        {
            _answerScrollView.scrollOffset = new Vector2(0, _answerScrollView.scrollOffset.y + 1000f * evt.delta.y);
            evt.StopPropagation();
        });

        _answerButton1.RegisterCallback<ClickEvent>((evt) => HandleAnswerButton(1));
        _answerButton2.RegisterCallback<ClickEvent>((evt) => HandleAnswerButton(2));
        _answerButton3.RegisterCallback<ClickEvent>((evt) => HandleAnswerButton(3));
        _answerButton4.RegisterCallback<ClickEvent>((evt) => HandleAnswerButton(4));
    }
    public override void Show()
    {
        base.Show();

        _isLastQuestion = false;
        ApplyQuestionToUI(QuizManager.currentQuestion);

        container.RemoveFromClassList(_screenEnter);
    }

    public override void Hide()
    {
        base.Hide();
        container.AddToClassList(_screenLeave);
        container.RegisterCallback<TransitionEndEvent>(OnQuestionAnswered);
    }

    public override void Reset()
    {
        base.Reset();
        container.AddToClassList(_screenIdle);
        container.AddToClassList(_screenEnter);
    }

    private void ApplyQuestionToUI(int index)
    {
        _question.text = QuizManager.quiz.questions[index].question;
        _answerButton1.text = QuizManager.quiz.questions[index].answer1;
        _answerButton2.text = QuizManager.quiz.questions[index].answer2;
        _answerButton3.text = QuizManager.quiz.questions[index].answer3;
        _answerButton4.text = QuizManager.quiz.questions[index].answer4;
        _questionNumber.text = $"{index + 1}/{QuizManager.quiz.questions.Count}";
    }

    private void HandleAnswerButton(int answerButtonId)
    {
        base.OnButtonClick();

        _isLastQuestion = QuizManager.HandleAnswer(answerButtonId);
        Hide();
    }

    private void OnQuestionAnswered(TransitionEndEvent evt)
    {
        if (evt.target == container && evt.stylePropertyNames.Contains("translate") && container.ClassListContains(screen_leave))
        {
            Reset();
            
            if (_isLastQuestion)
            {
                UIBehavior.instance.Results.Show();
            }
            else
            {
                GameManager.instance.CoroutineStarter(NextQuestion(new WaitForSeconds(0.5f)));
            }
        }
    }

    private IEnumerator NextQuestion(WaitForSeconds waitTime)
    {
        yield return waitTime;

        ApplyQuestionToUI(QuizManager.currentQuestion);
        Show();
        yield break;
    }
}