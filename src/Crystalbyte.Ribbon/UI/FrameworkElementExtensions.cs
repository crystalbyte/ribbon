using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Crystalbyte.UI {
    internal static class FrameworkElementExtensions {
        public static void BlendIn(this FrameworkElement element) {
            element.SetValue(Ribbon.IsFloatingProperty, true);
            element.SetValue(Grid.RowProperty, 0);
            element.SetValue(Grid.RowSpanProperty, 2);
            element.SetValue(Panel.ZIndexProperty, 1000);
            element.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
            element.Visibility = Visibility.Visible;

            // The sequence of calls in this method is important.
            // Don't change the layout after the animation has started.
            // The storyboard must be invoked last. 
            var story = (Storyboard) element.FindResource("RibbonBlendInStoryboard");
            story.Begin();
        }

        public static void BlendOut(this FrameworkElement element) {
            if (!(bool)element.GetValue(Ribbon.IsFloatingProperty)) {
                return;
            }
            element.SetValue(Ribbon.IsFloatingProperty, false);
            element.Visibility = Visibility.Collapsed;
        }

        public static void SnapIn(this FrameworkElement element) {
            element.SetValue(Ribbon.IsFloatingProperty, false);
            element.SetValue(Grid.RowProperty, 1);
            element.SetValue(Grid.RowSpanProperty, 1);
            element.SetValue(Panel.ZIndexProperty, 25);
            element.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);

            //
            element.Visibility = Visibility.Visible;
        }

        public static void SnapOut(this FrameworkElement element) {
            element.Visibility = Visibility.Collapsed;
        }

        internal static void ExtendIntoContent(this FrameworkElement element) {
            element.SetValue(Grid.RowSpanProperty, 2);
        }

        internal static void RetractFromContent(this FrameworkElement element) {
            element.SetValue(Grid.RowSpanProperty, 1);
        }
    }
}
