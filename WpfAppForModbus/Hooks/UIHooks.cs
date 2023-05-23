using System.Windows;
using System.Windows.Input;

namespace WpfAppForModbus.Hooks {
    public class UIHooks {
        public static void ClickElement<T>(T element) where T : UIElement {
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) {
                RoutedEvent = Mouse.MouseDownEvent
            };

            element.RaiseEvent(args);
        }
    }
}
