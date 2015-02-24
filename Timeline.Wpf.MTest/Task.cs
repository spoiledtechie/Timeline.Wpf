using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Timeline.Wpf.MTest {
	public class Task : INotifyPropertyChanged {

		private IList<Task> depTasks;
		private SuperTask parent;

		public Task(SuperTask parent) {
			depTasks = new ObservableCollection<Task>();
			this.parent = parent;
		}

		public string Name { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public int DisplayOrder { get; set; }

		public bool IsCollapsed {
			get { return !parent.IsChildrenVisible; }
		}

		public IList<Task> DepTasks { get { return depTasks; } }

		public override string ToString() {
			return string.Format("{0} {1}", GetType().Name, Name);
		}

		internal void NotifyIsCollapsed() {
			OnPropertyChanged("IsCollapsed");
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
