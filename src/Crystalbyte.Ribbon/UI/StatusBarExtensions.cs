using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Crystalbyte.UI {
    public static class StatusBarExtensions {
        public static void BlendIn(this StatusBar bar) {
            bar.Visibility = Visibility.Visible;
            bar.SetValue(Grid.RowProperty, 3);
            bar.SetValue(Grid.RowSpanProperty, 1);
            bar.SetValue(Panel.ZIndexProperty, 1000);
            bar.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            // The sequence of calls in this method is important.
            // Don't change the layout after the animation has started.
            // The storyboard must be invoked last. 
            var story = (Storyboard)bar.FindResource("StatusBarBlendInStoryboard");
            story.Begin();
        }

        public static void BlendOut(this StatusBar bar) {
            bar.Visibility = Visibility.Collapsed;
        }

        public static void SnapOut(this StatusBar bar) {
            bar.Visibility = Visibility.Collapsed;
        }

        public static void SnapIn(this StatusBar bar) {
            bar.Visibility = Visibility.Visible;
            bar.SetValue(Grid.RowProperty, 3);
            bar.SetValue(Grid.RowSpanProperty, 1);
            bar.SetValue(Panel.ZIndexProperty, 25);
            bar.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        }
    }
}
