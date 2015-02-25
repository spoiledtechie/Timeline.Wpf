using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline.TreeListViewControl {
  /// <summary>
  /// This code was downloaded from http://blogs.windowsclient.net/ricciolocristian/archive/2008/03/22/a-complete-wpf-treelistview-control.aspx
  /// 
  /// Convert Level to left margin
  /// Pass a prarameter if you want a unit length other than 19.0.
  /// </summary>
  public class LevelToIndentConverter : IValueConverter {
    public object Convert(object o, Type type, object parameter, CultureInfo culture) {
      Double indentSize = 0;
      if (parameter != null)
        Double.TryParse(parameter.ToString(), out indentSize);

      return ((int)o) * indentSize;
      //else
      //    return new Thickness((int)o * c_IndentSize, 0, 0, 0);
    }

    public object ConvertBack(object o, Type type, object parameter, CultureInfo culture) {
      throw new NotSupportedException();
    }

  }

}
