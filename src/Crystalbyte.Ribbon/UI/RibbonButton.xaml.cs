using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Crystalbyte.UI {
    public class RibbonButton : Button, IQuickAccessConform {

        #region Construction

        static RibbonButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButton),
               new FrameworkPropertyMetadata(typeof(RibbonButton)));
        }

        public RibbonButton() {
            QuickAccessRegistry.Register(this);
        }

        #endregion

        #region Dependency Properties

        public ImageSource ImageSource {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RibbonButton), new PropertyMetadata(null));

        #endregion

        #region Implementation of IQuickAccessConform

        public ImageSource QuickAccessImageSource {
            get { return ImageSource; }
        }

        ICommand IQuickAccessConform.Command {
            get { return Command; }
        }

        object IQuickAccessConform.CommandParameter {
            get { return CommandParameter; }
        }

        object IQuickAccessConform.ToolTip {
            get { return ToolTip; }
        }

        public string Key {
            get {
                if (!string.IsNullOrWhiteSpace(QuickAccessKeyOverride)) {
                    return QuickAccessKeyOverride;
                }

                var routed = Command as RoutedCommand;
                if (routed != null) {
                    return routed.Name;
                }
                return Command == null ? null : Command.GetType().FullName;
            }
        }

        #endregion

        #region Dependency Properties

        public string QuickAccessKeyOverride {
            get { return (string)GetValue(QuickAccessKeyOverrideProperty); }
            set { SetValue(QuickAccessKeyOverrideProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuickAccessKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuickAccessKeyOverrideProperty =
            DependencyProperty.Register("QuickAccessKeyOverride", typeof(string), typeof(RibbonButton), new PropertyMetadata(string.Empty));

        #endregion
    }
}
