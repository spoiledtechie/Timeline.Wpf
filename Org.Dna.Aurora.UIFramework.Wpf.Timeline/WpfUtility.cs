using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {
	public static class WpfUtility {

		public static void WaitForPriority(DispatcherPriority priority) {
			DispatcherFrame frame = new DispatcherFrame();
			DispatcherOperation dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrameOperation), frame);
			Dispatcher.PushFrame(frame);
			if (dispatcherOperation.Status != DispatcherOperationStatus.Completed) {
				dispatcherOperation.Abort();
			}
		}

		private static object ExitFrameOperation(object obj) {
			((DispatcherFrame)obj).Continue = false;
			return null;
		}

		public static TChildItem FindVisualChild<TChildItem>(DependencyObject obj) where TChildItem : DependencyObject {
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is TChildItem)
					return (TChildItem)child;
				else {
					TChildItem childOfTchild = FindVisualChild<TChildItem>(child);
					if (childOfTchild != null)
						return childOfTchild;
				}
			}
			return null;
		}

		public static TParentItem FindVisualParent<TParentItem>(DependencyObject obj) where TParentItem : DependencyObject {
			DependencyObject current = obj;
			while (current != null && !(current is TParentItem))
				current = VisualTreeHelper.GetParent(current);
			return (TParentItem)current;
		}


	}
}
