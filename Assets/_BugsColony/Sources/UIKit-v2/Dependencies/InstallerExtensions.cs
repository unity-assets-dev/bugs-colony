using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public static class InstallerExtensions {
    private static System.Random _random = new();
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> array, uint count = 1) {
        var source = array.ToArray();
        
        for (var i = 0; i < count; i++) {
            var n = source.Length;
            while (n > 1) {
                n--;
                var k = _random.Next(n + 1);
                (source[k], source[n]) = (source[n], source[k]);
            }
        }
        
        return source;
    }
    public static IEnumerable<T> EachNonAlloc<T>(this IEnumerable<T> source, Action<T> onItemSelect) {
        if (source == null || source.Count() == 0) return source;
        
        foreach (var item in source) {
            onItemSelect?.Invoke(item);
        }
        
        return source;
    }
    
    public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> onItemSelect) {
        if (source == null || source.Count() == 0) return source;
        
        var buffer = source.ToArray();
        
        foreach (var item in buffer) {
            onItemSelect?.Invoke(item);
        }
        
        return buffer;
    }
    
    public static IEnumerable<T> For<T>(this IEnumerable<T> source, Action<T, int> onItemSelect) {
        if (source == null || source.Count() == 0) return source;
        
        var buffer = source.ToArray();

        for (var index = 0; index < buffer.Length; index++) {
            onItemSelect?.Invoke(buffer[index], index);
        }

        return buffer;
    }

    public static bool TryGetItem<TSource, TItem>(this IEnumerable<TSource> source, out TItem item) where TItem : TSource {
        item = source.OfType<TItem>().FirstOrDefault();

        return item != null;
    }
    
    public static Type[] GetAssembliesTypes(this AppDomain domain) {
        return domain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToArray();
    }
    
    public static void OnType<TType>(this IEnumerable<Type> source, Action<Type> onItem) {
        Type interfaceType = typeof(TType);
        
        source
            .Where(type => !type.IsAbstract && type.IsClass && interfaceType.IsAssignableFrom(type))
            .Each(t => onItem?.Invoke(t));
    }

    public static void BindAsSingle<TType, TService>(this DiContainer container) where TType: class where TService: class, TType {
        container
            .Bind<TType>()
            .To<TService>()
            .AsSingle();
    }
    
    public static void BindAsSingle<TType>(this DiContainer container)  {
        container
            .BindInterfacesAndSelfTo<TType>()
            .AsSingle();
    }

    public static void BindAsSingleFromInstanceMono<TType>(this DiContainer container) where TType : MonoBehaviour {
        container.BindAsSingleFromInstance(UnityEngine.Object.FindAnyObjectByType<TType>());
    }
    
    public static void BindAsSingleFromInstance<TType>(this DiContainer container, TType instance) {
        container.BindAsSingleFromInstanceType(instance);
    }
    
    public static void BindAsSingleFromInstanceType(this DiContainer container, object instance) {
        container
            .BindInterfacesAndSelfTo(instance.GetType())
            .FromInstance(instance);
    }
    
    public static bool TryGetAttribute<TAttribute>(this Type type, out TAttribute attribute)
        where TAttribute : Attribute {
        attribute = type.GetCustomAttributes(false).OfType<TAttribute>().FirstOrDefault();

        return attribute != null;
    }
    
}