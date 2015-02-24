using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Timeline.Wpf.MTest {
	public class DoubleToTimeSpanFromMinutesConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double minutes = (double)value;
			return TimeSpan.FromSeconds(minutes);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			TimeSpan ts = (TimeSpan)value;
			return ts.TotalSeconds; 
		}

	}
}
