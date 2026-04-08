namespace Wpf.Ui.Violeta.Controls;

public sealed class SecondarySubItem(string display, object? value) : ISecondarySubItem
{
    public string Display { get; set; } = display;

    public object? Tag { get; set; }

    public object? Value { get; set; } = value;
}

public interface ISecondarySubItem
{
    public string Display { get; set; }

    public object? Tag { get; set; }

    public object? Value { get; set; }
}
