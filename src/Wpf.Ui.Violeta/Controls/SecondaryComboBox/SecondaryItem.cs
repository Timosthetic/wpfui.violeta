using System.Collections.Generic;

namespace Wpf.Ui.Violeta.Controls;

public sealed class SecondaryItem(string group, IEnumerable<ISecondarySubItem> items) : ISecondaryItem
{
    public string Group { get; set; } = group;

    public object? Tag { get; set; }

    public IEnumerable<ISecondarySubItem> Items { get; set; } = items;
}

/// <summary>
/// Universal secondary menu data item interface
/// </summary>
public interface ISecondaryItem
{
    public string Group { get; set; }

    public object? Tag { get; set; }

    public IEnumerable<ISecondarySubItem> Items { get; set; }
}
