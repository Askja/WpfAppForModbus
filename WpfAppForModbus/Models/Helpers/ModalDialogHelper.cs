using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace WpfAppForModbus.Models.Helpers {
    public static class ModalDialogHelper {
        public static void OpenModalWindow(string windowTitle, string contentText, PackIconKind iconKind, Window? ownerWindow = null) {
            var modalWindow = new Window {
                Title = windowTitle,
                SizeToContent = SizeToContent.Height,
                Owner = ownerWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                MaxWidth = 450,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                BorderBrush = Brushes.Indigo,
                BorderThickness = new Thickness(1)
            };

            var grid = new Grid();
            modalWindow.Content = grid;

            var stackPanel = new StackPanel {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical
            };

            var icon = new PackIcon {
                Kind = iconKind,
                Width = 48,
                Height = 48,
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(icon);

            var textBlock = new TextBlock {
                Text = contentText,
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            stackPanel.Children.Add(textBlock);

            var button = new Button {
                Content = "Ок",
                Width = 100,
                Margin = new Thickness(15),
                HorizontalAlignment = HorizontalAlignment.Center,
                Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"]
            };
            button.Click += (sender, e) => modalWindow.Close();
            stackPanel.Children.Add(button);

            grid.Children.Add(stackPanel);

            modalWindow.MouseDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left) {
                    modalWindow.DragMove();
                }
            };

            modalWindow.Loaded += (sender, e) =>
            {
                var chrome = new WindowChrome {
                    CaptionHeight = 0,
                    CornerRadius = new CornerRadius(10)
                };

                WindowChrome.SetWindowChrome(modalWindow, chrome);
            };

            modalWindow.ShowDialog();
        }
    }
}
