using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Crystalbyte.UI;

namespace Crystalbyte.Converters {
    [ValueConversion(typeof(RibbonState), typeof(Visibility))]
    public sealed class RibbonStateToVisibilityConverter : IValueConverter {

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var validValues = parameter as string;
            if (string.IsNullOrWhiteSpace(validValues)) {
                return Visibility.Collapsed;
            }

            var current = (RibbonState) value;
            var enums = validValues.Split('|').Select(x => (RibbonState)Enum.Parse(typeof(RibbonState), x));
            return enums.Any(x => x == current) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }

        #endregion
    }
}
