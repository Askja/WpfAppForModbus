using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfAppForModbus.Models
{
    public class Logger {
        private TextBox LogElement { get; set; }

        public Logger(TextBox Element) {
            LogElement = Element;
        }

        public Logger AddLog(string Text) {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => LogElement.AppendText(Text + "\r\n")));

            return this;
        }

        public Logger AddDatedLog(string Text) {
            return AddLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + Text);
        }

        public Logger AddCategorizedLog(string Category, string Text) {
            return AddLog("[" + Category + "] " + Text);
        }

        public Logger ClearLog() {
            LogElement.Clear();

            return this;
        }
    }
}
