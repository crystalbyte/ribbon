using System.Windows;
using System.Windows.Controls;

namespace Crystalbyte.UI {
    /// <summary>
    /// Interaction logic for ApplicationMenuItem.xaml
    /// </summary>
    public class ApplicationMenuItem : TabItem {

        #region Construction

        static ApplicationMenuItem() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationMenuItem),
                new FrameworkPropertyMetadata(typeof(ApplicationMenuItem)));
        }

        #endregion
    }
}
