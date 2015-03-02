using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline
{
    public class Timeline : DependencyObject
    {


        public static Nullable<long> GetMinimumTick(DependencyObject obj)
        {
            return (Nullable<long>)obj.GetValue(MinimumTickProperty);
        }

        public static void SetMinimumTick(DependencyObject obj, Nullable<long> value)
        {
            obj.SetValue(MinimumTickProperty, value);
        }

        // Using a DependencyProperty as the backing store for MinimumDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumTickProperty = DependencyProperty.RegisterAttached("MinimumTick", typeof(Nullable<long>), typeof(Timeline), new FrameworkPropertyMetadata(default(long), FrameworkPropertyMetadataOptions.Inherits));



        public static Nullable<long> GetMaximumTick(DependencyObject obj)
        {
            return (Nullable<long>)obj.GetValue(MaximumTickProperty);
        }

        public static void SetMaximumTick(DependencyObject obj, long value)
        {
            obj.SetValue(MaximumTickProperty, value);
        }

        // Using a DependencyProperty as the backing store for MaximumDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumTickProperty = DependencyProperty.RegisterAttached("MaximumTick", typeof(Nullable<long>), typeof(Timeline), new FrameworkPropertyMetadata(default(long), FrameworkPropertyMetadataOptions.Inherits));



        public static readonly TimeSpan TickTimeSpanDefaultValue = TimeSpan.FromDays(1);

        public static TimeSpan GetTickTimeSpan(DependencyObject obj)
        {
            return (TimeSpan)obj.GetValue(TickTimeSpanProperty);
        }

        public static void SetTickTimeSpan(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(TickTimeSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for TickTimeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickTimeSpanProperty =
                DependencyProperty.RegisterAttached("TickTimeSpan", typeof(TimeSpan),
                typeof(Timeline), new FrameworkPropertyMetadata(TickTimeSpanDefaultValue, FrameworkPropertyMetadataOptions.Inherits));


        public static double GetPixelsPerTick(DependencyObject obj)
        {
            TimeSpan tickTimeSpan = GetTickTimeSpan(obj);
            if (tickTimeSpan.Ticks == 0) return 1;
            return ((double)1 / tickTimeSpan.Ticks);
        }

        public static double TickToOffset(long current, DependencyObject owner)
        {
            Nullable<long> min = GetMinimumTick(owner);
            Nullable<long> max = GetMaximumTick(owner);
            double pixelsPerTick = GetPixelsPerTick(owner);

            if (min == null || max == null)
            {
                return -1;
            }

            else if (current < min || current > max)
            {
                return -1;
            }

            double offset = (current - min.Value) * pixelsPerTick;
            return offset;
        }

        public static long OffsetToTick(double offset, DependencyObject owner)
        {
            Nullable<long> min = GetMinimumTick(owner);
            Nullable<long> max = GetMaximumTick(owner);
            double pixelsPerTick = GetPixelsPerTick(owner);

            if (min == null || max == null)
            {
                return 0;
            }

            long date = min.Value + ((long)(offset / pixelsPerTick));

            if (date < min || date > max)
            {
                return 0;
            }

            return date;
        }
    }
}
