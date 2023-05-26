using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace WpfAppForModbus.Models.Core {
    public class Settings {
        [JsonProperty("comboBoxValues")]
        public Dictionary<string, int> ComboBoxValues { get; set; }

        [JsonProperty("textBoxValues")]
        public Dictionary<string, string> TextBoxValues { get; set; }

        [JsonProperty("checkBoxValues")]
        public Dictionary<string, bool> CheckBoxValues { get; set; }

        public Settings() {
            ComboBoxValues = new Dictionary<string, int>();
            TextBoxValues = new Dictionary<string, string>();
            CheckBoxValues = new Dictionary<string, bool>();
        }

        public void Save(string filePath) {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static Settings? Load(string filePath) {
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);

                return JsonConvert.DeserializeObject<Settings>(json);
            } else {
                return new Settings();
            }
        }
    }
}