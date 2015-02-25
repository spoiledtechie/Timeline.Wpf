//Copyright 2010 Ido Ran. All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are
//permitted provided that the following conditions are met:

//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.

//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.

//THIS SOFTWARE IS PROVIDED BY Ido Ran ``AS IS'' AND ANY EXPRESS OR IMPLIED
//WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Ido Ran OR
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//The views and conclusions contained in the software and documentation are those of the
//authors and should not be interpreted as representing official policies, either expressed
//or implied, of Ido Ran.

//==============================
//My Blog: dotdotnet.blogger.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline.MTest {
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
