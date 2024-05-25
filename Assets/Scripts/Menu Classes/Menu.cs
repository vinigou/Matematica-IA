using UnityEngine;
using UnityEngine.UIElements;

public class Menu
{
    protected MenuIndex index;
    protected VisualTreeAsset rootVisualElement;
    protected VisualElement container;
    protected string header, header2, button, margins, screen_idle, screen_enter, screen_leave;

    public virtual void Initialize(VisualElement root)
    {
        rootVisualElement = UIBehavior.instance.visualTreeAssets[(int)index];
        VisualElement tree = rootVisualElement.CloneTree();
        root.Add(tree);

        header = "header";
        header2 = "header2";
        button = "button";
        margins = "margins";
        screen_idle = "screen-idle";
        screen_enter = "screen-enter";
        screen_leave = "screen-leave";
    }

    public virtual void Show()
    {
        container.style.display = DisplayStyle.Flex;
    }

    public virtual void Hide()
    {

    }

    public virtual void Reset()
    {
        container.style.display = DisplayStyle.None;
        container.ClearClassList();
    }

    protected virtual void OnButtonClick()
    {

    }
}