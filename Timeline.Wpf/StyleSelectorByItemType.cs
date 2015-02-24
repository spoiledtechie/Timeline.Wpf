using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Timeline.Wpf {
	
	/// <summary>
	/// StyleSelector implementation which select style by the type of the
	/// item as the key of the style resource.
	/// </summary>
	public class StyleSelectorByItemType : StyleSelector {
		
		public override Style SelectStyle(object item, DependencyObject container) {

			Style style = null;

			FrameworkElement fe = container as FrameworkElement;
			if (fe != null) {
				style = fe.TryFindResource(item.GetType()) as Style;
			}

			return style;
		}
	}
}
