using UnityEngine.UIElements;

public class InsertKey : Menu
{
    private TextField TextFieldKey;
    private Button _confirmButton;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.InsertKey;
        base.Initialize(root);

        container = root.Q("ContainerInsertKey");
        TextFieldKey = root.Q("TextFieldKey") as TextField;
        _confirmButton = root.Q("ConfirmKeyButton") as Button;
        
        _confirmButton.RegisterCallback<ClickEvent>(OnConfirmButtonClick);
    }

    public override void Show()
    {
        base.Show();
        container.RemoveFromClassList(screen_enter);
    }

    public override void Hide()
    {
        base.Hide();
        container.AddToClassList(screen_leave);
        container.RegisterCallback<TransitionEndEvent>(ShowQuizSelect);
    }

    public override void Reset()
    {
        base.Reset();
        container.AddToClassList(screen_idle);
        container.AddToClassList(screen_enter);
    }

    private void OnConfirmButtonClick(ClickEvent evt)
    {
        base.OnButtonClick();

        string key = TextFieldKey.text;
        GameManager.instance.SetKey(key);

        Hide();
    }

    private void ShowQuizSelect(TransitionEndEvent evt)
    {
        if (evt.target == container && evt.stylePropertyNames.Contains("translate") && container.ClassListContains(screen_leave))
        {
            Reset();

            UIBehavior.instance.Options.Show();
        }
    }
}
