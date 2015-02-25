using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {
	public class ScrollViewerSyncer {

		private ScrollViewer sv1;
		private ScrollViewer sv2;

		private bool sv1HorizontalVisible;
		private bool sv2HorizontalVisible;

		public ScrollViewerSyncer(ScrollViewer sv1, ScrollViewer sv2) {
			if (sv1 == null) throw new ArgumentNullException("sv1");
			if (sv2 == null) throw new ArgumentNullException("sv2");

			this.sv1 = sv1;
			this.sv2 = sv2;

			sv1HorizontalVisible = 
				sv1.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
			sv2HorizontalVisible =
				sv2.ComputedHorizontalScrollBarVisibility == Visibility.Visible;


			sv1.ScrollChanged += new ScrollChangedEventHandler(sv1_ScrollChanged);
			sv2.ScrollChanged += new ScrollChangedEventHandler(sv2_ScrollChanged);
		}

		private void sv2_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			if (e.OriginalSource != sv2) return;

			if (e.VerticalChange != 0) {
				sv1.ScrollToVerticalOffset(sv2.VerticalOffset);
			}

			bool sv2NewHV = sv2.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
			if (sv2HorizontalVisible != sv2NewHV) {
				sv2HorizontalVisible = sv2NewHV;
				MatchHeightDifferences();
			}
		}

		private void sv1_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			if (e.OriginalSource != sv1) return;

			if (e.VerticalChange != 0) {
				sv2.ScrollToVerticalOffset(sv1.VerticalOffset);
			}

			bool sv1NewHV = sv1.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
			if (sv1HorizontalVisible != sv1NewHV) {
				sv1HorizontalVisible = sv1NewHV;
				MatchHeightDifferences();
			}
		}

		private bool sv1HF;
/*
		private bool sv2HF = false;
*/

		private void MatchHeightDifferences() {
			if (sv1HorizontalVisible == sv2HorizontalVisible) {
				if (sv1HF) {
					Thickness margin = sv1.Margin;
					margin.Bottom = 0;
					sv1.Margin = margin;
				}
			}
			else {
				if (!sv1HorizontalVisible && sv2HorizontalVisible) {
					sv1HF = true;
					Thickness margin = sv1.Margin;
					margin.Bottom = sv1.ViewportHeight - sv2.ViewportHeight;
					sv1.Margin = margin;
				}
			}
		}
	}
}
