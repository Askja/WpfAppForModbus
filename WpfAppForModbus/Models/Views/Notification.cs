using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfAppForModbus.Models.Views {
    public static class Notification {
        public static Border Show(string notificationText, Color cardColor) {
            Border border = new() {
                Margin = new Thickness(3),
                Padding = new Thickness(12),
                CornerRadius = new CornerRadius(5),
                BorderBrush = Brushes.DarkGray,
                Background = new SolidColorBrush(cardColor)
            };

            TextBlock notificationTextBlock = new() {
                Text = notificationText,
                FontSize = 15,
                Foreground = Brushes.Black
            };

            border.Child = notificationTextBlock;

            return border;
        }

        public static Border ShowWarning(string Text) {
            return Show(Text, Colors.Yellow);
        }

        public static Border ShowError(string Text) {
            return Show(Text, Colors.IndianRed);
        }
    }
}
