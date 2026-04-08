using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Ui.Violeta.Controls;

[TemplatePart(Name = PART_GroupsListView, Type = typeof(ListView))]
[TemplatePart(Name = PART_ItemsListView, Type = typeof(ListView))]
[TemplatePart(Name = PART_SelectedText, Type = typeof(TextBlock))]
public class CascadingComboBox : ComboBox
{
    public const string PART_GroupsListView = "PART_GroupsListView";
    public const string PART_ItemsListView = "PART_ItemsListView";
    public const string PART_SelectedText = "PART_SelectedText";

    private ListView? _groupsListView;
    private ListView? _itemsListView;

    static CascadingComboBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CascadingComboBox), new FrameworkPropertyMetadata(typeof(CascadingComboBox)));
    }

    /// <summary>
    /// Secondary menu data source
    /// </summary>
    public IEnumerable<ISecondaryItem> ItemsSource2
    {
        get => (IEnumerable<ISecondaryItem>)GetValue(ItemsSource2Property);
        set => SetValue(ItemsSource2Property, value);
    }

    public static readonly DependencyProperty ItemsSource2Property =
        DependencyProperty.Register(nameof(ItemsSource2), typeof(IEnumerable<ISecondaryItem>), typeof(CascadingComboBox), new PropertyMetadata(Array.Empty<ISecondaryItem>(), OnItemsSource2Changed));

    private static void OnItemsSource2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CascadingComboBox)d;
        control.UpdateGroups();
    }

    /// <summary>
    /// Currently selected main group
    /// </summary>
    public ISecondaryItem? SelectedGroup
    {
        get => (ISecondaryItem?)GetValue(SelectedGroupProperty);
        set => SetValue(SelectedGroupProperty, value);
    }

    public static readonly DependencyProperty SelectedGroupProperty =
        DependencyProperty.Register(nameof(SelectedGroup), typeof(ISecondaryItem), typeof(CascadingComboBox), new PropertyMetadata(null, OnSelectedGroupChanged));

    private static void OnSelectedGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CascadingComboBox)d;
        control.UpdateItems();
    }

    /// <summary>
    /// 当前选中的子项
    /// </summary>
    public ISecondarySubItem? SelectedSubItem
    {
        get => (ISecondarySubItem?)GetValue(SelectedSubItemProperty);
        set => SetValue(SelectedSubItemProperty, value);
    }

    public static readonly DependencyProperty SelectedSubItemProperty =
        DependencyProperty.Register(nameof(SelectedSubItem), typeof(ISecondarySubItem), typeof(CascadingComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedSubItemChanged));

    private static void OnSelectedSubItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CascadingComboBox)d;
        if (e.NewValue is ISecondarySubItem && control.SelectedGroup != null)
        {
            // 选中子项后关闭下拉
            control.IsDropDownOpen = false;
        }
    }

    /// <summary>
    /// 当前分组下的子项集合
    /// </summary>
    public IEnumerable<ISecondarySubItem> FilteredItems
    {
        get => (IEnumerable<ISecondarySubItem>)GetValue(FilteredItemsProperty);
        set => SetValue(FilteredItemsProperty, value);
    }

    public static readonly DependencyProperty FilteredItemsProperty =
        DependencyProperty.Register(nameof(FilteredItems), typeof(IEnumerable<ISecondarySubItem>), typeof(CascadingComboBox), new PropertyMetadata(Array.Empty<ISecondarySubItem>()));

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _groupsListView = GetTemplateChild(PART_GroupsListView) as ListView;
        _itemsListView = GetTemplateChild(PART_ItemsListView) as ListView;

        if (_groupsListView != null)
        {
            _groupsListView.SelectionChanged -= GroupsListView_SelectionChanged;
            _groupsListView.SelectionChanged += GroupsListView_SelectionChanged;
        }
        if (_itemsListView != null)
        {
            _itemsListView.SelectionChanged -= ItemsListView_SelectionChanged;
            _itemsListView.SelectionChanged += ItemsListView_SelectionChanged;
        }
    }

    private void GroupsListView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_groupsListView?.SelectedItem is ISecondaryItem group)
        {
            SelectedGroup = group;
        }
    }

    private void ItemsListView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_itemsListView?.SelectedItem is ISecondarySubItem subItem)
        {
            SelectedSubItem = subItem;
        }
    }

    private void UpdateGroups()
    {
        // The first group is selected by default
        var groups = ItemsSource2?.ToList() ?? [];
        if (groups.Count > 0)
        {
            SelectedGroup = groups[0];
        }
        else
        {
            SelectedGroup = null;
        }
    }

    private void UpdateItems()
    {
        if (SelectedGroup != null)
        {
            FilteredItems = SelectedGroup.Items?.ToList() ?? [];
        }
        else
        {
            FilteredItems = [];
        }
    }
}
