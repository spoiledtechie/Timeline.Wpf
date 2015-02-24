using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timeline.Wpf.MTest {
  public class TaskDependency {

    private Task prior;
    private Task depend;

    public TaskDependency() {
    }

    public Task Prior {
      get { return prior; }
      set { prior = value; }
    }

    public Task Depend {
      get { return depend; }
      set { depend = value; }
    }

    public string Text {
      get {
        StringBuilder sb = new StringBuilder();
        sb.Append(Prior.Name);
        sb.AppendLine();
        sb.Append(Depend.Name);

        return sb.ToString();
      }
    }

  }
}
