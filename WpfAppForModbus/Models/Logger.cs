using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfAppForModbus.Models {
    public class Logger {
        private TextBox LogElement { get; set; }
        private CheckBox SaveToFile { get; set; } = null!;

        public Logger(TextBox element) {
            LogElement = element;
        }

        public Logger(TextBox element, CheckBox saveToFile) {
            LogElement = element;
            SaveToFile = saveToFile;
        }

        public Logger AddLog(string text) {
            Dispatcher.CurrentDispatcher.Invoke(() => LogElement.AppendText(text + "\r\n"));

            if (SaveToFile != null && SaveToFile.IsChecked == true) {
                if (!Directory.Exists("logs")) {
                    Directory.CreateDirectory("logs");
                }

                File.AppendAllTextAsync("logs/" + DateTime.Now.ToString("dd.MM.yyyy") + ".log", text);
            }

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