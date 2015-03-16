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
            MinimumTickProperty = Timeline.MinimumTickProperty.AddOwner(typeof(TimelinePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
            MaximumTickProperty = Timeline.MaximumTickProperty.AddOwner(typeof(TimelinePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
            TickTimeSpanProperty = Timeline.TickTimeSpanProperty.AddOwner(typeof(TimelinePanel), new FrameworkPropertyMetadata(Timeline.TickTimeSpanDefaultValue, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public TimelinePanel()
        {

        }

        #region DP

        public static readonly DependencyProperty MaximumTickProperty;
        public static readonly DependencyProperty MinimumTickProperty;
        public static readonly DependencyProperty TickTimeSpanProperty;


        public static readonly DependencyProperty StartTickProperty = DependencyProperty.RegisterAttached("StartTick", typeof(long?), typeof(TimelinePanel), new FrameworkPropertyMetadata(default(long), FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public static readonly DependencyProperty EndTickProperty = DependencyProperty.RegisterAttached("EndTick", typeof(long?), typeof(TimelinePanel), new FrameworkPropertyMetadata(default(long), FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public static readonly DependencyProperty RowIndexProperty = DependencyProperty.RegisterAttached("RowIndex", typeof(int), typeof(TimelinePanel), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        private static readonly DependencyPropertyKey ActualRowIndexPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ActualRowIndex", typeof(int), typeof(TimelinePanel), new FrameworkPropertyMetadata(0));

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

        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected static void ClearActualRowIndex(DependencyObject obj)
        {
            obj.ClearValue(ActualRowIndexPropertyKey);
        }

        public static long? GetStartTick(DependencyObject obj)
        {
            // Return start time and if null end time.
            return (long?)obj.GetValue(StartTickProperty) ??
                            (long?)obj.GetValue(EndTickProperty);
        }

        public static void SetStartDate(DependencyObject obj, DateTime? value)
        {
            obj.SetValue(StartTickProperty, value);
        }

        public static long? GetEndTick(DependencyObject obj)
        {
            // Return end time, and if null start time.
            return (long?)obj.GetValue(EndTickProperty) ?? (long?)obj.GetValue(StartTickProperty);
        }

        public static void SetEndDate(DependencyObject obj, DateTime? value)
        {
            obj.SetValue(EndTickProperty, value);
        }

        public static int GetRowIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(RowIndexProperty);
        }

        public static void SetRowIndex(DependencyObject obj, int value)
        {
            obj.SetValue(RowIndexProperty, value);
        }

        public Nullable<long> MaximumTick
        {
            get { return (Nullable<long>)GetValue(MaximumTickProperty); }
            set { SetValue(MaximumTickProperty, value); }
        }

        public Nullable<long> MinimumTick
        {
            get { return (Nullable<long>)GetValue(MinimumTickProperty); }
            set { SetValue(MinimumTickProperty, value); }
        }

        public int RowIndex
        {
            get { return (int)GetValue(RowIndexProperty); }
            set { SetValue(RowIndexProperty, value); }
        }

        public TimeSpan TickTimeSpan
        {
            get { return (TimeSpan)GetValue(TickTimeSpanProperty); }
            set { SetValue(TickTimeSpanProperty, value); }
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
            long? childStartTick = GetStartTick(child);
            long? childEndTick = GetEndTick(child);

            bool displayAsZero;
            if (!(childStartTick.HasValue && childEndTick.HasValue))
            {
                displayAsZero = false;
            }
            else if (childStartTick.HasValue && childEndTick.HasValue)
            {
                long duration = childEndTick.Value - childStartTick.Value;
                displayAsZero = (duration <
                    TickTimeSpan.Ticks * MinimumDisplayMultiplier);
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
            long? childStartTick = GetStartTick(child);
            long? childEndTick = GetEndTick(child);
            long centerTick;

            if (MinimumTick == null || MaximumTick == null)
            {
                return Rect.Empty;
            }

            if (!(childStartTick.HasValue && childEndTick.HasValue))
            {
                // Patch?
                centerTick = MinimumTick.Value;
            }
            else if (childStartTick.HasValue && childEndTick.HasValue)
            {
                long duration = childEndTick.Value - childStartTick.Value;
                centerTick = childStartTick.Value + (duration / 2);
            }
            else if (childStartTick.HasValue)
            {
                centerTick = childStartTick.Value;
            }
            else if (childEndTick.HasValue)
            {
                centerTick = childEndTick.Value;
            }
            else
            {
                throw new Exception("Invalid state of TimelineItem values");
            }

            double offset = (centerTick - MinimumTick.Value) * PixelsPerTick;
            double width = 20;
            double childTopOffset = childRowIndex * RowHeight + childRowIndex * RowVerticalMargin;

            return new Rect(offset, childTopOffset, width, RowHeight);

        }

        private Rect CalcChildRectByDuration(TimelineItem child, int childRowIndex)
        {
            if (GetStartTick(child) == null || GetEndTick(child) == null || MinimumTick == null)
            {
                return Rect.Empty;
            }

            long childStartTick = GetStartTick(child) ?? MinimumTick.Value;
            long childEndTick = GetEndTick(child) ?? MinimumTick.Value;

            if (childEndTick < childStartTick)
            {
                childEndTick = childStartTick;
            }
            long childDuration = childEndTick - childStartTick;

            double offset = (childStartTick - MinimumTick.Value) * PixelsPerTick;
            double width = childDuration * PixelsPerTick;
            double childTopOffset = childRowIndex * RowHeight + childRowIndex * RowVerticalMargin;

            return new Rect(offset, childTopOffset, width, RowHeight);
        }

    }
}
