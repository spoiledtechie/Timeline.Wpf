using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline
{

    /// <summary>
    /// Timeline Ruler control display ruler of dates.
    /// </summary>
    public class TimelineRulerControl : Control
    {

        static TimelineRulerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineRulerControl),
              new FrameworkPropertyMetadata(typeof(TimelineRulerControl)));

            MinimumMillisecondsProperty = Timeline.MinimumMillisecondsProperty.AddOwner(typeof(TimelineRulerControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure, TimeframeChanged));
            MaximumMillisecondsProperty = Timeline.MaximumMillisecondsProperty.AddOwner(typeof(TimelineRulerControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure, TimeframeChanged));
            TickMillisecondsProperty = Timeline.TickMillisecondsProperty.AddOwner(typeof(TimelineRulerControl), new FrameworkPropertyMetadata(Timeline.TickMillisecondsDefaultValue, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, TicksTimeSpanChanged, TicksTimeSpanCoerce));
        }

        public static readonly DependencyProperty MaximumMillisecondsProperty;
        public static readonly DependencyProperty MinimumMillisecondsProperty;
        public static readonly DependencyProperty TickMillisecondsProperty;

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

        public long TickMilliseconds
        {
            get { return (long)GetValue(TickMillisecondsProperty); }
            set { SetValue(TickMillisecondsProperty, value); }
        }



        public long BlockMilliseconds
        {
            get { return (long)GetValue(BlockMillisecondsProperty); }
            set { SetValue(BlockMillisecondsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlockTimeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlockMillisecondsProperty =
          DependencyProperty.Register("BlockMilliseconds", typeof(long), typeof(TimelineRulerControl), new FrameworkPropertyMetadata(TimeSpan.FromDays(1).Milliseconds));


        private static readonly DependencyPropertyKey RulerBlocksPropertyKey =
        DependencyProperty.RegisterReadOnly("RulerBlocks", typeof(IList<RulerBlockItem>),
        typeof(TimelineRulerControl), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty RulerBlocksProperty =
        RulerBlocksPropertyKey.DependencyProperty;

        public IList<RulerBlockItem> RulerBlocks
        {
            get { return (IList<RulerBlockItem>)GetValue(RulerBlocksProperty); }
            private set { SetValue(RulerBlocksPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey DpiPerBlockPropertyKey =
        DependencyProperty.RegisterReadOnly("DpiPerBlock", typeof(double),
        typeof(TimelineRulerControl), new FrameworkPropertyMetadata(1D));

        public static readonly DependencyProperty DpiPerBlockProperty =
        DpiPerBlockPropertyKey.DependencyProperty;

        public double DpiPerBlock
        {
            get { return (double)GetValue(DpiPerBlockProperty); }
            private set { SetValue(DpiPerBlockPropertyKey, value); }
        }

        public TimelineRulerControl()
        {

        }

        private bool blockUpdatePending = false;

        private static void TimeframeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimelineRulerControl ruler = (TimelineRulerControl)o;
            ruler.blockUpdatePending = true;
            //ruler.UpdateRulerBlocks();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            MeasureEffectiveBlockSpan();

            if (blockUpdatePending)
            {
                blockUpdatePending = false;
                UpdateRulerBlocks();
            }

            return base.MeasureOverride(constraint);
        }

        private ScrollViewer _headerSV;
        private ScrollViewer _mainSV;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer viewer = this._headerSV;
            this._headerSV = base.Parent as ScrollViewer;
            if (viewer != this._headerSV)
            {
                if (viewer != null)
                {
                    viewer.ScrollChanged -= new ScrollChangedEventHandler(this.OnHeaderScrollChanged);
                }
                if (this._headerSV != null)
                {
                    this._headerSV.ScrollChanged += new ScrollChangedEventHandler(this.OnHeaderScrollChanged);
                }
            }
            ScrollViewer viewer2 = this._mainSV;
            this._mainSV = base.TemplatedParent as ScrollViewer;
            if (viewer2 != this._mainSV)
            {
                if (viewer2 != null)
                {
                    viewer2.ScrollChanged -= new ScrollChangedEventHandler(this.OnMasterScrollChanged);
                }
                if (this._mainSV != null)
                {
                    this._mainSV.ScrollChanged += new ScrollChangedEventHandler(this.OnMasterScrollChanged);
                }
            }

        }

        private void OnHeaderScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if ((this._mainSV != null) && (this._headerSV == e.OriginalSource))
            {
                this._mainSV.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }

        private void OnMasterScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if ((this._headerSV != null) && (this._mainSV == e.OriginalSource))
            {
                this._headerSV.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }

        private static object TicksTimeSpanCoerce(DependencyObject d, object baseValue)
        {
            TimeSpan ts = (TimeSpan)baseValue;
            if (ts.Ticks <= 0)
            {
                return TimeSpan.FromMinutes(5);
            }
            else
            {
                return baseValue;
            }
        }

        private static void TicksTimeSpanChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimelineRulerControl ruler = (TimelineRulerControl)o;
            ruler.updateDpiPerBlock = true;
        }

        private Nullable<long> effectiveBlockSpan;
        private bool updateDpiPerBlock = false;

        private void MeasureEffectiveBlockSpan()
        {
            if (updateDpiPerBlock)
            {
                updateDpiPerBlock = false;
                effectiveBlockSpan = null;

                double pixelPerTick = 1D / TickMilliseconds;
                DpiPerBlock = pixelPerTick * EffectiveBlockMilliseconds;

                UpdateRulerBlocks();
            }
        }

        private long EffectiveBlockMilliseconds
        {
            get
            {
                if (effectiveBlockSpan == null)
                {
                    double pixelPerTick = 1D / TickMilliseconds;
                    long ticks = (long)(150 / pixelPerTick);
                    effectiveBlockSpan = ticks;

                    // NACHON: Added to show nicer timelineRuler                     
                    foreach (var blockFactor in BlockFactors())
                    {
                        if (TickMilliseconds <= blockFactor)
                        {
                            effectiveBlockSpan = blockFactor;
                            break;
                        }
                    }

                    // End NACHON
                }

                return effectiveBlockSpan.Value;
            }
        }

        private IEnumerable<long> BlockFactors()
        {
            yield return 15 * 60000;
            yield return 30 * 60000;
            yield return 1 * 60 * 60000;
            yield return 2 * 60 * 60000;
            yield return 4 * 60 * 60000;
            yield return 8 * 60 * 60000;
            yield return 16 * 60 * 60000;
            yield return 1 * 24 * 60 * 60000;
            yield return 2 * 24 * 60 * 60000;
            yield return 3 * 24 * 60 * 60000;
            yield return 4 * 24 * 60 * 60000;
            yield return 5 * 24 * 60 * 60000;
            yield return 6 * 24 * 60 * 60000;
            yield return 7 * 24 * 60 * 60000;
        }

        private readonly List<RulerBlockItem> EmptyRulerBlockList = new List<RulerBlockItem>();

        private void UpdateRulerBlocks()
        {
            if (MinimumMilliseconds == null || MaximumMilliseconds == null)
            {
                // Clear all block
                RulerBlocks = EmptyRulerBlockList;
            }
            else
            {
                long timeframe = MaximumMilliseconds.Value - MinimumMilliseconds.Value;
                int totalBlocks = (int)Math.Ceiling((double)(timeframe / EffectiveBlockMilliseconds));
                totalBlocks++;

                if (totalBlocks > 2000)
                {
                    Debug.WriteLine("Because we do not support virtualization for TimelineRulerControl yet the number of blocks was limit to 2000");
                    totalBlocks = 2000;
                }

                List<RulerBlockItem> blocks = new List<RulerBlockItem>();

                long spanFromStart = EffectiveBlockMilliseconds;
                long prev = MinimumMilliseconds.Value;

                for (int blockIdx = 0; blockIdx < totalBlocks; blockIdx++)
                {
                    long current = MinimumMilliseconds.Value + spanFromStart;

                    RulerBlockItem block = new RulerBlockItem();
                    block.Start = prev;
                    block.Span = EffectiveBlockMilliseconds;
                    //block.Text = prev.ToString();
                    blocks.Add(block);

                    prev = current;
                    spanFromStart = spanFromStart + EffectiveBlockMilliseconds;
                }

                RulerBlocks = blocks;
            }
        }

    }

    public class RulerBlockSizeConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 2 ||
              values[0] == DependencyProperty.UnsetValue ||
              values[1] == DependencyProperty.UnsetValue)
            {
                return 0.0d;
            }

            TimeSpan tickTimeSpan = (TimeSpan)values[0];
            double pixelPerTick = 1D / tickTimeSpan.Ticks;
            TimeSpan span = (TimeSpan)values[1];

            double width = span.Ticks * pixelPerTick;

            return width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
