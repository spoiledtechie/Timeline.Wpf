using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline
{
    public class Timeline : DependencyObject
    {


        public static Nullable<long> GetMinimumMilliseconds(DependencyObject obj)
        {
            return (Nullable<long>)obj.GetValue(MinimumMillisecondsProperty);
        }

        public static void SetMinimumMilliseconds(DependencyObject obj, Nullable<long> value)
        {
            obj.SetValue(MinimumMillisecondsProperty, value);
        }

        // Using a DependencyProperty as the backing store for MinimumDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumMillisecondsProperty =
                DependencyProperty.RegisterAttached("MinimumMilliseconds", typeof(Nullable<long>),
                typeof(Timeline), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));



        public static Nullable<long> GetMaximumMilliseconds(DependencyObject obj)
        {
            return (Nullable<long>)obj.GetValue(MaximumMillisecondsProperty);
        }

        public static void SetMaximumMilliseconds(DependencyObject obj, long value)
        {
            obj.SetValue(MaximumMillisecondsProperty, value);
        }

        // Using a DependencyProperty as the backing store for MaximumDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumMillisecondsProperty =
                DependencyProperty.RegisterAttached("MaximumMilliseconds", typeof(Nullable<long>),
                typeof(Timeline), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));



        public static readonly long TickMillisecondsDefaultValue = TimeSpan.FromDays(1).Milliseconds;

        public static long GetTickMilliseconds(DependencyObject obj)
        {
            return (long)obj.GetValue(TickMillisecondsProperty);
        }

        public static void SetTickTimeSpan(DependencyObject obj, long value)
        {
            obj.SetValue(TickMillisecondsProperty, value);
        }

        // Using a DependencyProperty as the backing store for TickTimeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickMillisecondsProperty =
                DependencyProperty.RegisterAttached("TickMilliseconds", typeof(long),
                typeof(Timeline), new FrameworkPropertyMetadata(TickMillisecondsDefaultValue, FrameworkPropertyMetadataOptions.Inherits));


        public static double GetPixelsPerTick(DependencyObject obj)
        {
            long tickMilliseconds = GetTickMilliseconds(obj);
            if (tickMilliseconds == 0) return 1;
            return ((double)1 / tickMilliseconds);
        }

        public static double DateToOffset(long currentMilliseconds, DependencyObject owner)
        {
            Nullable<long> min = GetMinimumMilliseconds(owner);
            Nullable<long> max = GetMaximumMilliseconds(owner);
            double pixelsPerTick = GetPixelsPerTick(owner);

            if (min == null || max == null)
            {
                return -1;
            }

            else if (currentMilliseconds < min || currentMilliseconds > max)
            {
                return -1;
            }

            double offset = currentMilliseconds - min.Value * pixelsPerTick;
            return offset;
        }

        public static long OffsetToMilliseconds(double offset, DependencyObject owner)
        {
            Nullable<long> min = GetMinimumMilliseconds(owner);
            Nullable<long> max = GetMaximumMilliseconds(owner);
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
