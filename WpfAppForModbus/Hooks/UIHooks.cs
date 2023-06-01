namespace WpfAppForModBus.Hooks;

public class UiHooks {
    public static void ClickElement<T>(T element) where T : System.Windows.UIElement {
        System.Windows.Input.MouseButtonEventArgs args =
            new(mouse: System.Windows.Input.Mouse.PrimaryDevice, timestamp: 0,
                button: System.Windows.Input.MouseButton.Left) {
                RoutedEvent = System.Windows.Input.Mouse.MouseDownEvent
            };

        element.RaiseEvent(e: args);
    }
}