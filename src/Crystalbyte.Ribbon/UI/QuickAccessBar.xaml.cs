using System.Windows;
using System.Windows.Controls;

namespace Crystalbyte.UI {
    /// <summary>
    /// Interaction logic for QuickAccessBar.xaml
    /// </summary>
    public class QuickAccessBar : ItemsControl {

        #region Construction

        static QuickAccessBar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessBar),
             new FrameworkPropertyMetadata(typeof(QuickAccessBar)));
        }

        #endregion

        #region Class Overrides

        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is QuickAccessBarItem;
        }

        protected override DependencyObject GetContainerForItemOverride() {
            return new QuickAccessBarItem();
        }

        #endregion
    }
}
