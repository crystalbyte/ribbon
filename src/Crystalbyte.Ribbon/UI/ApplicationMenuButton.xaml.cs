using System.Windows;
using System.Windows.Controls;

namespace Crystalbyte.UI {
    /// <summary>
    /// Interaction logic for ApplicationMenuButton.xaml
    /// </summary>
    public class ApplicationMenuButton : Button {

        #region Construction

        static ApplicationMenuButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationMenuButton),
             new FrameworkPropertyMetadata(typeof(ApplicationMenuButton)));
        }

        #endregion
    }
}
