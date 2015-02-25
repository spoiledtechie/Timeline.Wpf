using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {

	public class ConnectionsPresenter : ItemsControl {

		static ConnectionsPresenter() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectionsPresenter),
				new FrameworkPropertyMetadata(typeof(ConnectionsPresenter)));

			ItemContainerStyleSelectorProperty.AddOwner(typeof(ConnectionsPresenter),
				new FrameworkPropertyMetadata(null, CoerceItemContainerStyleSelector));

		}

		private static object CoerceItemContainerStyleSelector(DependencyObject d, object baseValue) {
			object value = baseValue ?? new StyleSelectorByItemType();
			return value;
		}

		public ConnectionsPresenter() {
		}

		public TimelineControl Timeline { get; set; }

		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is Connection;
		}

		protected override System.Windows.DependencyObject GetContainerForItemOverride() {
			return new Connection();
		}

		protected override void PrepareContainerForItemOverride(
			DependencyObject element, object item) {

			base.PrepareContainerForItemOverride(element, item);

			if (Timeline != null) {
				object fromItem = GetFromItem(element);
				object toItem = GetToItem(element);

				if (fromItem != null && toItem != null) {
					TimelineItem tlFromItem = Timeline.ContainerFromItem(fromItem);
					TimelineItem tlToItem = Timeline.ContainerFromItem(toItem);

					if (tlFromItem != null && tlToItem != null) {

						Connection conn = (Connection)element;
						conn.SourceItem = tlFromItem;
						conn.SinkItem = tlToItem;
					}
				}
			}
		}

		#region Attached Properties



		public static object GetFromItem(DependencyObject obj) {
			return (object)obj.GetValue(FromItemProperty);
		}

		public static void SetFromItem(DependencyObject obj, object value) {
			obj.SetValue(FromItemProperty, value);
		}

		// Using a DependencyProperty as the backing store for FromItem.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FromItemProperty =
				DependencyProperty.RegisterAttached("FromItem", typeof(object), typeof(ConnectionsPresenter), new FrameworkPropertyMetadata(null));



		public static object GetToItem(DependencyObject obj) {
			return (object)obj.GetValue(ToItemProperty);
		}

		public static void SetToItem(DependencyObject obj, object value) {
			obj.SetValue(ToItemProperty, value);
		}

		// Using a DependencyProperty as the backing store for ToItem.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ToItemProperty =
				DependencyProperty.RegisterAttached("ToItem", typeof(object), typeof(ConnectionsPresenter), new FrameworkPropertyMetadata(null));




		#endregion
	}

}
