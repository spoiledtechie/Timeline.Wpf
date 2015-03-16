using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline.TreeListViewControl {
  /// <summary>
  /// This code was downloaded from http://blogs.windowsclient.net/ricciolocristian/archive/2008/03/22/a-complete-wpf-treelistview-control.aspx
  /// </summary>
  public class TreeListViewItem : TreeViewItem {

    static TreeListViewItem() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem), new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
    }

    /// <summary>
    /// Item's hierarchy in the tree
    /// </summary>
    public int Level {
      get {
        if (_level == -1) {
          TreeListViewItem parent = ItemsControl.ItemsControlFromItemContainer(this) as TreeListViewItem;
          _level = (parent != null) ? parent.Level + 1 : 0;
        }
        return _level;
      }
    }

    protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
    {
        base.OnKeyUp(e);
    }


    protected override DependencyObject GetContainerForItemOverride() {
      return new TreeListViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item) {
      return item is TreeListViewItem;
    }

    private int _level = -1;

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
      base.PrepareContainerForItemOverride(element, item);

      TreeListView parent = WpfUtility.FindVisualParent<TreeListView>(this);
      TreeViewItem child = element as TreeViewItem;
      if (parent != null && child != null) {
        parent.ApplySorting(child.Items);
      }
    }
  }
}
