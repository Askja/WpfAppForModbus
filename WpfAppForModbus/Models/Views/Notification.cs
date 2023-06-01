namespace WpfAppForModBus.Models.Views;

public static class Notification {
    public static System.Windows.Controls.Border Show(string notificationTitle, string notificationText,
        System.Windows.Media.Color cardColor, System.Func<bool> handle) {
        System.Windows.Controls.Border border = new() {
            Margin = new(uniformLength: 3),
            Padding = new(uniformLength: 12),
            CornerRadius = new(uniformRadius: 3),
            BorderBrush = System.Windows.Media.Brushes.DarkGray,
            Background = new System.Windows.Media.SolidColorBrush(color: cardColor)
        };

        System.Windows.Controls.Grid grid = new();

        grid.ColumnDefinitions.Add(value: new() { Width = new(value: 1, type: System.Windows.GridUnitType.Auto) });
        grid.ColumnDefinitions.Add(value: new() { Width = new(value: 1, type: System.Windows.GridUnitType.Auto) });
        grid.ColumnDefinitions.Add(value: new() { Width = new(value: 1, type: System.Windows.GridUnitType.Star) });

        MaterialDesignThemes.Wpf.PackIcon icon = new() {
            Kind = MaterialDesignThemes.Wpf.PackIconKind.Email,
            Foreground = System.Windows.Media.Brushes.White,
            Width = 24,
            Height = 24,
            Margin = new(left: 0, top: 0, right: 10, bottom: 0)
        };

        System.Windows.Controls.Button closeButton = new() {
            Content = new MaterialDesignThemes.Wpf.PackIcon {
                Kind = MaterialDesignThemes.Wpf.PackIconKind.Close,
                Foreground = System.Windows.Media.Brushes.White,
                Width = 24,
                Height = 24,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            },
            Background = System.Windows.Media.Brushes.Transparent,
            BorderBrush = System.Windows.Media.Brushes.Transparent,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
            Padding = new(uniformLength: 0),
            Margin = new(left: 0, top: 0, right: 0, bottom: 5)
        };

        closeButton.Click += (sender, e) => {
            System.Windows.Controls.Border parentBorderPanel = border;
            System.Windows.Controls.StackPanel? parentNotificationsList =
                System.Windows.Media.VisualTreeHelper.GetParent(reference: parentBorderPanel) as
                    System.Windows.Controls.StackPanel;

            parentNotificationsList?.Children.Remove(element: parentBorderPanel);

            handle();
        };

        System.Windows.Controls.TextBlock textBlock = new() {
            Text = notificationText,
            FontSize = 14,
            Foreground = System.Windows.Media.Brushes.White
        };

        System.Windows.Controls.TextBlock titleTextBlock = new() {
            Text = notificationTitle,
            FontSize = 16,
            FontWeight = System.Windows.FontWeights.Bold,
            Foreground = System.Windows.Media.Brushes.White,
            Margin = new(left: 0, top: 0, right: 0, bottom: 5)
        };

        System.Windows.Controls.StackPanel notificationStackPanel = new() {
            Orientation = System.Windows.Controls.Orientation.Vertical,
            Background = new System.Windows.Media.SolidColorBrush(color: cardColor),
            Margin = new(left: 0, top: 0, right: 20, bottom: 5),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalAlignment = System.Windows.VerticalAlignment.Bottom
        };

        notificationStackPanel.Children.Add(element: titleTextBlock);
        notificationStackPanel.Children.Add(element: textBlock);

        grid.Children.Add(element: icon);
        grid.Children.Add(element: notificationStackPanel);
        grid.Children.Add(element: closeButton);

        System.Windows.Controls.Grid.SetColumn(element: icon, value: 0);
        System.Windows.Controls.Grid.SetColumn(element: notificationStackPanel, value: 1);
        System.Windows.Controls.Grid.SetColumn(element: closeButton, value: 2);

        border.Child = grid;

        return border;
    }

    public static System.Windows.Controls.Border ShowWarning(string title, string text, System.Func<bool> handle) =>
        Show(notificationTitle: title, notificationText: text, cardColor: System.Windows.Media.Colors.CornflowerBlue,
            handle: handle);

    public static System.Windows.Controls.Border ShowError(string title, string text, System.Func<bool> handle) => Show(
        notificationTitle: title, notificationText: text, cardColor: System.Windows.Media.Colors.DarkRed,
        handle: handle);
}