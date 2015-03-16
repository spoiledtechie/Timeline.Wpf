using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {

	public class TimelineItem : ContentControl, INotifyPropertyChanged {

		static TimelineItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineItem),
				new FrameworkPropertyMetadata(typeof(TimelineItem)));
		}

		private Connector leftConnector;
		private Connector rightConnector;

		public TimelineItem() {
			
		}

        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
        }


		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsCollapsedProperty =
				DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(TimelineItem), 
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentMeasure ,IsCollapsedChanged));

		private static void IsCollapsedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			TimelineItem item = (TimelineItem)o;
			item.OnPropertyChanged("IsCollapsed");

		}

		/// <summary>
		/// Get or set indication that this timeline item should be display
		/// as zero length.
		/// This can be either because the item length is really zero or
		/// because its length fall below the zoom factor.
		/// </summary>
		public bool IsDisplayAsZero {
			get { return (bool)GetValue(IsDisplayAsZeroProperty); }
			set { SetValue(IsDisplayAsZeroProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsDisplayAsZero.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsDisplayAsZeroProperty =
				DependencyProperty.Register("IsDisplayAsZero", typeof(bool), typeof(TimelineItem), new FrameworkPropertyMetadata(false, IsDisplayAsZeroChanged));

		private static void IsDisplayAsZeroChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			TimelineItem item = (TimelineItem)o;
			item.OnPropertyChanged("IsDisplayAsZero");
		}


		public override void OnApplyTemplate() {
			base.OnApplyTemplate();

			//leftConnector = Template.FindName("PART_Left", this) as Connector;
			//rightConnector = Template.FindName("PART_Right", this) as Connector;
		}

		public Connector LeftConnector {
			get {
				if (leftConnector == null) {
					ApplyTemplate();
					leftConnector = Template.FindName("PART_Left", this) as Connector;
				}
				return leftConnector;
			}
		}

		public Connector RightConnector {
			get {
				if (rightConnector == null) {
					ApplyTemplate();
					rightConnector = Template.FindName("PART_Right", this) as Connector;
				}
				return rightConnector;
			}
		}




		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName) {
			var handlers = PropertyChanged;
			if (handlers != null) {
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

}
