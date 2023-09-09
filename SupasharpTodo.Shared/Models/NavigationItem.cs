namespace SupasharpTodo.Shared.Models;

public class NavigationItem
{
    public delegate void OnClickHandler(string location);

    public string Location { get; private set; }
    public string Title { get; private set; }
    public Type IconType { get; private set; }

    public string IconClasses { get; private set; }

    public OnClickHandler? OnClick { get; private set; }

    public bool IsHighlighted { get; set; } = false;

    public NavigationItem(string title, string location, Type iconTypeType,
        string iconClasses = "w-4 h-4 stroke-default", OnClickHandler? handler = null)
    {
        Title = title;
        Location = location;
        IconType = iconTypeType;
        IconClasses = iconClasses;
        OnClick = handler;
    }
}