using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline
{
    public class RulerBlockItem : INotifyPropertyChanged
    {

        private string text;

        public RulerBlockItem()
        {
        }

        public long Span { get; set; }

        public long Start { get; set; }

        private string calcText;

        public string Text
        {
            get
            {
                if (text == null)
                {
                    if (calcText == null)
                    {
                        var ts = TimeSpan.FromTicks(Start);
                        if (ts.Hours > 0)
                            return ts.Hours+ ":" + ts.Minutes.ToString("D2") + ":" + ts.Seconds.ToString("D2");
                        if (ts.Minutes > 0)
                            return ts.Minutes.ToString("D2") + ":" + ts.Seconds.ToString("D2");
                        return ts.Seconds.ToString("D2");
                    }
                    return calcText;
                }
                return text;
            }
            set { text = value; }
        }

        public long End
        {
            get { return Start + Span; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string name)
        {
            var handlers = PropertyChanged;
            if (handlers != null)
            {
                handlers(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
