using System.Windows;
using System.Windows.Controls;

namespace Crystalbyte.UI {
    /// <summary>
    /// Interaction logic for ApplicationMenuSeparator.xaml
    /// </summary>
    public class ApplicationMenuSeparator : Separator {
        static ApplicationMenuSeparator() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationMenuSeparator),
              new FrameworkPropertyMetadata(typeof(ApplicationMenuSeparator)));
        }
    }
}
