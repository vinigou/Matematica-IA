using UnityEngine.UIElements;

public class ExitScreen : Menu
{
    private Button _xButton, _confirmButton, _cancelButton;
    private VisualElement _closeGameContainer, _tree;
    private string _treeShow, _treeHidden;

    public override void Initialize(VisualElement root)
    {
        index = MenuIndex.ExitScreen; 
        
        rootVisualElement = UIBehavior.instance.visualTreeAssets[(int)index];
        VisualElement tree = rootVisualElement.CloneTree();
        root.Add(tree);

        container = root.Q("ExitScreenContainer");
        _closeGameContainer = root.Q("CloseGameContainer");

        _xButton = root.Q("CloseGameButtonShow") as Button;
        _confirmButton = root.Q("CloseGameButtonConfirm") as Button;
        _cancelButton = root.Q("CloseGameButtonReturn") as Button;

        _xButton.RegisterCallback<ClickEvent>(evt => OnXButtonPress());
        _confirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonPress());
        _cancelButton.RegisterCallback<ClickEvent>(evt => OnCancelButtonPress());

        _treeShow = "treeShow";
        _treeHidden = "treeHidden";
        tree.AddToClassList(_treeShow);
        tree.AddToClassList(_treeHidden);

        _tree = tree;
    }

    public override void Show()
    {
        
    }

    public override void Hide()
    {
        _closeGameContainer.style.display = DisplayStyle.None;
    }

    public override void Reset()
    {
        // Reset shouldn't be called
    }

    public void ShowExit()
    {
        _tree.RemoveFromClassList(_treeHidden);
        _closeGameContainer.style.display = DisplayStyle.Flex;
    }

    public void HideExit()
    {
        _tree.AddToClassList(_treeHidden);
        _closeGameContainer.style.display = DisplayStyle.None;
    }

    private void OnXButtonPress()
    {
        ShowExit();
    }

    private void OnConfirmButtonPress()
    {
        GameManager.instance.QuitProgram();
    }

    private void OnCancelButtonPress()
    {
        HideExit();
    }
}