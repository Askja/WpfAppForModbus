using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfAppForModbus.Models.Views {
    public static class Notification {
        public static Border Show(string notificationTitle, string notificationText, Color cardColor) {
            var border = new Border {
                Margin = new Thickness(3),
                Padding = new Thickness(12),
                CornerRadius = new CornerRadius(3),
                BorderBrush = Brushes.DarkGray,
                Background = new SolidColorBrush(cardColor)
            };

            var stackPanel = new StackPanel {
                Orientation = Orientation.Horizontal
            };

            var icon = new PackIcon {
                Kind = PackIconKind.Email,
                Foreground = Brushes.White,
                Width = 24,
                Height = 24,
                Margin = new Thickness(0, 0, 10, 0)
            };

            stackPanel.Children.Add(icon);

            var textBlock = new TextBlock {
                Text = notificationText,
                FontSize = 14,
                Foreground = Brushes.White
            };

            var titleTextBlock = new TextBlock {
                Text = notificationTitle,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var notificationStackPanel = new StackPanel {
                Orientation = Orientation.Vertical,
                Background = new SolidColorBrush(cardColor),
                Margin = new Thickness(0, 0, 20, 5),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            notificationStackPanel.Children.Add(titleTextBlock);
            notificationStackPanel.Children.Add(textBlock);

            stackPanel.Children.Add(notificationStackPanel);

            border.Child = stackPanel;

            return border;
        }

        public static Border ShowWarning(string Title, string Text) {
            return Show(Title, Text, Colors.CornflowerBlue);
        }

        public static Border ShowError(string Title, string Text) {
            return Show(Title, Text, Colors.DarkRed);
        }
    }
}
