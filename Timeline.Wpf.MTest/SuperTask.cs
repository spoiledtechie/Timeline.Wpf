using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Timeline.Wpf.MTest {
	public class SuperTask : INotifyPropertyChanged {

		private IList<Task> tasks;
		private bool isChildrenVisible;

		public SuperTask() {
			tasks = new List<Task>();
      isChildrenVisible = true;
		}

		public string Name { get; set; }
		public int DisplayOrder { get; set; }

		public bool IsChildrenVisible {
			get { return isChildrenVisible; }
			set {
				isChildrenVisible = value;
				OnPropertyChanged("IsChildrenVisible");

				foreach (var item in tasks) {
					item.NotifyIsCollapsed();
				}
			}
		}

		public DateTime Start {
			get {
				var q =
					from t in tasks
					select t.Start;
				return q.Min();
			}
		}
		public DateTime End {
			get {
				var q =
					from t in tasks
					select t.End;
				return q.Max();
			}
		}

		public IList<Task> Tasks {
			get { return tasks; }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string name) {
			var h = PropertyChanged;
			if (h != null) {
				h(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
