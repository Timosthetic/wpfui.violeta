using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Wpf.Ui.Violeta.Controls;

public class SecondaryComboBox : Control
{
    private ToggleButton? _mainToggle;
    private Popup? _mainPopup;
    private Border? _popupBorder;
    private ListView? _countriesListView;
    private ListView? _domainsListView;

    static SecondaryComboBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SecondaryComboBox),
            new FrameworkPropertyMetadata(typeof(SecondaryComboBox)));
    }

    public SecondaryComboBox()
    {
        Unloaded += (_, _) => RemoveWindowMouseWheelHandler();
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
            control.FilteredDomains = new List<Tuple<string, Point>>();
            return;
        }

        if (TryCountryToDomains(country, out var domains))
        {
            control.FilteredDomains = domains.Select(i => new Tuple<string, Point>(i, new Point())).Reverse().ToList();
        }
        else
        {
            control.FilteredDomains = new List<Tuple<string, Point>>();
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

        var country = GetCountryByDomain(domain);
        if (!string.IsNullOrEmpty(country) && country != control.SelectedCountry)
        {
            control.SelectedCountry = country;
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_mainToggle != null)
        {
            _mainToggle.Click -= MainToggle_Click;
        }

        if (_mainPopup != null)
        {
            _mainPopup.Opened -= MainPopup_Opened;
            _mainPopup.Closed -= MainPopup_Closed;
        }

        _mainToggle = GetTemplateChild("MainToggle") as ToggleButton;
        _mainPopup = GetTemplateChild("MainPopup") as Popup;
        _popupBorder = GetTemplateChild("PopupBorder") as Border;
        _countriesListView = GetTemplateChild("CountriesListView") as ListView;
        _domainsListView = GetTemplateChild("DomainsListView") as ListView;

        if (_mainToggle != null)
        {
            _mainToggle.Click += MainToggle_Click;
        }

        if (_mainPopup != null)
        {
            _mainPopup.Opened += MainPopup_Opened;
            _mainPopup.Closed += MainPopup_Closed;
        }

        if (_domainsListView != null)
        {
            _domainsListView.SelectionChanged -= DomainsListView_SelectionChanged;
            _domainsListView.SelectionChanged += DomainsListView_SelectionChanged;
        }
    }

    private void MainToggle_Click(object? sender, RoutedEventArgs e)
    {
        if (_mainPopup != null)
        {
            _mainPopup.IsOpen = !_mainPopup.IsOpen;
        }
    }

    private void DomainsListView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_mainToggle != null && _mainToggle.IsChecked == true)
        {
            _mainToggle.IsChecked = false;
        }
    }

    private void MainPopup_Opened(object? sender, EventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window != null)
        {
            window.PreviewMouseWheel -= Window_PreviewMouseWheel;
            window.PreviewMouseWheel += Window_PreviewMouseWheel;
        }
    }

    private void MainPopup_Closed(object? sender, EventArgs e)
    {
        RemoveWindowMouseWheelHandler();
    }

    private void RemoveWindowMouseWheelHandler()
    {
        var window = Window.GetWindow(this);
        if (window != null)
            window.PreviewMouseWheel -= Window_PreviewMouseWheel;
    }

    private void Window_PreviewMouseWheel(object? sender, MouseWheelEventArgs e)
    {
        if (_mainPopup?.IsOpen != true)
            return;

        e.Handled = true;

        var sv1 = FindScrollViewer(_countriesListView);
        var sv2 = FindScrollViewer(_domainsListView);

        if (sv1 != null && sv1.IsMouseOver)
        {
            sv1.ScrollToVerticalOffset(sv1.VerticalOffset - e.Delta / 2.0);
            return;
        }

        if (sv2 != null && sv2.IsMouseOver)
        {
            sv2.ScrollToVerticalOffset(sv2.VerticalOffset - e.Delta / 2.0);
            return;
        }
    }

    private void PopupBorder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        var sv1 = FindScrollViewer(_countriesListView);
        var sv2 = FindScrollViewer(_domainsListView);

        if (sv1 != null && sv1.IsMouseOver)
        {
            sv1.ScrollToVerticalOffset(sv1.VerticalOffset - e.Delta / 2.0);
            return;
        }

        if (sv2 != null && sv2.IsMouseOver)
        {
            sv2.ScrollToVerticalOffset(sv2.VerticalOffset - e.Delta / 2.0);
            return;
        }
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject? parent)
    {
        if (parent == null) return null;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is ScrollViewer sv) return sv;

            var res = FindScrollViewer(child);
            if (res != null) return res;
        }

        return null;
    }

    // Placeholder implementations — keep simple mappings for now
    private static string GetCountryByDomain(string domain)
    {
        return string.Empty;
    }

    private static bool TryCountryToDomains(string country, out List<string> domains)
    {
        domains = new List<string>();
        return false;
    }
}
