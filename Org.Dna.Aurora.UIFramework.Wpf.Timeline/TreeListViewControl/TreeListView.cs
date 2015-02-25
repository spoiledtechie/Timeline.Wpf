using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline.TreeListViewControl {
  /// <summary>
  /// This code was downloaded from http://blogs.windowsclient.net/ricciolocristian/archive/2008/03/22/a-complete-wpf-treelistview-control.aspx
  /// 
  /// Represent a control which is both TreeView and ListView in one.
  /// It displays hierarchical data as well as tabular data.
  /// </summary>
	public class TreeListView : TreeView {


		static TreeListView() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
		}

		protected override DependencyObject GetContainerForItemOverride() {
			return new TreeListViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is TreeListViewItem;
		}

		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
		}



		public IEnumerable<SortDescription> SortDescriptions {
			get { return (IEnumerable<SortDescription>)GetValue(SortDescriptionsProperty); }
			set { SetValue(SortDescriptionsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SortDescriptions.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SortDescriptionsProperty =
				DependencyProperty.Register("SortDescriptions", typeof(IEnumerable<SortDescription>), typeof(TreeListView), new FrameworkPropertyMetadata(null, SortDescriptionsChanged));


		private static void SortDescriptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var self = (TreeListView)d;
			self.SortDescriptionsChanged(e);
		}

		private void SortDescriptionsChanged(DependencyPropertyChangedEventArgs e) {
			ApplySorting(Items);
		}

		internal void ApplySorting(ItemCollection items) {
			// Set sort descriptions for the current list.
			items.SortDescriptions.Clear();

			if (SortDescriptions != null) {
				foreach (var sortDesc in SortDescriptions) {
					items.SortDescriptions.Add(sortDesc);
				}
			}

			// Set sort description for child lists
			foreach (var item in items) {
				TreeViewItem tvi = 
					ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
				if (tvi != null) {
					ApplySorting(tvi.Items);
				}
			}
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);

			TreeViewItem tvi = element as TreeViewItem;
			if (tvi != null) {
				ApplySorting(tvi.Items);
			}
		}
		

		/// <summary> GridViewColumn List</summary>
		public GridViewColumnCollection Columns {
			get {
				if (_columns == null) {
					_columns = new GridViewColumnCollection();
				}

				return _columns;
			}
		}

		private GridViewColumnCollection _columns;

    // This method was replaced by TreeViewExtensions which support select of
    // top-level item as well as items path (hierarchical item).

    //public bool SetSelectedItem(object item) {
    //  if (item == null) return false;

    //  TreeListViewItem container = 
    //    (TreeListViewItem)ItemContainerGenerator.ContainerFromItem(item);
    //  if (container != null) {
    //    container.IsSelected = true;
    //    return true;
    //  }

    //  return false;
    //}
	}
}
