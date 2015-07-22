#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

#endregion

namespace Crystalbyte.UI {
    /// <summary>
    /// This class represents a window with an integrated ribbon.
    /// </summary>
    [TemplatePart(Name = RibbonHostName, Type = typeof(Border))]
    [TemplatePart(Name = StatusBarName, Type = typeof(StatusBar))]
    [TemplatePart(Name = ApplicationMenuHostName, Type = typeof(Grid))]
    [TemplatePart(Name = RibbonOptionsPopupName, Type = typeof(Popup))]
    [TemplatePart(Name = RibbonOptionsListName, Type = typeof(ListView))]
    public class RibbonWindow : Window {

        #region Private Fields

        private Ribbon _ribbon;
        private ApplicationMenu _appMenu;
        private Border _ribbonHost;
        private StatusBar _statusBar;
        private Grid _appMenuHost;
        private ContentPresenter _contentPresenter;
        private Popup _ribbonOptionsPopup;
        private ListView _ribbonOptionsList;
        private HwndSource _hwndSource;
        private Storyboard _appMenuOpenStoryboard;
        private Storyboard _appMenuCloseStoryboard;

        #endregion

        #region Xaml Support

        public const string RibbonHostName = "PART_RibbonHost";
        public const string StatusBarName = "PART_StatusBar";
        public const string ApplicationMenuHostName = "PART_ApplicationMenuHost";
        public const string RibbonOptionsListName = "PART_RibbonOptionsList";
        public const string RibbonOptionsPopupName = "PART_RibbonOptionsPopup";
        public const string ContentPresenterName = "PART_ContentPresenter";

        #endregion

        #region Construction

        static RibbonWindow() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow),
                new FrameworkPropertyMetadata(typeof(RibbonWindow)));
        }

        public RibbonWindow() {
            Loaded += OnWindowLoaded;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            SourceInitialized += OnSourceInitialized;

            CommandBindings.Add(new CommandBinding(RibbonCommands.OpenAppMenu, OnOpenAppMenu));
            CommandBindings.Add(new CommandBinding(RibbonCommands.CloseAppMenu, OnCloseAppMenu));
            CommandBindings.Add(new CommandBinding(RibbonCommands.BlendInRibbon, OnBlendInRibbon));
            CommandBindings.Add(new CommandBinding(RibbonCommands.OpenRibbonOptions, OnOpenRibbonOptions));
            CommandBindings.Add(new CommandBinding(RibbonCommands.AddQuickAccess, OnAddQuickAccess));
            CommandBindings.Add(new CommandBinding(RibbonCommands.RemoveQuickAccess, OnRemoveQuickAccess));
            CommandBindings.Add(new CommandBinding(WindowCommands.Maximize, OnMaximize));
            CommandBindings.Add(new CommandBinding(WindowCommands.Minimize, OnMinimize));
            CommandBindings.Add(new CommandBinding(WindowCommands.RestoreDown, OnRestoredDown));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnClose));

            QuickAccessItems = new QuickAccessCollection();
        }

        #endregion

        #region Public Events

        public event EventHandler RibbonStateChanged;

        protected virtual void OnRibbonStateChanged() {
            var handler = RibbonStateChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Dependency Properties

        public ApplicationMenu ApplicationMenu {
            get { return (ApplicationMenu)GetValue(ApplicationMenuProperty); }
            set { SetValue(ApplicationMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ApplicationMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplicationMenuProperty =
            DependencyProperty.Register("ApplicationMenu", typeof(ApplicationMenu), typeof(RibbonWindow), new PropertyMetadata(null, OnApplicationMenuChanged));

        public QuickAccessCollection QuickAccessItems {
            get { return (QuickAccessCollection)GetValue(QuickAccessItemsProperty); }
            set { SetValue(QuickAccessItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuickAccessItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuickAccessItemsProperty =
            DependencyProperty.Register("QuickAccessItems", typeof(QuickAccessCollection), typeof(RibbonWindow), new PropertyMetadata(null));

        public RibbonState DefaultState {
            get { return (RibbonState)GetValue(DefaultStateProperty); }
            set { SetValue(DefaultStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultStateProperty =
            DependencyProperty.Register("DefaultState", typeof(RibbonState), typeof(RibbonWindow), new PropertyMetadata(RibbonState.Tabs));


        public RibbonState RibbonState {
            get { return (RibbonState)GetValue(RibbonStateProperty); }
            set { SetValue(RibbonStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RibbonState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RibbonStateProperty =
            DependencyProperty.Register("RibbonState", typeof(RibbonState), typeof(RibbonWindow),
                new PropertyMetadata(RibbonState.Tabs, OnRibbonStateChanged));

        public Thickness FramePadding {
            get { return (Thickness)GetValue(FramePaddingProperty); }
            set { SetValue(FramePaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FramePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FramePaddingProperty =
            DependencyProperty.Register("FramePadding", typeof(Thickness), typeof(RibbonWindow), new PropertyMetadata(new Thickness(0)));

        public bool IsNormalized {
            get { return (bool)GetValue(IsNormalizedProperty); }
            set { SetValue(IsNormalizedProperty, value); }
        }

        public Thickness ActualFramePadding {
            get { return (Thickness)GetValue(ActualFramePaddingProperty); }
            set { SetValue(ActualFramePaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualFramePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualFramePaddingProperty =
            DependencyProperty.Register("ActualFramePadding", typeof(Thickness), typeof(RibbonWindow), new PropertyMetadata(new Thickness(0)));

        public Thickness ActualBorderThickness {
            get { return (Thickness)GetValue(ActualBorderThicknessProperty); }
            set { SetValue(ActualBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualBorderThicknessProperty =
            DependencyProperty.Register("ActualBorderThickness", typeof(Thickness), typeof(RibbonWindow), new PropertyMetadata(new Thickness(0)));

        // Using a DependencyProperty as the backing store for QuickAccessCommands.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNormalizedProperty =
            DependencyProperty.Register("IsNormalized", typeof(bool), typeof(RibbonWindow),
                new PropertyMetadata(false));

        public bool IsMaximized {
            get { return (bool)GetValue(IsMaximizedProperty); }
            set { SetValue(IsMaximizedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMaximized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMaximizedProperty =
            DependencyProperty.Register("IsMaximized", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false));

        public object StatusBarItemsSource {
            get { return GetValue(StatusBarItemsSourceProperty); }
            set { SetValue(StatusBarItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarItemsSourceProperty =
            DependencyProperty.Register("StatusBarItemsSource", typeof(object), typeof(RibbonWindow), new PropertyMetadata(null));

        public DataTemplate StatusBarItemTemplate {
            get { return (DataTemplate)GetValue(StatusBarItemTemplateProperty); }
            set { SetValue(StatusBarItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarItemTemplateProperty =
            DependencyProperty.Register("StatusBarItemTemplate", typeof(DataTemplate), typeof(RibbonWindow), new PropertyMetadata(null));

        public ItemsPanelTemplate StatusBarItemsPanel {
            get { return (ItemsPanelTemplate)GetValue(StatusBarItemsPanelProperty); }
            set { SetValue(StatusBarItemsPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarItemsPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarItemsPanelProperty =
            DependencyProperty.Register("StatusBarItemsPanel", typeof(ItemsPanelTemplate), typeof(RibbonWindow), new PropertyMetadata(null));


        public Style StatusBarContainerStyle {
            get { return (Style)GetValue(StatusBarContainerStyleProperty); }
            set { SetValue(StatusBarContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarContainerStyleProperty =
            DependencyProperty.Register("StatusBarContainerStyle", typeof(Style), typeof(RibbonWindow), new PropertyMetadata(null));

        public Ribbon Ribbon {
            get { return (Ribbon)GetValue(RibbonProperty); }
            set { SetValue(RibbonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ribbon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RibbonProperty =
            DependencyProperty.Register("Ribbon", typeof(Ribbon), typeof(RibbonWindow), new PropertyMetadata(null, OnRibbonChanged));

        public bool IsAppMenuOpened {
            get { return (bool)GetValue(IsAppMenuOpenedProperty); }
            set { SetValue(IsAppMenuOpenedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAppMenuOpened.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAppMenuOpenedProperty =
            DependencyProperty.Register("IsAppMenuOpened", typeof(bool), typeof(RibbonWindow), new PropertyMetadata(false, OnIsAppMenuOpenedChanged));

        public Brush AccentBrush {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(RibbonWindow), new PropertyMetadata(null));

        public Brush HoverBrush {
            get { return (Brush)GetValue(HoverBrushProperty); }
            set { SetValue(HoverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HoverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register("HoverBrush", typeof(Brush), typeof(RibbonWindow), new PropertyMetadata(null));

        #endregion

        #region Event Handlers

        private static void OnApplicationMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var window = (RibbonWindow)d;

            var old = e.OldValue as ApplicationMenu;
            if (old != null) {
                window.DetachAppMenu(old);
            }

            var @new = e.NewValue as ApplicationMenu;
            if (@new != null) {
                window.AttachAppMenu(@new);
            }
        }

        private void AttachAppMenu(ApplicationMenu menu) {
            _appMenu = menu;
            _appMenu.SelectionChanged += OnAppMenuSelectionChanged;
        }

        private void DetachAppMenu(Selector menu) {
            if (menu != null) {
                menu.SelectionChanged -= OnAppMenuSelectionChanged;
            }
        }

        private void OnAppMenuSelectionChanged(object sender, SelectionChangedEventArgs e) {
            RunExchangeAnimation();
        }

        private static void OnIsAppMenuOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var window = (RibbonWindow)d;
            var value = (bool)e.NewValue;
            if (value) {
                window.OpenAppMenu();
            } else {
                window.CloseAppMenu();
            }
        }

        private void OnOpenAppMenu(object sender, ExecutedRoutedEventArgs e) {
            if (_appMenu == null) {
                return;
            }

            IsAppMenuOpened = true;
            _contentPresenter.Visibility = Visibility.Collapsed;
            _ribbonHost.BlendOut();
            _appMenu.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        private void OnCloseAppMenu(object sender, ExecutedRoutedEventArgs e) {
            _contentPresenter.Visibility = Visibility.Visible;
            IsAppMenuOpened = false;
        }

        private static void OnRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var window = (RibbonWindow)d;
            var ribbon = e.OldValue as Ribbon;
            if (ribbon != null) {
                window.DetachRibbon(ribbon);
            }
            window.AttachRibbon(e.NewValue as Ribbon);
        }

        private static void OnRibbonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var window = (RibbonWindow)d;
            window.OnRibbonStateChanged();
            window.UpdateRibbonBehavior();
            window.SyncRibbonOptionsSelection();
        }

        private async void OnRemoveQuickAccess(object sender, ExecutedRoutedEventArgs e) {
            if (!(e.Parameter is IQuickAccessConform))
                return;

            QuickAccessItems.Remove((IQuickAccessConform)e.Parameter);
            await StoreQuickAccessAsync();
        }

        private async void OnAddQuickAccess(object sender, ExecutedRoutedEventArgs e) {
            if (!(e.Parameter is IQuickAccessConform))
                return;

            QuickAccessItems.Add((IQuickAccessConform)e.Parameter);
            await StoreQuickAccessAsync();
        }

        private void OnRibbonSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count > 0 && !_ribbon.IsCommandStripVisible && RibbonState == RibbonState.Tabs) {
                _ribbon.SlideInCommandStrip();
            }
        }

        private void OnRibbonOptionsPopupMouseUp(object sender, MouseButtonEventArgs e) {
            _ribbonOptionsPopup.IsOpen = false;
        }

        private void OnRibbonOptionSelectionChanged(object sender, SelectionChangedEventArgs e) {
            var option = (RibbonOption)_ribbonOptionsList.SelectedValue;
            RibbonState = option.Visibility;
        }

        private void OnBlendInRibbon(object sender, ExecutedRoutedEventArgs e) {
            if (_ribbon == null || _ribbonHost == null) {
                return;
            }

            _ribbon.RestoreSelection();
            _ribbonHost.BlendIn();
            _statusBar.BlendIn();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (_ribbon == null || _ribbonHost == null) {
                return;
            }

            var point = e.GetPosition(sender as IInputElement);
            if (RibbonState == RibbonState.TabsAndCommands) {
                return;
            }

            var hit = HitTestFloatingControls(point);
            if (hit) return;

            if (RibbonState == RibbonState.Hidden) {
                _ribbonHost.BlendOut();
                _statusBar.BlendOut();
                return;
            }

            _ribbon.IsCommandStripVisible = false;
            _ribbon.ClearSelection();
        }

        internal void OpenAppMenu() {
            _statusBar.Visibility = Visibility.Collapsed;
            _appMenuOpenStoryboard.Begin();
        }

        internal void CloseAppMenu() {
            _appMenuCloseStoryboard.Begin();
        }

        private bool HitTestFloatingControls(Point point) {
            var visuals = new List<DependencyObject>();
            VisualTreeHelper.HitTest(this, OnFilterHitTestResult, target => {
                visuals.Add(target.VisualHit);
                return HitTestResultBehavior.Continue;
            }, new PointHitTestParameters(point));

            return visuals.Contains(_ribbon) || visuals.Contains(_statusBar);
        }

        private static HitTestFilterBehavior OnFilterHitTestResult(DependencyObject target) {
            if (target is Ribbon) {
                return HitTestFilterBehavior.ContinueSkipChildren;
            }

            if (target is StatusBar) {
                return HitTestFilterBehavior.ContinueSkipChildren;
            }

            return HitTestFilterBehavior.Continue;
        }

        private void OnOpenRibbonOptions(object sender, ExecutedRoutedEventArgs e) {
            _ribbonOptionsPopup.PlacementTarget = (UIElement)e.OriginalSource;
            _ribbonOptionsPopup.IsOpen = true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            UpdateWindowStates();
            UpdateFrameBorder();
            RestoreQuickAccess();
        }

        private void OnMinimize(object sender, ExecutedRoutedEventArgs e) {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void OnMaximize(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;
            e.Handled = true;
        }

        private void OnClose(object sender, RoutedEventArgs e) {
            Close();
            e.Handled = true;
        }

        private void OnRestoredDown(object sender, RoutedEventArgs e) {
            if (RibbonState == RibbonState.Hidden) {
                RibbonState = DefaultState;
            }
            WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void OnSourceInitialized(object sender, EventArgs e) {
            var helper = new WindowInteropHelper(this);
            _hwndSource = HwndSource.FromHwnd(helper.Handle);
        }

        #endregion

        #region Class Overrides

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);

            if (e.Property == BorderThicknessProperty) {
                ActualBorderThickness = (Thickness)e.NewValue;
            }
        }

        private void UpdateFrameBorder() {
            ActualBorderThickness = IsMaximized ? new Thickness(0) : BorderThickness;
            ActualFramePadding = IsMaximized ? new Thickness(0) : FramePadding;
        }

        protected override void OnStateChanged(EventArgs e) {
            base.OnStateChanged(e);
            if (WindowState != WindowState.Maximized && RibbonState == RibbonState.Hidden) {
                RibbonState = DefaultState;
            }
            UpdateWindowStates();
            UpdateFrameBorder();
        }

        public override void OnApplyTemplate() {
            // Should be called first
            // See http://msdn.microsoft.com/en-us/library/system.windows.frameworkelement.onapplytemplate(v=vs.110).aspx
            base.OnApplyTemplate();

            _contentPresenter = (ContentPresenter)Template.FindName(ContentPresenterName, this);

            _ribbonHost = (Border)Template.FindName(RibbonHostName, this);
            _statusBar = (StatusBar)Template.FindName(StatusBarName, this);

            if (_ribbonOptionsPopup != null) {
                _ribbonOptionsPopup.MouseUp -= OnRibbonOptionsPopupMouseUp;
            }

            _ribbonOptionsPopup = (Popup)Template.FindName(RibbonOptionsPopupName, this);
            _ribbonOptionsPopup.DataContext = this;
            _ribbonOptionsPopup.MouseUp += OnRibbonOptionsPopupMouseUp;

            if (_ribbonOptionsList != null) {
                _ribbonOptionsList.SelectionChanged -= OnRibbonOptionSelectionChanged;
            }

            _ribbonOptionsList = (ListView)Template.FindName(RibbonOptionsListName, this);
            _ribbonOptionsList.SelectionChanged += OnRibbonOptionSelectionChanged;
            _ribbonOptionsList.Items.AddRange(new[] {
                new RibbonOption {
                    Title = Properties.Resources.AutoHideRibbonTitle, 
                    Description = Properties.Resources.AutoHideRibbonDescription,
                    Visibility = RibbonState.Hidden,
                    ImageSource = new BitmapImage(new Uri("/Crystalbyte.Ribbon;component/Assets/autohide.png", UriKind.Relative))
                },
                new RibbonOption {
                    Title = Properties.Resources.ShowTabsTitle, 
                    Description = Properties.Resources.ShowTabsDescription,
                    Visibility = RibbonState.Tabs,
                    ImageSource = new BitmapImage(new Uri("/Crystalbyte.Ribbon;component/Assets/show.tabs.png", UriKind.Relative))
                },
                new RibbonOption {
                    Title = Properties.Resources.ShowTabsAndCommandsTitle, 
                    Description = Properties.Resources.ShowTabsAndCommandsDescription,
                    Visibility = RibbonState.TabsAndCommands,
                    ImageSource = new BitmapImage(new Uri("/Crystalbyte.Ribbon;component/Assets/show.tabs.commands.png", UriKind.Relative))
                }
            });

            _appMenuHost = (Grid)Template.FindName(ApplicationMenuHostName, this);

            if (_appMenuOpenStoryboard != null) {
                _appMenuOpenStoryboard.Completed -= OnAppMenuOpened;
            }

            _appMenuOpenStoryboard = (Storyboard)_appMenuHost.FindResource("OpenApplicationMenuStoryboard");
            _appMenuOpenStoryboard.Completed += OnAppMenuOpened;

            if (_appMenuCloseStoryboard != null) {
                _appMenuCloseStoryboard.Completed -= OnAppMenuClosed;
            }

            _appMenuCloseStoryboard = (Storyboard)_appMenuHost.FindResource("CloseApplicationMenuStoryboard");
            _appMenuCloseStoryboard.Completed += OnAppMenuClosed;

            SyncRibbonOptionsSelection();
        }

        private void OnAppMenuClosed(object sender, EventArgs e) {
            _appMenuHost.IsHitTestVisible = false;
            if (RibbonState != RibbonState.Hidden) {
                _statusBar.BlendIn();
            }
        }

        private void OnAppMenuOpened(object sender, EventArgs e) {
            _appMenuHost.IsHitTestVisible = true;
        }

        #endregion

        #region Methods

        private bool Minimize() {
            WindowState = WindowState.Minimized;
            return true;
        }

        private bool Maximize() {
            WindowState = WindowState.Maximized;
            return true;
        }

        private bool Restore() {
            WindowState = WindowState.Normal;
            return true;
        }

        private void UpdateWindowStates() {
            IsNormalized = WindowState == WindowState.Normal;
            IsMaximized = WindowState == WindowState.Maximized;
        }

        private void RunExchangeAnimation() {
            var story = (Storyboard)_appMenuHost.FindResource("ExchangeAppMenuContentStoryboard");
            story.Begin();
        }

        private void RestoreQuickAccess() {
            const string name = "ribbon.xml";
            if (!File.Exists(name)) {
                return;
            }

            try {
                var fs = File.OpenRead(name);
                using (var reader = new StreamReader(fs)) {
                    var serializer = new XmlSerializer(typeof(ArrayList));
                    var list = serializer.Deserialize(reader) as ArrayList;
                    if (list == null)
                        return;
                    var names = list.OfType<string>();
                    var items = names.Select(QuickAccessRegistry.Find).Where(x => x != null);
                    QuickAccessItems.AddRange(items);
                }
            } catch (IOException ex) {
                Debug.WriteLine(ex);
            }
        }

        private async Task StoreQuickAccessAsync() {
            var keys = new ArrayList(QuickAccessItems
                .Where(x => x.Key != null)
                .Select(x => x.Key)
                .ToArray());

            if (keys.Count == 0) {
                return;
            }

            const string name = "ribbon.xml";
            try {
                var fs = !File.Exists(name)
                    ? File.Create(name)
                    : File.Open(name, FileMode.Truncate, FileAccess.Write);

                using (var writer = new StreamWriter(fs)) {
                    var serializer = new XmlSerializer(typeof(ArrayList));
                    serializer.Serialize(writer, keys);
                    await writer.FlushAsync();
                }
            } catch (IOException ex) {
                Debug.WriteLine(ex);
            }
        }

        private void AttachRibbon(Ribbon ribbon) {
            _ribbon = ribbon;
            _ribbon.SelectionChanged += OnRibbonSelectionChanged;
        }

        private void DetachRibbon(Selector ribbon) {
            if (ribbon != null) {
                ribbon.SelectionChanged -= OnRibbonSelectionChanged;
            }
        }

        private void UpdateRibbonBehavior() {
            if (_ribbon == null || _statusBar == null || _ribbonHost == null) {
                return;
            }

            switch (RibbonState) {
                case RibbonState.Tabs:
                    _ribbon.IsCommandStripVisible = false;
                    _ribbon.IsWindowCommandStripVisible = false;
                    _ribbon.ClearSelection();
                    _statusBar.SnapIn();
                    _ribbonHost.SnapIn();
                    _ribbonHost.ExtendIntoContent();
                    break;
                case RibbonState.TabsAndCommands:
                    _ribbon.IsCommandStripVisible = true;
                    _ribbon.IsWindowCommandStripVisible = false;
                    _ribbon.RestoreSelection();
                    _statusBar.SnapIn();
                    _ribbonHost.SnapIn();
                    _ribbonHost.RetractFromContent();
                    break;
                case RibbonState.Hidden:
                    WindowState = WindowState.Maximized;
                    _ribbon.IsCommandStripVisible = true;
                    _ribbon.IsWindowCommandStripVisible = true;
                    _statusBar.SnapOut();
                    _ribbonHost.SnapOut();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SyncRibbonOptionsSelection() {
            if (_ribbonOptionsList == null) {
                return;
            }

            var option = _ribbonOptionsList.Items
                .OfType<RibbonOption>()
                .First(x => x.Visibility == RibbonState);

            option.IsSelected = true;
        }

        #endregion

        #region Native Window Support

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable MemberCanBePrivate.Local

        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private const int WM_GETMINMAXINFO = 0x0024;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_RESTORE = 0xF120;
        private const int SWP_NOMOVE = 0x0002;

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case WM_WINDOWPOSCHANGING:
                    handled = WmPositionChanging(lParam);
                    break;
                case WM_SYSCOMMAND:
                    if (wParam.ToInt32() == SC_MINIMIZE) {
                        handled = Minimize();
                        break;
                    }

                    if (wParam.ToInt32() == SC_MAXIMIZE) {
                        handled = Maximize();
                        break;
                    }

                    if (wParam.ToInt32() == SC_RESTORE) {
                        handled = Restore();
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        private bool WmPositionChanging(IntPtr lParam) {
            var target = _hwndSource.CompositionTarget;
            if (target == null) {
                return false;
            }

            var matrix = target.TransformToDevice;
            var minWidth = Convert.ToInt32(matrix.Transform(new Point(MinWidth, 0)).X);
            var minHeight = Convert.ToInt32(matrix.Transform(new Point(0, MinHeight)).Y);

            var pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
            if ((pos.flags & SWP_NOMOVE) != 0) {
                return false;
            }

            var isAdjusted = false;
            if (pos.cx < minWidth) {
                pos.cx = minWidth;
                isAdjusted = true;
            }
            if (pos.cy < minHeight) {
                pos.cy = minHeight;
                isAdjusted = true;
            }

            if (!isAdjusted) {
                return false;
            }

            Marshal.StructureToPtr(pos, lParam, true);
            return true;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor

            var monitor = NativeMethods.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero) {
                var monitorInfo = new MONITORINFOEX {
                    cbSize = Marshal.SizeOf(typeof(MONITORINFOEX))
                };

                NativeMethods.GetMonitorInfo(monitor, ref monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;

                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private static class NativeMethods {
            // To get a handle to the specified monitor
            [DllImport("user32.dll")]
            public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

            [DllImport("user32.dll")]
            public static extern bool GetMonitorInfo(IntPtr hmonitor, ref MONITORINFOEX monitorInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPOS {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT {
            public int x;
            public int y;

            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFOEX {
            public int cbSize;
            public RECT rcMonitor; // Total area
            public RECT rcWork; // Working area
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public char[] szDevice;
        }

        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore MemberCanBePrivate.Local

        #endregion
    }
}