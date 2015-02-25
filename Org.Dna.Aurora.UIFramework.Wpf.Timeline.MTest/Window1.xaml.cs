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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline.MTest {
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window {
		private DateTime START = new DateTime(2001, 01, 01);

		private DateTime minDate, maxDate;
		private List<SuperTask> superTasks = new List<SuperTask>();
    private List<TaskDependency> taskDependencies = new List<TaskDependency>();

		public Window1() {
			InitializeComponent();

			minDate = DateTime.MaxValue;
			maxDate = DateTime.MinValue;

			GenerateDependentTasks();

			TheGantt.MinimumDate = minDate.AddHours(-2);
			TheGantt.MaximumDate = maxDate.AddHours(2);
			TheGantt.ItemsSource = superTasks.Cast<object>().Concat(tasksMap.Values.Cast<object>());
      TheGantt.ConnectionsSource = taskDependencies;
			TheGantt.TickTimeSpan = TimeSpan.FromSeconds(60);

			ContentTree.ItemsSource = superTasks;

			SyncScrollViewers();
		}

		private ScrollViewerSyncer syncher;

		private void SyncScrollViewers() {
			ContentTree.ApplyTemplate();
			TheGantt.ApplyTemplate();
			ScrollViewer treeSV = WpfUtility.FindVisualChild<ScrollViewer>(ContentTree);
			ScrollViewer timelineSV = WpfUtility.FindVisualChild<ScrollViewer>(TheGantt);

			if (treeSV != null && timelineSV != null) {
				syncher = new ScrollViewerSyncer(treeSV, timelineSV);
			}
		}

		private class ListGen : IItemsProvider<int> {


			public int FetchCount() {
				return 1 * 100 * 100;
			}

			public IList<int> FetchRange(int startIndex, int count) {
				List<int> l = new List<int>(count);
				for (int i = startIndex; i < startIndex+count; i++) {
					l.Add(i);
				}
				return l;
			}
		}

		private static DateTime dt(string dateString) {
			return DateTime.Parse(dateString);
		}

		private Dictionary<int, Task> tasksMap = new Dictionary<int, Task>();
		private void GenerateDependentTasks() {

			SuperTask st1 = new SuperTask();
			st1.Name = "S1";
			superTasks.Add(st1);

			SuperTask st2 = new SuperTask();
			st2.Name = "S2";
			superTasks.Add(st2);

			SuperTask st3 = new SuperTask();
			st3.Name = "S3";
			superTasks.Add(st3);


			tasksMap[9] = new Task(st1) { Name = new string('9', 500), Start = dt("22:00"), End = dt("23:00") };
			tasksMap[8] = new Task(st3) { Name = "8", Start = dt("21:00"), End = dt("21:30") };
			tasksMap[8].DepTasks.Add(tasksMap[9]);
			tasksMap[7] = new Task(st3) { Name = "7", Start = dt("20:00"), End = dt("20:30") };
			tasksMap[7].DepTasks.Add(tasksMap[8]);
			tasksMap[6] = new Task(st3) { Name = "6", Start = dt("21:10"), End = dt("21:40") };
			tasksMap[6].DepTasks.Add(tasksMap[9]);
			tasksMap[5] = new Task(st2) { Name = new string('5', 10), Start = dt("20:10"), End = dt("20:30") };
			tasksMap[5].DepTasks.Add(tasksMap[6]);
			tasksMap[4] = new Task(st2) { Name = new string('4', 20), Start = dt("19:00"), End = dt("19:30") };
			tasksMap[4].DepTasks.Add(tasksMap[5]);
			tasksMap[4].DepTasks.Add(tasksMap[7]);
			tasksMap[3] = new Task(st1) { Name = "3", Start = dt("18:00"), End = dt("18:30") };
			tasksMap[3].DepTasks.Add(tasksMap[4]);
			tasksMap[2] = new Task(st1) { Name = "2", Start = dt("17:50"), End = dt("18:30") };
			tasksMap[2].DepTasks.Add(tasksMap[4]);
			tasksMap[1] = new Task(st1) { Name = "1", Start = dt("16:00"), End = dt("17:00") };
			tasksMap[1].DepTasks.Add(tasksMap[2]);
			tasksMap[1].DepTasks.Add(tasksMap[3]);

			st1.Tasks.Add(tasksMap[1]);
			st1.Tasks.Add(tasksMap[2]);
			st1.Tasks.Add(tasksMap[3]);
			st1.Tasks.Add(tasksMap[9]);

			st2.Tasks.Add(tasksMap[4]);
			st2.Tasks.Add(tasksMap[5]);

			st3.Tasks.Add(tasksMap[6]);
			st3.Tasks.Add(tasksMap[7]);
			st3.Tasks.Add(tasksMap[8]);

      foreach (var priorTask in tasksMap) {
        foreach (var depTask in priorTask.Value.DepTasks) {
          TaskDependency dep = new TaskDependency();
          dep.Prior = priorTask.Value;
          dep.Depend = depTask;
          taskDependencies.Add(dep);
        }
      }

			minDate = dt("16:00");
			maxDate = dt("22:00");

			ApplySorting();
		}

		private void ApplySorting() {
			int order = 0;

			foreach (var st in superTasks.OrderBy<SuperTask, DateTime>(t=>t.Start)) {
				st.DisplayOrder = order++;

				var tasks =
					from t in st.Tasks
					orderby t.Start
					select t;
				foreach (var t in tasks) {
					t.DisplayOrder = order++;
				}
			}
		}

		private void Generate100RandomTasks(ref List<Task> tasks) {
			SuperTask st = new SuperTask();
			st.Name = "MainSuperTask";

			for (int i = 0; i < 100; i++) {
				Task t = new Task(st);
				t.Name = i + " Task";
				t.Start = START.AddHours(Math.Abs(Math.Sin(i) * 40));
				t.End = t.Start.AddHours(Math.Abs(Math.Cos(i) * 3));
				Debug.Assert(t.Start <= t.End);
				tasks.Add(t);

				if (t.Start < minDate) minDate = t.Start;
				if (t.End > maxDate) maxDate = t.End;
			}
		}
	}
}
