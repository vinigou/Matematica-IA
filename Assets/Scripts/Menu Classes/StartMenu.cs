using UnityEngine.UIElements;

public class StartMenu : Menu
{
    private Button _startButton;
    private string _startMenuEnter, _startMenuIdle, _startMenuLeave;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.StartMenu;

        base.Initialize(root);

        container = root.Q("ContainerStart");

        _startButton = root.Q("StartButton") as Button;
        _startButton.RegisterCallback<ClickEvent>(OnStartButtonClick);

        _startMenuEnter = "startMenu-enter";
        _startMenuIdle = "startMenu-idle";
        _startMenuLeave = "startMenu-leave";
    }

    public override void Show()
    {
        base.Show();
        container.RemoveFromClassList(_startMenuEnter);
    }

    public override void Hide()
    {
        base.Hide();

        container.AddToClassList(_startMenuLeave);
        container.RegisterCallback<TransitionEndEvent>(ShowInsertKey);
    }

    public override void Reset()
    {
        base.Reset();

        container.AddToClassList(_startMenuEnter);
        container.AddToClassList(_startMenuIdle);

    }

    private void OnStartButtonClick(ClickEvent evt)
    {
        base.OnButtonClick();
        Hide();
    }

    private void ShowInsertKey(TransitionEndEvent evt)
    {
        if (evt.target == container && evt.stylePropertyNames.Contains("translate") && container.ClassListContains(_startMenuLeave))
        {
            Reset();
            UIBehavior.instance.InsertKey.Show();
        }
    }
}
