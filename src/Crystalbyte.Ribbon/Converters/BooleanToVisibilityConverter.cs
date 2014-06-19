using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Crystalbyte.Converters {
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter {

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var boolean = (bool)value;
            var param = parameter as string;
            if (string.IsNullOrWhiteSpace(param)) {
                return boolean ? Visibility.Visible : Visibility.Collapsed;
            }

            return boolean ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }

        #endregion
    }
}
