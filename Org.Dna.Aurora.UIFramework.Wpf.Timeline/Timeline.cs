using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {
	public class Timeline : DependencyObject {


		public static Nullable<DateTime> GetMinimumDate(DependencyObject obj) {
			return (Nullable<DateTime>)obj.GetValue(MinimumDateProperty);
		}

		public static void SetMinimumDate(DependencyObject obj, Nullable<DateTime> value) {
			obj.SetValue(MinimumDateProperty, value);
		}

		// Using a DependencyProperty as the backing store for MinimumDate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MinimumDateProperty =
				DependencyProperty.RegisterAttached("MinimumDate", typeof(Nullable<DateTime>), 
				typeof(Timeline), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));



		public static Nullable<DateTime> GetMaximumDate(DependencyObject obj) {
			return (Nullable<DateTime>)obj.GetValue(MaximumDateProperty);
		}

		public static void SetMaximumDate(DependencyObject obj, DateTime value) {
			obj.SetValue(MaximumDateProperty, value);
		}

		// Using a DependencyProperty as the backing store for MaximumDate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaximumDateProperty =
				DependencyProperty.RegisterAttached("MaximumDate", typeof(Nullable<DateTime>), 
				typeof(Timeline), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));



		public static readonly TimeSpan TickTimeSpanDefaultValue = TimeSpan.FromDays(1);

		public static TimeSpan GetTickTimeSpan(DependencyObject obj) {
			return (TimeSpan)obj.GetValue(TickTimeSpanProperty);
		}

		public static void SetTickTimeSpan(DependencyObject obj, TimeSpan value) {
			obj.SetValue(TickTimeSpanProperty, value);
		}

		// Using a DependencyProperty as the backing store for TickTimeSpan.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TickTimeSpanProperty =
				DependencyProperty.RegisterAttached("TickTimeSpan", typeof(TimeSpan), 
				typeof(Timeline), new FrameworkPropertyMetadata(TickTimeSpanDefaultValue, FrameworkPropertyMetadataOptions.Inherits));


		public static double GetPixelsPerTick(DependencyObject obj) {
			TimeSpan tickTimeSpan = GetTickTimeSpan(obj);
      if (tickTimeSpan.Ticks == 0) return 1;
			return ((double)1 / tickTimeSpan.Ticks);
		}

		public static double DateToOffset(DateTime current, DependencyObject owner) {
			Nullable<DateTime> min = GetMinimumDate(owner);
			Nullable<DateTime> max = GetMaximumDate(owner);
			double pixelsPerTick = GetPixelsPerTick(owner);

			if (min == null || max == null) {
				return -1;
			}

			else if (current < min || current > max) {
				return -1;
			}

			double offset = (current - min.Value).Ticks * pixelsPerTick;
			return offset;
		}

		public static DateTime OffsetToDate(double offset, DependencyObject owner) {
			Nullable<DateTime> min = GetMinimumDate(owner);
			Nullable<DateTime> max = GetMaximumDate(owner);
			double pixelsPerTick = GetPixelsPerTick(owner);

			if (min == null || max == null) {
				return DateTime.MinValue;
			}

			DateTime date = min.Value.AddTicks((long) (offset / pixelsPerTick));

			if (date < min || date > max) {
				return DateTime.MinValue;
			}

			return date;
		}
	}
}
