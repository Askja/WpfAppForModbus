namespace WpfAppForModBus.Models.Core;

public class Settings {
    public Settings() {
        ComboBoxValues = new();
        TextBoxValues = new();
        CheckBoxValues = new();
    }

    [Newtonsoft.Json.JsonPropertyAttribute(propertyName: "comboBoxValues")]
    public System.Collections.Generic.Dictionary<string, int> ComboBoxValues { get; set; }

    [Newtonsoft.Json.JsonPropertyAttribute(propertyName: "textBoxValues")]
    public System.Collections.Generic.Dictionary<string, string> TextBoxValues { get; set; }

    [Newtonsoft.Json.JsonPropertyAttribute(propertyName: "checkBoxValues")]
    public System.Collections.Generic.Dictionary<string, bool> CheckBoxValues { get; set; }

    public void Save(string filePath) {
        string json =
            Newtonsoft.Json.JsonConvert.SerializeObject(value: this, formatting: Newtonsoft.Json.Formatting.Indented);
        System.IO.File.WriteAllText(path: filePath, contents: json);
    }

    public static Settings Load(string filePath) {
        if (System.IO.File.Exists(path: filePath)) {
            string json = System.IO.File.ReadAllText(path: filePath);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(value: json) ?? new Settings();
        }

        return new();
    }
}