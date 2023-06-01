namespace WpfAppForModBus.Models.Core;

public class Logger {
    public Logger(System.Windows.Threading.Dispatcher currentDispatcher, System.Windows.Controls.TextBox element) {
        LogElement = element;
        CurrentDispatcher = currentDispatcher;
    }

    public Logger(System.Windows.Threading.Dispatcher currentDispatcher, System.Windows.Controls.TextBox element,
        System.Windows.Controls.Primitives.ToggleButton saveToFile) {
        LogElement = element;
        SaveToFile = saveToFile;
        CurrentDispatcher = currentDispatcher;
    }

    private System.Windows.Controls.TextBox LogElement { get; }
    private System.Windows.Controls.Primitives.ToggleButton SaveToFile { get; } = null!;
    private System.Windows.Threading.Dispatcher CurrentDispatcher { get; } = null!;

    public Logger AddLog(string text) {
        CurrentDispatcher.Invoke(callback: () => {
            LogElement.AppendText(textData: text + "\r\n");
            LogElement.ScrollToEnd();

            if (SaveToFile != null && SaveToFile.IsChecked == true) {
                if (!System.IO.Directory.Exists(path: "logs")) {
                    System.IO.Directory.CreateDirectory(path: "logs");
                }

                System.IO.File.AppendAllTextAsync(
                    path: "logs/" + System.DateTime.Now.ToString(format: "dd.MM.yyyy") + ".log",
                    contents: text + "\r\n");
            }
        });

        return this;
    }

    public Logger AddDatedLog(string text) =>
        AddLog(text: "[" + System.DateTime.Now.ToString(format: "HH:mm:ss") + "] " + text);

    public Logger AddCategorizedLog(string category, string text) => AddLog(text: "[" + category + "] " + text);

    public Logger ClearLog() {
        LogElement.Clear();

        return this;
    }
}