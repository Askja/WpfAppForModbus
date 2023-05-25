using System.Collections.Generic;

namespace WpfAppForModbus.Models.Helpers {
    public static class DictionaryHelper {
        public static TValue GetValueOrDefault<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) {
            if (dictionary.TryGetValue(key, out TValue? value)) {
                return value;
            } else {
                return defaultValue;
            }
        }
    }
}
