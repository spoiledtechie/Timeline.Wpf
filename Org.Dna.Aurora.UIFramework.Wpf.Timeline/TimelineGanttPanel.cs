using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {

  /// <summary>
  /// Timeline Panel which arrange the items like Gantt chart -
  /// each item in a separated row.
  /// </summary>
  public class TimelineGanttPanel : TimelinePanel {

    protected override Size MeasureOverride(Size availableSize) {
      rowsCount = -1;
      List<UIElement> measuredChildren = new List<UIElement>();
      Dictionary<int, int> logicalToActualMap = new Dictionary<int, int>();

      var q =
        from c in Children.OfType<UIElement>()
        let row = GetRowIndex(c)
        orderby row
        select new { Child = c, Row = row };

      int nextActualRowIndex = 0;
      foreach (var childAndRow in q) {
        ClearActualRowIndex(childAndRow.Child);
        childAndRow.Child.ClearValue(UIElement.VisibilityProperty);

        TimelineItem tlChild = (TimelineItem)childAndRow.Child;

        tlChild.IsDisplayAsZero = ShouldDisplayAsZero(tlChild);
        tlChild.ApplyTemplate();

        Rect calcChildSize = CalcChildRect(tlChild, nextActualRowIndex);
        childAndRow.Child.Measure(calcChildSize.Size);

        if (tlChild.IsCollapsed) {
          childAndRow.Child.Visibility = Visibility.Collapsed;
        }
        else {
          int actualRowIndex;
          if (!logicalToActualMap.TryGetValue(childAndRow.Row, out actualRowIndex)) {
            actualRowIndex = nextActualRowIndex++;
            logicalToActualMap.Add(childAndRow.Row, actualRowIndex);
          }
          SetActualRowIndex(childAndRow.Child, actualRowIndex);

          SetChildRow(childAndRow.Child, measuredChildren);
          measuredChildren.Add(childAndRow.Child);

          if (IsChildHasNoStartAndEndTime(childAndRow.Child)) {
            childAndRow.Child.Visibility = Visibility.Hidden;
          }
        }
      }

      TimeSpan totalTimeSpan = TimeSpan.Zero;
      if (MaximumDate.HasValue && MinimumDate.HasValue) {
        totalTimeSpan = MaximumDate.Value - MinimumDate.Value;
      }
      double totalWidth = Math.Max(0, totalTimeSpan.Ticks * PixelsPerTick);
      double totalHeight = Math.Max(0,
        nextActualRowIndex * RowHeight + nextActualRowIndex * RowVerticalMargin);
      return totalWidth <= 0 || totalHeight <= 0 ? Size.Empty : new Size(totalWidth, totalHeight);
    }

    private bool IsChildHasNoStartAndEndTime(UIElement child) {
      DateTime? childStartDate = GetStartDate(child);
      DateTime? childEndDate = GetEndDate(child);

      bool noTimes = childStartDate == null && childEndDate == null;
      return noTimes;
    }

    protected virtual void SetChildRow(
      UIElement child,
      List<UIElement> measuredChildren) {

    }

  }
}
