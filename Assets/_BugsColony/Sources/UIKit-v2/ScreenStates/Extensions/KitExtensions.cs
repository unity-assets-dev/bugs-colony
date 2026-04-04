using Newtonsoft.Json;

public static class KitExtensions {
    public static T Deserialize<T>(this string source) {
        return JsonConvert.DeserializeObject<T>(source);
    }

    public static string Serialize<T>(this T source) {
        return JsonConvert.SerializeObject(source);
    }
    
    public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
}