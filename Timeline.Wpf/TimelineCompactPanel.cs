using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.Windows.Media.Animation;

namespace Timeline.Wpf {
	/// <summary>
	/// Timeline panel which arrange the items in a compact way.
	/// Items happen concurrencly arranged in seperate rows,
	/// serial items arrange on the same row, as much as possible.
	/// </summary>
	public class TimelineCompactPanel : TimelineGanttPanel {

		public TimelineCompactPanel() {

		}

		protected override void SetChildRow(UIElement child, List<UIElement> measuredChildren) {
			//int childRow = 0;
			int childRow = -1;

			bool[] overlapRows = new bool[RowsCount + 1];
			Nullable<DateTime> currStart = GetStartDate(child);
			Nullable<DateTime> currEnd = GetEndDate(child);

			if (!(currStart == null || currEnd == null)) {
				foreach (UIElement previousChild in measuredChildren) {
					Nullable<DateTime> prevStart = GetStartDate(previousChild);
					Nullable<DateTime> prevEnd = GetEndDate(previousChild);

					if (prevStart.HasValue && prevEnd.HasValue) {
						DateTime smallStart, smallEnd, largeStart, largeEnd;
						if (currStart <= prevStart) {
							smallStart = currStart.Value;
							smallEnd = currEnd.Value;
							largeStart = prevStart.Value;
							largeEnd = prevEnd.Value;
						}
						else {
							smallStart = prevStart.Value;
							smallEnd = prevEnd.Value;
							largeStart = currStart.Value;
							largeEnd = currEnd.Value;
						}

						bool overlap = largeStart <= smallEnd;
						if (overlap) {
							overlapRows[GetRowIndex(previousChild)] = true;
							//childRow = GetRowIndex(previousChild) + 1;
						}
					}
				}

				for (int i = 0; i <= RowsCount; i++) {
					if (!overlapRows[i]) {
						childRow = i;
						break;
					}
				}
			}

			if (childRow == -1) {
				childRow = RowsCount + 1;
			}

			SetRowIndex(child, childRow);
			RowsCount = Math.Max(RowsCount, childRow);
		}

		protected override void ArrangeChild(UIElement child, int rowIndex) {
			int transRowIndex = TransformRowIndex(rowIndex);
			base.ArrangeChild(child, transRowIndex);
		}

		/// <summary>
		/// Transform zero-based row index to distribute the rows
		/// from center out-words.
		/// 
		/// 0 => 4
		/// 1 => 2
		/// 2 => 0
		/// 3 => 1
		/// 4 => 3
		/// </summary>
		/// <param name="originalRowIndex"></param>
		/// <returns></returns>
		private int TransformRowIndex(int originalRowIndex) {
			int midRow = (int)Math.Ceiling(RowsCount / (float)2);
			int step = (int)Math.Ceiling(originalRowIndex / (float)2);
			if (originalRowIndex % 2 == 0) step *= -1;

			int trans = midRow + step;
			return trans - RowsCount % 2;
		}
	}
}
