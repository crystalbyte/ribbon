using System;
using System.Globalization;
using System.Windows.Data;

namespace Crystalbyte.Converters {
    [ValueConversion(typeof(double), typeof(double))]
    public sealed class DoubleMultiplier : IValueConverter {

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var multiplier = double.Parse((string)parameter);
            var basis = (double) value;
            if (double.IsNaN(basis)) {
                return value;
            }
            return basis*multiplier;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return Binding.DoNothing;
        }

        #endregion
    }
}
