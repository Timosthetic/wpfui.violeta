using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Wpf.Ui.Violeta.Controls;

[TemplatePart(Name = PART_MainToggle, Type = typeof(ToggleButton))]
[TemplatePart(Name = PART_MainPopup, Type = typeof(Popup))]
[TemplatePart(Name = PART_PopupBorder, Type = typeof(Border))]
[TemplatePart(Name = PART_CountriesListView, Type = typeof(ListView))]
[TemplatePart(Name = PART_DomainsListView, Type = typeof(ListView))]
[TemplatePart(Name = PART_SelectedText, Type = typeof(TextBlock))]
[SuppressMessage("Style", "IDE0052:Remove unread private members")]
public class SecondaryComboBox : Control
{
    public const string PART_MainToggle = "PART_MainToggle";
    public const string PART_MainPopup = "PART_MainPopup";
    public const string PART_PopupBorder = "PART_PopupBorder";
    public const string PART_CountriesListView = "PART_CountriesListView";
    public const string PART_DomainsListView = "PART_DomainsListView";
    public const string PART_SelectedText = "PART_SelectedText";

    private ToggleButton? _mainToggle;
    private Popup? _mainPopup;
    private Border? _popupBorder;
    private ListView? _countriesListView;
    private ListView? _domainsListView;

    static SecondaryComboBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SecondaryComboBox), new FrameworkPropertyMetadata(typeof(SecondaryComboBox)));
    }

    public SecondaryComboBox()
    {
    }

    public List<string> Countries
    {
        get => (List<string>)GetValue(CountriesProperty);
        set => SetValue(CountriesProperty, value);
    }

    public static readonly DependencyProperty CountriesProperty =
        DependencyProperty.Register("Countries", typeof(List<string>), typeof(SecondaryComboBox), new PropertyMetadata(new List<string>()));

    public string SelectedCountry
    {
        get => (string)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    public static readonly DependencyProperty SelectedCountryProperty =
        DependencyProperty.Register("SelectedCountry", typeof(string), typeof(SecondaryComboBox), new PropertyMetadata(string.Empty, OnSelectedCountryChanged));

    private static void OnSelectedCountryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (SecondaryComboBox)d;
        var country = (string?)e.NewValue;

        if (string.IsNullOrEmpty(country))
        {
            control.FilteredDomains = [];
            return;
        }

        if (TryCountryToDomains(country!, out var domains))
        {
            control.FilteredDomains = [.. domains.Select(i => new Tuple<string, Point>(i, new Point())).Reverse()];
        }
        else
        {
            control.FilteredDomains = [];
        }
    }

    public List<Tuple<string, Point>> FilteredDomains
    {
        get => (List<Tuple<string, Point>>)GetValue(FilteredDomainsProperty);
        set => SetValue(FilteredDomainsProperty, value);
    }

    public static readonly DependencyProperty FilteredDomainsProperty =
        DependencyProperty.Register("FilteredDomains", typeof(List<Tuple<string, Point>>), typeof(SecondaryComboBox), new PropertyMetadata(new List<Tuple<string, Point>>()));

    public string SelectedDomain
    {
        get => (string)GetValue(SelectedDomainProperty);
        set => SetValue(SelectedDomainProperty, value);
    }

    public static readonly DependencyProperty SelectedDomainProperty =
        DependencyProperty.Register("SelectedDomain", typeof(string), typeof(SecondaryComboBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDomainChanged));

    private static void OnSelectedDomainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (SecondaryComboBox)d;
        var domain = (string?)e.NewValue;
        if (string.IsNullOrEmpty(domain)) return;

        var country = GetCountryByDomain(domain!);
        if (!string.IsNullOrEmpty(country) && country != control.SelectedCountry)
        {
            control.SelectedCountry = country;
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _mainToggle?.Click -= MainToggle_Click;

        _mainToggle = GetTemplateChild(PART_MainToggle) as ToggleButton;
        _mainPopup = GetTemplateChild(PART_MainPopup) as Popup;
        _popupBorder = GetTemplateChild(PART_PopupBorder) as Border;
        _countriesListView = GetTemplateChild(PART_CountriesListView) as ListView;
        _domainsListView = GetTemplateChild(PART_DomainsListView) as ListView;

        _mainToggle?.Click += MainToggle_Click;

        _domainsListView?.SelectionChanged -= DomainsListView_SelectionChanged;
        _domainsListView?.SelectionChanged += DomainsListView_SelectionChanged;
    }

    private void MainToggle_Click(object? sender, RoutedEventArgs e)
    {
        _mainPopup?.IsOpen = !_mainPopup.IsOpen;
    }

    private void DomainsListView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _mainPopup?.IsOpen = false;
    }

    // Placeholder implementations — keep simple mappings for now
    private static string GetCountryByDomain(string domain)
    {
        return string.Empty;
    }

    private static bool TryCountryToDomains(string country, out List<string> domains)
    {
        domains = [];
        return false;
    }
}
