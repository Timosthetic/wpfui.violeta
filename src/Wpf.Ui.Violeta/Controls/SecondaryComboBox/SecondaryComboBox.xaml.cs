using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Wpf.Ui.Violeta.Controls;

[Obsolete("Under development")]
public partial class SecondaryComboBox : UserControl
{
    public SecondaryComboBox()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Clean the event handler when the control is unloaded to prevent memory leaks
    /// </summary>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        RemoveWindowMouseWheelHandler();
    }

    public List<string> Countries
    {
        get => (List<string>)GetValue(CountriesProperty);
        set => SetValue(CountriesProperty, value);
    }

    public static readonly DependencyProperty CountriesProperty =
        DependencyProperty.Register("Countries", typeof(List<string>), typeof(SecondaryComboBox), new PropertyMetadata(null));

    public string SelectedCountry
    {
        get => (string)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    public static readonly DependencyProperty SelectedCountryProperty =
        DependencyProperty.Register("SelectedCountry", typeof(string), typeof(SecondaryComboBox), new PropertyMetadata(null, OnSelectedCountryChanged));

    private static void OnSelectedCountryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (SecondaryComboBox)d;
        var country = (string)e.NewValue;
        if (string.IsNullOrEmpty(country))
        {
            control.FilteredDomains = [];
        }
        else
        {
            if (TryCountryToDomains(country, out var domains))
            {
                // Reverse the list for display
                control.FilteredDomains = [.. domains.Select(i => new Tuple<string, Point>(i.ToString(), new())).Reverse()];
            }
            else
            {
                control.FilteredDomains = [];
            }
        }
    }

    public List<Tuple<string, Point>> FilteredDomains
    {
        get => (List<Tuple<string, Point>>)GetValue(FilteredDomainsProperty);
        set => SetValue(FilteredDomainsProperty, value);
    }

    public static readonly DependencyProperty FilteredDomainsProperty =
        DependencyProperty.Register("FilteredDomains", typeof(List<Tuple<string, Point>>), typeof(SecondaryComboBox), new PropertyMetadata(null));

    public string SelectedDomain
    {
        get => (string)GetValue(SelectedDomainProperty);
        set => SetValue(SelectedDomainProperty, value);
    }

    public static readonly DependencyProperty SelectedDomainProperty =
        DependencyProperty.Register("SelectedDomain", typeof(string), typeof(SecondaryComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDomainChanged));

    private static void OnSelectedDomainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (SecondaryComboBox)d;
        var domain = (string)e.NewValue;

        if (string.IsNullOrEmpty(domain)) return;

        // Verify if domain matches current country, if not, update country
        var country = GetCountryByDomain(domain);
        if (country != null && country != control.SelectedCountry)
        {
            control.SelectedCountry = country;
        }
    }

    private void DomainListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainToggle.IsChecked == true)
        {
            MainToggle.IsChecked = false;
        }
    }

    /// <summary>
    /// Add global wheel event interception when Popup is opened
    /// </summary>
    private void MainPopup_Opened(object sender, EventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window != null)
        {
            window.PreviewMouseWheel -= Window_PreviewMouseWheel;
            window.PreviewMouseWheel += Window_PreviewMouseWheel;
        }
    }

    /// <summary>
    /// Remove global wheel event interception when Popup is closed
    /// </summary>
    private void MainPopup_Closed(object sender, EventArgs e)
    {
        RemoveWindowMouseWheelHandler();
    }

    /// <summary>
    /// Remove window level scroll wheel event handler
    /// </summary>
    private void RemoveWindowMouseWheelHandler()
    {
        var window = Window.GetWindow(this);
        window?.PreviewMouseWheel -= Window_PreviewMouseWheel;
    }

    /// <summary>
    /// Global scroll wheel event processing, intercept all scroll wheel events when the Popup is opened
    /// </summary>
    private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (MainPopup.IsOpen)
        {
            e.Handled = true;

            var scrollViewer1 = FindScrollViewer(CountriesListView);
            var scrollViewer2 = FindScrollViewer(DomainsListView);

            if (scrollViewer1 != null && scrollViewer1.IsMouseOver)
            {
                scrollViewer1.ScrollToVerticalOffset(scrollViewer1.VerticalOffset - e.Delta / 2.0);
                return;
            }

            if (scrollViewer2 != null && scrollViewer2.IsMouseOver)
            {
                scrollViewer2.ScrollToVerticalOffset(scrollViewer2.VerticalOffset - e.Delta / 2.0);
                return;
            }
        }
    }

    /// <summary>
    /// Handle mouse wheel events in Popup to prevent scrolling from penetrating to external pages
    /// </summary>
    private void PopupBorder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        var scrollViewer1 = FindScrollViewer(CountriesListView);
        var scrollViewer2 = FindScrollViewer(DomainsListView);

        if (scrollViewer1 != null && scrollViewer1.IsMouseOver)
        {
            scrollViewer1.ScrollToVerticalOffset(scrollViewer1.VerticalOffset - e.Delta / 2.0);
            return;
        }

        if (scrollViewer2 != null && scrollViewer2.IsMouseOver)
        {
            scrollViewer2.ScrollToVerticalOffset(scrollViewer2.VerticalOffset - e.Delta / 2.0);
            return;
        }
    }

    /// <summary>
    /// Find ScrollViewer in the visual tree
    /// </summary>
    private static ScrollViewer? FindScrollViewer(DependencyObject parent)
    {
        if (parent == null) return null;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            var result = FindScrollViewer(child);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    // TODO
    private static string GetCountryByDomain(string domain)
    {
        return "X";
    }

    // TODO
    private static bool TryCountryToDomains(string country, out string domain)
    {
        domain = "Y";
        return true;
    }
}
