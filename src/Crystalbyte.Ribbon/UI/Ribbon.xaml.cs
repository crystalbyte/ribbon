#region Using directives

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#endregion

namespace Crystalbyte.UI {
    [TemplatePart(Name = RibbonCommandStripName, Type = typeof(Border))]
    public class Ribbon : TabControl {

        #region Private Fields

        private Border _commandStrip;

        #endregion

        #region Construction

        static Ribbon() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), 
                new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        #endregion

        #region Xaml Support

        public const string RibbonCommandStripName = "PART_RibbonCommandStrip";

        #endregion

        #region Class Overrides

        protected override DependencyObject GetContainerForItemOverride() {
            return new RibbonTab();
        }

        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is RibbonTab;
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters parameters) {
            return new PointHitTestResult(this, parameters.HitPoint);
        }

        #endregion

        #region Attached Properties

        public static bool GetIsFloating(DependencyObject d) {
            return (bool)d.GetValue(IsFloatingProperty);
        }

        public static void SetIsFloating(DependencyObject d, object value) {
            d.SetValue(IsFloatingProperty, value); 
        }

        // Using a DependencyProperty as the backing store for IsFloating.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFloatingProperty =
            DependencyProperty.RegisterAttached("IsFloating", typeof(bool), typeof(Ribbon), new PropertyMetadata(false));

        #endregion

        #region Dependency Properties

        public bool IsCommandStripVisible {
            get { return (bool)GetValue(IsCommandStripVisibleProperty); }
            set { SetValue(IsCommandStripVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HideCommandStrip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCommandStripVisibleProperty =
            DependencyProperty.Register("IsCommandStripVisible", typeof(bool), typeof(Ribbon), new PropertyMetadata(true));

        public bool IsWindowCommandStripVisible {
            get { return (bool)GetValue(IsWindowCommandStripVisibleProperty); }
            set { SetValue(IsWindowCommandStripVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWindowCommandBarVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWindowCommandStripVisibleProperty =
            DependencyProperty.Register("IsWindowCommandStripVisible", typeof(bool), typeof(Ribbon), new PropertyMetadata(false));


        public string AppMenuText {
            get { return (string)GetValue(AppMenuTextProperty); }
            set { SetValue(AppMenuTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppMenuButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AppMenuTextProperty =
            DependencyProperty.Register("AppMenuText", typeof(string), typeof(Ribbon),
                new PropertyMetadata(string.Empty));

        #endregion

        #region Class Overrides

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            _commandStrip = (Border) Template.FindName(RibbonCommandStripName, this);
        }

        #endregion

        #region Methods

        internal void SlideInCommandStrip() {
            IsCommandStripVisible = true;
            var story = (Storyboard)_commandStrip.FindResource("CommandStripSlideInStoryboard");
            story.Begin();
        }

        internal void ClearSelection() {
            SelectedIndex = -1;
        }

        internal void RestoreSelection() {
            if (HasItems && SelectedValue == null) {
                SelectedIndex = 0;
            }
        }

        #endregion
    }
}