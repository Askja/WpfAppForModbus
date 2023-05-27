using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace WpfAppForModbus.Models.Core {
    public class Logger {
        private TextBox LogElement { get; set; }
        private ToggleButton SaveToFile { get; set; } = null!;
        private Dispatcher CurrentDispatcher { get; set; } = null!;

        public Logger(Dispatcher currentDispatcher, TextBox element) {
            LogElement = element;
            CurrentDispatcher = currentDispatcher;
        }

        public Logger(Dispatcher currentDispatcher, TextBox element, ToggleButton saveToFile) {
            LogElement = element;
            SaveToFile = saveToFile;
            CurrentDispatcher = currentDispatcher;
        }

        public Logger AddLog(string text) {
            CurrentDispatcher.Invoke(() => {
                LogElement.AppendText(text + "\r\n");
                LogElement.ScrollToEnd();

                if (SaveToFile != null && SaveToFile.IsChecked == true) {
                    if (!Directory.Exists("logs")) {
                        Directory.CreateDirectory("logs");
                    }

                    File.AppendAllTextAsync("logs/" + DateTime.Now.ToString("dd.MM.yyyy") + ".log", text + "\r\n");
                }
            });

            return this;
        }

        public Logger AddDatedLog(string text) {
            return AddLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
        }

        public Logger AddCategorizedLog(string category, string text) {
            return AddLog("[" + category + "] " + text);
        }

        public Logger ClearLog() {
            LogElement.Clear();

            return this;
        }
    }
}