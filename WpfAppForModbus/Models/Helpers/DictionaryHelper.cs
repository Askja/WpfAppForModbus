namespace WpfAppForModBus.Models.Helpers;

public static class DictionaryHelper {
    public static TValue GetValueOrDefault<TKey, TValue>(System.Collections.Generic.Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue) where TKey : notnull =>
        dictionary.TryGetValue(key: key, value: out TValue? value) ? value : defaultValue;

    public static void UpdateDictionaryValue<TKey, TValue>(
        System.Collections.Generic.Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull {
        if (dictionary.ContainsKey(key: key)) {
            dictionary[key: key] = value;
        }
        else {
            dictionary.Add(key: key, value: value);
        }
    }
}