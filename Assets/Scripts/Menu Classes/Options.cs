using UnityEngine.UIElements;

public class Options : Menu
{
    private Button _selectQuizButton, _checkPerformanceButton, _changeKeyButton;
    private OptionsIndex _optionsIndex;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.Options;
        base.Initialize(root);

        container = root.Q("ContainerOptions");

        _selectQuizButton = root.Q("SelectQuizButton") as Button;
        _checkPerformanceButton = root.Q("CheckPerformanceButton") as Button;
        _changeKeyButton = root.Q("ChangeKeyButton") as Button;

        _selectQuizButton.RegisterCallback<ClickEvent>(evt => OnOptionsButtonPress(OptionsIndex.Quiz));
        _checkPerformanceButton.RegisterCallback<ClickEvent>(evt => OnOptionsButtonPress(OptionsIndex.Performance));
        _changeKeyButton.RegisterCallback<ClickEvent>(evt => OnOptionsButtonPress(OptionsIndex.Key));
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

    private void OnOptionsButtonPress(OptionsIndex index)
    {
        base.OnButtonClick();

        _optionsIndex = index;
        Hide();
    }

    private void OnHidden(TransitionEndEvent evt)
    {
        if (evt.target == container && evt.stylePropertyNames.Contains("translate") && container.ClassListContains(screen_leave))
        {
            Reset();

            switch (_optionsIndex)
            {
                case OptionsIndex.Quiz:
                    UIBehavior.instance.QuizSelection.Show();
                    break;
                case OptionsIndex.Performance:
                    UIBehavior.instance.PerformanceSelect.Show();
                    break;
                case OptionsIndex.Key:
                    UIBehavior.instance.InsertKey.Show();
                    break;
                default:
                    UIBehavior.instance.Options.Show();
                    break;
            }
        }
    }

    private enum OptionsIndex
    {
        Quiz,
        Performance,
        Key
    }
}
