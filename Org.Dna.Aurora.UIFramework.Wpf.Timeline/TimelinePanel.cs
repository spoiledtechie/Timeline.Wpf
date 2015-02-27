using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline
{

    /// <summary>
    /// Represent the base panel for all timeline items panel.
    /// </summary>
    public abstract class TimelinePanel : Panel
    {

        static TimelinePanel()
        {
            MinimumMillisecondsProperty = Timeline.MinimumMillisecondsProperty.AddOwner(typeof(TimelinePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
            MaximumMillisecondsProperty = Timeline.MaximumMillisecondsProperty.AddOwner(typeof(TimelinePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
            TickMillisecondsProperty = Timeline.TickMillisecondsProperty.AddOwner(typeof(TimelinePanel), new FrameworkPropertyMetadata(Timeline.TickMillisecondsDefaultValue, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public TimelinePanel()
        {

        }

        #region DP

        public static readonly DependencyProperty MaximumMillisecondsProperty;
        public static readonly DependencyProperty MinimumMillisecondsProperty;
        public static readonly DependencyProperty TickMillisecondsProperty;


        public static readonly DependencyProperty StartMillisecondsProperty =
                DependencyProperty.RegisterAttached("StartMilliseconds", typeof(long?), typeof(TimelinePanel),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public static readonly DependencyProperty EndMillisecondsProperty =
                DependencyProperty.RegisterAttached("EndMilliseconds", typeof(long?), typeof(TimelinePanel),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public static readonly DependencyProperty RowIndexProperty =
        DependencyProperty.RegisterAttached("RowIndex", typeof(int), typeof(TimelinePanel),
        new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        private static readonly DependencyPropertyKey ActualRowIndexPropertyKey =
              DependencyProperty.RegisterAttachedReadOnly("ActualRowIndex", typeof(int), typeof(TimelinePanel),
        new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty ActualRowIndexProperty =
            ActualRowIndexPropertyKey.DependencyProperty;

        public static int GetActualRowIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(ActualRowIndexProperty);
        }

        protected static void SetActualRowIndex(DependencyObject obj, int value)
        {
            obj.SetValue(ActualRowIndexPropertyKey, value);
        }

        protected static void ClearActualRowIndex(DependencyObject obj)
        {
            obj.ClearValue(ActualRowIndexPropertyKey);
        }

        public static long? GetStartMilliseconds(DependencyObject obj)
        {
            // Return start time and if null end time.
            return (long?)obj.GetValue(StartMillisecondsProperty) ??
                            (long?)obj.GetValue(EndMillisecondsProperty);
        }

        public static void SetStartDate(DependencyObject obj, DateTime? value)
        {
            obj.SetValue(StartMillisecondsProperty, value);
        }

        public static long? GetEndMilliseconds(DependencyObject obj)
        {
            // Return end time, and if null start time.
            return (long?)obj.GetValue(EndMillisecondsProperty) ??
                            (long?)obj.GetValue(StartMillisecondsProperty);
        }

        public static void SetEndDate(DependencyObject obj, DateTime? value)
        {
            obj.SetValue(EndMillisecondsProperty, value);
        }

        public static int GetRowIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(RowIndexProperty);
        }

        public static void SetRowIndex(DependencyObject obj, int value)
        {
            obj.SetValue(RowIndexProperty, value);
        }

        public Nullable<long> MaximumMilliseconds
        {
            get { return (Nullable<long>)GetValue(MaximumMillisecondsProperty); }
            set { SetValue(MaximumMillisecondsProperty, value); }
        }

        public Nullable<long> MinimumMilliseconds
        {
            get { return (Nullable<long>)GetValue(MinimumMillisecondsProperty); }
            set { SetValue(MinimumMillisecondsProperty, value); }
        }

        public int RowIndex
        {
            get { return (int)GetValue(RowIndexProperty); }
            set { SetValue(RowIndexProperty, value); }
        }

        public long TickTimeSpan
        {
            get { return (long)GetValue(TickMillisecondsProperty); }
            set { SetValue(TickMillisecondsProperty, value); }
        }

        protected TimelineItem FindChildByDaya(object dep)
        {
            foreach (UIElement element in Children)
            {
                TimelineItem priorItem = element as TimelineItem;
                if (priorItem != null)
                {
                    if (object.Equals(priorItem.DataContext, dep)) return priorItem;
                }
            }

            return null;
        }

        protected double PixelsPerTick
        {
            get
            {
                return Timeline.GetPixelsPerTick(this);
            }
        }


        /// <summary>
        /// Get or set the height of each row in the timeline panel
        /// </summary>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeightProperty =
                DependencyProperty.Register("RowHeight", typeof(double), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(20D, FrameworkPropertyMetadataOptions.AffectsMeasure));


        /// <summary>
        /// Get or set the vertical margin between two rows.
        /// </summary>
        public double RowVerticalMargin
        {
            get { return (double)GetValue(RowVerticalMarginProperty); }
            set { SetValue(RowVerticalMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowVerticalMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowVerticalMarginProperty =
                DependencyProperty.Register("RowVerticalMargin", typeof(double), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(5D, FrameworkPropertyMetadataOptions.AffectsMeasure));


        protected double MinimumDisplayMultiplier
        {
            get { return 30; }
        }

        #endregion

        protected int rowsCount;

        protected int RowsCount
        {
            get { return rowsCount; }
            set { rowsCount = value; }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                ArrangeChild(child);
            }

            return finalSize;
        }

        protected virtual void ArrangeChild(UIElement child)
        {
            int rowIndex = GetActualRowIndex(child);
            ArrangeChild(child, rowIndex);
        }

        protected virtual void ArrangeChild(UIElement child, int rowIndex)
        {
            Rect childRect = CalcChildRect((TimelineItem)child, rowIndex);
            if (!childRect.IsEmpty)
            {
                child.Arrange(childRect);
            }
        }

        protected Rect CalcChildRect(TimelineItem child, int childRowIndex)
        {

            if (/*child.Visibility == Visibility.Collapsed || */child.IsCollapsed)
            {
                return Rect.Empty;
            }

            Rect desiredSize;
            if (child.IsDisplayAsZero)
            {
                desiredSize = CalcChildRectForDisplayAsZero(child, childRowIndex);
            }
            else
            {
                desiredSize = CalcChildRectByDuration(child, childRowIndex);
            }

            return desiredSize;
        }

        protected bool ShouldDisplayAsZero(TimelineItem child)
        {
            long? childStartDate = GetStartMilliseconds(child);
            long? childEndDate = GetEndMilliseconds(child);

            bool displayAsZero;
            if (!(childStartDate.HasValue && childEndDate.HasValue))
            {
                displayAsZero = false;
            }
            else if (childStartDate.HasValue && childEndDate.HasValue)
            {
                long duration = childEndDate.Value - childStartDate.Value;
                displayAsZero = (duration <
                    TickTimeSpan * MinimumDisplayMultiplier);
            }
            else
            {
                // Only start or end time exist, display as zero.
                displayAsZero = true;
            }

            return displayAsZero;
        }

        private Rect CalcChildRectForDisplayAsZero(TimelineItem child, int childRowIndex)
        {
            long? childStartDate = GetStartMilliseconds(child);
            long? childEndDate = GetEndMilliseconds(child);
            long centerDate;

            if (MinimumMilliseconds == null || MaximumMilliseconds == null)
            {
                return Rect.Empty;
            }

            if (!(childStartDate.HasValue && childEndDate.HasValue))
            {
                // Patch?
                centerDate = MinimumMilliseconds.Value;
            }
            else if (childStartDate.HasValue && childEndDate.HasValue)
            {
                long duration = childEndDate.Value - childStartDate.Value;
                centerDate = childStartDate.Value + (duration / 2);
            }
            else if (childStartDate.HasValue)
            {
                centerDate = childStartDate.Value;
            }
            else if (childEndDate.HasValue)
            {
                centerDate = childEndDate.Value;
            }
            else
            {
                throw new Exception("Invalid state of TimelineItem values");
            }

            double offset = centerDate - MinimumMilliseconds.Value * PixelsPerTick;
            double width = 20;
            double childTopOffset = childRowIndex * RowHeight +
                childRowIndex * RowVerticalMargin;

            return new Rect(offset, childTopOffset, width, RowHeight);

        }

        private Rect CalcChildRectByDuration(TimelineItem child, int childRowIndex)
        {
            if (GetStartMilliseconds(child) == null || GetEndMilliseconds(child) == null || MinimumMilliseconds == null)
            {
                return Rect.Empty;
            }

            long childStartDate = GetStartMilliseconds(child) ?? MinimumMilliseconds.Value;
            long childEndDate = GetEndMilliseconds(child) ?? MinimumMilliseconds.Value;

            if (childEndDate < childStartDate)
            {
                childEndDate = childStartDate;
            }
            long childDuration = childEndDate - childStartDate;

            double offset = childStartDate - MinimumMilliseconds.Value * PixelsPerTick;
            double width = childDuration * PixelsPerTick;
            double childTopOffset = childRowIndex * RowHeight +
                childRowIndex * RowVerticalMargin;
            return new Rect(offset, childTopOffset, width, RowHeight);
        }

    }
}
