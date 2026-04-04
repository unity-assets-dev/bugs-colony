using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

public static class CommonExtensions {
    public static NativeArray<T> ToNativeArray<T>(this IEnumerable<T> collection, Allocator allocator) where T: struct {
        var result = new NativeArray<T>(collection.Count(), allocator);
        for (var n = 0; n < result.Length; n++) {
            result[n] = collection.ElementAt(n);
        }
        return result;  
    }
}
