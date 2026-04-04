using System;
using UnityEngine;

public class Egg : IConsumableTarget {
    
    private readonly EggView _view;
    
    public Vector3 Position {
        get => _view.Transform.position;
        set => _view.Transform.position = value;
    }
    
    public Egg(EggView view) {
        _view = view;
        _view.Actor = this;
    }

    public event Action<ActorStateAction> OnStateChanged;
    public event Action<IActor, ActorView> OnDisposed;

    public void OnUpdateFrame(float dt) {}

    public void OnConsume() => OnStateChanged?.Invoke(ActorStateAction.Died(this));

    public void Dispose() => OnDisposed?.Invoke(this, _view);
}