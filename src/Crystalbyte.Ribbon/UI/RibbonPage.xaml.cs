using System.Windows;
using System.Windows.Controls;

namespace Crystalbyte.UI {
    /// <summary>
    /// Interaction logic for RibbonPage.xaml
    /// </summary>
    public class RibbonPage : ItemsControl {

        #region Construction

        static RibbonPage() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonPage),
              new FrameworkPropertyMetadata(typeof(RibbonPage)));
        }

        #endregion

        #region Class Overrides

        protected override DependencyObject GetContainerForItemOverride() {
            return new RibbonGroup();
        }

        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is RibbonGroup;
        }

        #endregion
    }
}
