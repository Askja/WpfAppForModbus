using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace WpfAppForModbus.Models
{
    public class Settings {
        public bool SaveLogs { get; set; }

        public void SaveSettings(string filePath) {
            using var writer = new StreamWriter(filePath);

            var serializer = new XmlSerializer(typeof(Settings));
            serializer.Serialize(writer, this);
        }

        public static Settings? LoadSettings(string filePath) {
            if (File.Exists(filePath)) {
                using var reader = new StreamReader(filePath);

                var serializer = new XmlSerializer(typeof(Settings));
                return serializer.Deserialize(reader) as Settings;
            } else {
                return new Settings();
            }
        }

        public void UpdateFromControls(CheckBox checkBox) {
            SaveLogs = checkBox.IsChecked ?? false;
        }

        public void ApplyToControls(CheckBox checkBox) {
            checkBox.IsChecked = SaveLogs;
        }
    }
}
