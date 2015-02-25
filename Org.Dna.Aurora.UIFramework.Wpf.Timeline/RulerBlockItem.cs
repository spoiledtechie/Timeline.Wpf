using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {
	public class RulerBlockItem  : INotifyPropertyChanged {

		private string text;

		public RulerBlockItem() {
		}

		public TimeSpan Span { get; set; }

		public DateTime Start { get; set; }

		private string calcText;

		public string Text {
			get {
				if (text == null) {
					if (calcText == null) {
						if (Span.TotalDays > 50) {
							calcText = Start.ToString("MM/yy");
						}
						else if (Span.TotalDays > 1) {
							calcText = Start.ToString("dd/MM/yy");
						}
						else {
							calcText = Start.ToString("HH:mm dd/MM/yy");
						}
					}

					return calcText;
				}
				return text;
			}
			set { text = value; }
		}

		public DateTime End {
			get { return Start.Add(Span); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void onPropertyChanged(string name) {
			var handlers = PropertyChanged;
			if (handlers != null) {
				handlers(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
