using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPool : MonoBehaviour {
    public virtual Type Type => GetType();

    public virtual void DisposeAll() {}
}

public abstract class AbstractPool<T> : AbstractPool where T : MonoBehaviour {
    [SerializeField] private T _prefab;
    
    private readonly Queue<T> _pool = new();
    private readonly List<T> _active = new();
    
    public override Type Type => _prefab.GetType();
    

    public T Get() {
        if (_pool.Count == 0) {
            AddInstanceToPool();
        }
        
        var instance = _pool.Dequeue();
        _active.Add(instance);
        instance.gameObject.SetActive(true);
        return instance;
    }

    private void AddInstanceToPool() => AddInstance(Instantiate(_prefab, transform));

    private void AddInstance(T instance) {
        instance.gameObject.SetActive(false);
        _pool.Enqueue(instance);
    }

    public void Dispose(T instance) {
        if (_active.Contains(instance)) {
            _active.Remove(instance);
            
            AddInstance(instance);
        }
    }
    
    public void AddPrefab(T view) {
        _prefab = view;
    }

    public override void DisposeAll() {
        foreach (var instance in _active) {
            Dispose(instance);
        }
    }
}