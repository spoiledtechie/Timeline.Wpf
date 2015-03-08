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

            MinimumTickProperty = Timeline.MinimumTickProperty.AddOwner(typeof(TimelineRulerControl), new FrameworkPropertyMetadata(default(long), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure, TimeframeChanged));
            MaximumTickProperty = Timeline.MaximumTickProperty.AddOwner(typeof(TimelineRulerControl), new FrameworkPropertyMetadata(default(long), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure, TimeframeChanged));
            TickTimeSpanProperty = Timeline.TickTimeSpanProperty.AddOwner(typeof(TimelineRulerControl), new FrameworkPropertyMetadata(Timeline.TickTimeSpanDefaultValue, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, TicksTimeSpanChanged, TicksTimeSpanCoerce));
        }

        public static readonly DependencyProperty MaximumTickProperty;
        public static readonly DependencyProperty MinimumTickProperty;
        public static readonly DependencyProperty TickTimeSpanProperty;

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

        public TimeSpan TickTimeSpan
        {
            get { return (TimeSpan)GetValue(TickTimeSpanProperty); }
            set { SetValue(TickTimeSpanProperty, value); }
        }



        public TimeSpan BlockTimeSpan
        {
            get { return (TimeSpan)GetValue(BlockTimeSpanProperty); }
            set { SetValue(BlockTimeSpanProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlockTimeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlockTimeSpanProperty =
          DependencyProperty.Register("BlockTimeSpan", typeof(TimeSpan), typeof(TimelineRulerControl), new FrameworkPropertyMetadata(TimeSpan.FromDays(1)));


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

                double pixelPerTick = 1D / TickTimeSpan.Ticks;
                DpiPerBlock = pixelPerTick * EffectiveBlockTicks;

                UpdateRulerBlocks();
            }
        }

        private long EffectiveBlockTicks
        {
            get
            {
                if (effectiveBlockSpan == null)
                {
                    double pixelPerTick = 1D / TickTimeSpan.Ticks;
                    long ticks = (long)(150 / pixelPerTick);
                    effectiveBlockSpan = ticks;

                    // NACHON: Added to show nicer timelineRuler                     
                    foreach (var blockFactor in BlockFactors())
                    {
                        if (TickTimeSpan.Ticks >= blockFactor)
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
            yield return TimeSpan.FromSeconds(3).Ticks;
            yield return TimeSpan.FromSeconds(7).Ticks;
            yield return TimeSpan.FromSeconds(15).Ticks;
            yield return TimeSpan.FromSeconds(30).Ticks;
            yield return TimeSpan.FromSeconds(60).Ticks;
            yield return TimeSpan.FromMinutes(3).Ticks;
            yield return TimeSpan.FromMinutes(7).Ticks;
            yield return TimeSpan.FromMinutes(15).Ticks;
            yield return TimeSpan.FromMinutes(30).Ticks;
            yield return TimeSpan.FromHours(1).Ticks;
            yield return TimeSpan.FromHours(2).Ticks;
            yield return TimeSpan.FromHours(4).Ticks;
            yield return TimeSpan.FromHours(8).Ticks;
            yield return TimeSpan.FromHours(16).Ticks;
            yield return TimeSpan.FromDays(1).Ticks;
            yield return TimeSpan.FromDays(2).Ticks;
            yield return TimeSpan.FromDays(3).Ticks;
            yield return TimeSpan.FromDays(4).Ticks;
            yield return TimeSpan.FromDays(5).Ticks;
            yield return TimeSpan.FromDays(6).Ticks;
            yield return TimeSpan.FromDays(7).Ticks;
            yield return TimeSpan.FromDays(30).Ticks;
        }

        private readonly List<RulerBlockItem> EmptyRulerBlockList = new List<RulerBlockItem>();

        private void UpdateRulerBlocks()
        {
            if (MinimumTick == null || MaximumTick == null)
            {
                // Clear all block
                RulerBlocks = EmptyRulerBlockList;
            }
            else
            {
                long timeframe = MaximumTick.Value - MinimumTick.Value;
                System.Diagnostics.Debug.WriteLine("timeframe " +  timeframe);
                int totalBlocks = (int)Math.Ceiling((double)(timeframe / EffectiveBlockTicks));
                System.Diagnostics.Debug.WriteLine("fefTicks " + EffectiveBlockTicks);
                System.Diagnostics.Debug.WriteLine("totalBlocks " + totalBlocks);
                System.Diagnostics.Debug.WriteLine("devidied " + (double)(timeframe / EffectiveBlockTicks));
                System.Diagnostics.Debug.WriteLine("math " + Math.Ceiling((double)(timeframe / EffectiveBlockTicks)));
                
                
                
                totalBlocks++;

                if (totalBlocks > 2000)
                {
                    Debug.WriteLine("Because we do not support virtualization for TimelineRulerControl yet the number of blocks was limit to 2000");
                    totalBlocks = 2000;
                }

                List<RulerBlockItem> blocks = new List<RulerBlockItem>();

                long spanFromStart = EffectiveBlockTicks;
                long prev = MinimumTick.Value;

                for (int blockIdx = 0; blockIdx < totalBlocks; blockIdx++)
                {
                    long current = MinimumTick.Value + spanFromStart;

                    RulerBlockItem block = new RulerBlockItem();
                    block.Start = prev;
                    block.Span = EffectiveBlockTicks;
                    //block.Text = prev.ToString();
                    blocks.Add(block);

                    prev = current;
                    spanFromStart = spanFromStart + EffectiveBlockTicks;
                }

                RulerBlocks = blocks;
                System.Diagnostics.Debug.WriteLine(blocks.Count);
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
            long span = (long)values[1];

            double width = span * pixelPerTick;

            return width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
