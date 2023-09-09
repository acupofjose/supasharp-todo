using Microsoft.AspNetCore.Components;

namespace SupasharpTodo.Shared.Interfaces;

public interface INavigationIcon : IComponent
{
    /// <summary>
    /// Url this Icon directs to.
    /// </summary>
    string? Location { get; }
    
    /// <summary>
    /// The Displayed title
    /// </summary>
    string? Title { get; }
    
    /// <summary>
    /// CSS Classes Applied to the Icon
    /// </summary>
    string? IconClasses { get; }
    
    /// <summary>
    /// CSS Classes Applied to the Text
    /// </summary>
    string? TextClasses { get; }
}