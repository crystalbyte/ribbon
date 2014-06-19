using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Crystalbyte.Converters {
    [ValueConversion(typeof(object), typeof(Visibility))]
    public sealed class NullToVisibleConverter : IValueConverter {

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }

        #endregion
    }
}
