#region Using directives

using System.Windows.Input;
using Crystalbyte.Properties;

#endregion

namespace Crystalbyte.UI {
    public static class WindowCommands {
        public static RoutedUICommand Minimize = 
            new RoutedUICommand(Resources.MinimizeCommandTooltip, Resources.MinimizeCommandName, typeof(RibbonWindow));

        public static RoutedUICommand Maximize = 
            new RoutedUICommand(Resources.MaximizeCommandTooltip, Resources.MaximizeCommandName, typeof(RibbonWindow));

        public static RoutedUICommand RestoreDown = 
            new RoutedUICommand(Resources.RestoreDownCommandTooltip, Resources.RestoreDownCommandName, typeof(RibbonWindow));
    }
}