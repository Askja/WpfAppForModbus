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

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var icon = new PackIcon {
                Kind = PackIconKind.Email,
                Foreground = Brushes.White,
                Width = 24,
                Height = 24,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var closeButton = new Button {
                Content = new PackIcon {
                    Kind = PackIconKind.Close,
                    Foreground = Brushes.White,
                    Width = 24,
                    Height = 24,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 5)
            };

            closeButton.Click += (sender, e) =>
            {
                var parentBorderPanel = border;
                var parentNotificationsList = VisualTreeHelper.GetParent(parentBorderPanel) as StackPanel;

                parentNotificationsList?.Children.Remove(parentBorderPanel);
            };

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

            grid.Children.Add(icon);
            grid.Children.Add(notificationStackPanel);
            grid.Children.Add(closeButton);

            Grid.SetColumn(icon, 0);
            Grid.SetColumn(notificationStackPanel, 1);
            Grid.SetColumn(closeButton, 2);

            border.Child = grid;

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
