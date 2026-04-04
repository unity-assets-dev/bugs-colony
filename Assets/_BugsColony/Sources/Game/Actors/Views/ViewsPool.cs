using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ViewsPool : MonoBehaviour {
    [SerializeField] private PoolHandler[] _pools;
    
    private readonly Dictionary<Type, AbstractPool> _poolMap = new();

    [Inject]
    public void Construct(PoolHandler[] pools, IHeatMap heatMap) {
        _pools = pools;
        
        _pools.EachNonAlloc(pool => {
            pool.RegisterPool(this, heatMap);
        });
    }

    private bool TryGetPool<T>( out AbstractPool<ActorView> pool) where T: MonoBehaviour{
        var result = _poolMap.TryGetValue(typeof(T), out var p);
        pool = p as  AbstractPool<ActorView>;
        return result;
    }

    public T Get<T>() where T : MonoBehaviour {
        if (TryGetPool<T>(out var pool)) {
            return pool.Get() as T;
        }
        
        throw new NullReferenceException($"Requested pool {typeof(T)} was not found.");
    }

    public void Dispose<T>(T instance) where T: ActorView {
        var type = typeof(T).Name;
        if(TryGetPool<T>(out var pool)) 
            pool.Dispose(instance);
    }

    public void DisposeAll() => _poolMap.EachNonAlloc(p => p.Value.DisposeAll());

    public void AddView(ActorView view)  {
        var pool = new GameObject(view.GetType().Name);
        pool.transform.SetParent(transform);
        var instance = pool.gameObject.AddComponent<BugsViewPool>();
        instance.AddPrefab(view);
        _poolMap.Add(view.GetType(), instance);
    }
}