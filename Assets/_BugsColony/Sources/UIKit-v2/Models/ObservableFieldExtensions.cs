public static class ObservableFieldExtensions {
    
    public static void Set<T>(this IObservableField<T> field, T value) => field.Value = value;

    public static T Get<T>(this IObservableField<T> field) => field.Value;

    public static void Add(this IObservableField<int> field, int value = 1) => field.Value += value;
    
    public static void Remove(this IObservableField<int> field, int value = 1) => field.Value -= value;
    
    public static bool IsEmpty(this IObservableField<string> field) => field.Value.IsNullOrEmpty();
    
}