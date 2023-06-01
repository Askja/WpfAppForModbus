namespace WpfAppForModBus.Models.Helpers;

public class ComboBoxHelper {
    public static void AddRange<T>(System.Windows.Controls.ComboBox comboBox,
        System.Collections.Generic.IEnumerable<T> items) {
        foreach (T item in items) {
            comboBox.Items.Add(newItem: item);
        }
    }

    public static void AddRange(System.Windows.Controls.ComboBox comboBox, int[] items) {
        AddRange<int>(comboBox: comboBox, items: items);
    }

    public static T? GetSelectedItem<T>(System.Windows.Controls.ComboBox comboBox, T[] items) {
        int selectedIndex = comboBox.SelectedIndex;

        if (selectedIndex > -1) {
            return items[selectedIndex];
        }

        return default;
    }
}