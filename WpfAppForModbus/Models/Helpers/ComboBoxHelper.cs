using System.Collections.Generic;
using System.Windows.Controls;

namespace WpfAppForModbus.Models.Helpers {
    public class ComboBoxHelper {
        public static void AddRange<T>(ComboBox comboBox, IEnumerable<T> items) {
            foreach (T item in items) {
                comboBox.Items.Add(item);
            }
        }

        public static void AddRange(ComboBox comboBox, int[] items) {
            AddRange<int>(comboBox, items);
        }

        public static T? GetSelectedItem<T>(ComboBox comboBox, T[] items) {
            int selectedIndex = comboBox.SelectedIndex;

            if (selectedIndex > -1) {
                return items[selectedIndex];
            }

            return default;
        }
    }
}
