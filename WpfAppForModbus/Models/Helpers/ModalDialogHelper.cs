namespace WpfAppForModBus.Models.Helpers;

public static class ModalDialogHelper {
    public static void OpenModalWindow(string windowTitle, string contentText,
        MaterialDesignThemes.Wpf.PackIconKind iconKind,
        System.Windows.Window? ownerWindow = null) {
        System.Windows.Window modalWindow = new() {
            Title = windowTitle,
            SizeToContent = System.Windows.SizeToContent.Height,
            Owner = ownerWindow,
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
            MaxWidth = 450,
            WindowStyle = System.Windows.WindowStyle.None,
            ResizeMode = System.Windows.ResizeMode.NoResize,
            BorderBrush = System.Windows.Media.Brushes.Indigo,
            BorderThickness = new(uniformLength: 1)
        };

        System.Windows.Controls.Grid grid = new();
        modalWindow.Content = grid;

        System.Windows.Controls.StackPanel stackPanel = new() {
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            Orientation = System.Windows.Controls.Orientation.Vertical
        };

        MaterialDesignThemes.Wpf.PackIcon icon = new() {
            Kind = iconKind,
            Width = 48,
            Height = 48,
            Margin = new(left: 0, top: 0, right: 0, bottom: 10),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center
        };
        stackPanel.Children.Add(element: icon);

        System.Windows.Controls.TextBlock textBlock = new() {
            Text = contentText,
            FontSize = 16,
            TextAlignment = System.Windows.TextAlignment.Center,
            TextWrapping = System.Windows.TextWrapping.Wrap
        };
        stackPanel.Children.Add(element: textBlock);

        System.Windows.Controls.Button button = new() {
            Content = "Ок",
            Width = 100,
            Margin = new(uniformLength: 15),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            Style = (System.Windows.Style)System.Windows.Application.Current.Resources[key: "MaterialDesignFlatButton"]
        };
        button.Click += (sender, e) => modalWindow.Close();
        stackPanel.Children.Add(element: button);

        grid.Children.Add(element: stackPanel);

        modalWindow.MouseDown += (sender, e) => {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) {
                modalWindow.DragMove();
            }
        };

        modalWindow.Loaded += (sender, e) => {
            System.Windows.Shell.WindowChrome chrome = new() {
                CaptionHeight = 0,
                CornerRadius = new(uniformRadius: 10)
            };

            System.Windows.Shell.WindowChrome.SetWindowChrome(window: modalWindow, chrome: chrome);
        };

        modalWindow.ShowDialog();
    }
}