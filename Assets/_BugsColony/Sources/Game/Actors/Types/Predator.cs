using System;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IPredatorModel {
     float SearchRadius { get; }
     float LifeTime { get; }
     LayerMask ActorMask { get; }
}

public class Predator : IBugActor, IInteractionHandler, IConsumableTarget {
    private readonly IPredatorModel _model;
    private readonly PredatorView _view;
    private readonly IHeatMap _map;
    
    private readonly Collider[] _cache = new Collider[20];
    private readonly IDisposable _subscription;
    
    
    private int _collectedFood;
    private ActorState _state;
    
    private float _lifeTimer;
    private float _headTimer;

    public event Action<ActorStateAction> OnStateChanged;
    public event Action<IActor, ActorView> OnDisposed;

    public Vector3 Position {
        get => _view.Transform.position;
        set => _view.Transform.position = value;
    }
    
    public Predator(IPredatorModel model, PredatorView view, IHeatMap map) {
        _model = model;
        _view = view;
        _map = map;
        _view.Actor = this;
        _subscription = _view.Subscribe(this);
    }
    
    public void SetState(ActorState state) {
        if (_state != state) {
            _state = state;
            _view.gameObject.SetActive(_state == ActorState.Alive);
            
            if (_state == ActorState.Alive) {
                ReloadTimer();
            }
            
            OnStateChanged?.Invoke(ActorStateAction.State(this, _state));
        }
    }
    
    public void OnConsume() => SetState(ActorState.Died);

    public void ConsumeTarget(IActor actor) {
        if (actor is IConsumableTarget target) {
            _collectedFood++;
            ReloadTimer();

            OnStateChanged?.Invoke(ActorStateAction.Consume(this, target));
        }
    }

    public void OnMutate(ISimulationController simulation) {
        if (_collectedFood >= 3) {
            simulation.AddCocoon<Predator>(Position);
            _collectedFood = 0;
            ReloadTimer();
        }
    }

    private void ReloadTimer() {
        _lifeTimer = _model.LifeTime;
    }

    public void OnUpdateFrame(float dt) {
        if (_state != ActorState.Alive) {
            return;
        }

        if ((_lifeTimer -= dt) <= 0) {
            SetState(ActorState.Died);
            return;
        }
        
        if ((_headTimer -= dt) <= 0) {
            _headTimer = 2f + Random.Range(-.2f, .2f);
    
            if (TryGetEggInRange(_model.SearchRadius, out var eggPosition))
                _view.SetDestination(eggPosition);
            else
                _view.SetDestination(_map.RequestWarmThan(.25f));
        }
    }
    
    private bool TryGetEggInRange(float searchRadius, out Vector3 foodPosition) {
        var hits = Physics.OverlapSphereNonAlloc(Position, searchRadius, _cache, _model.ActorMask);
        foodPosition = Position;
        
        if (hits > 0) {
            for (var i = 0; i < hits; i++) {
                if (_cache[i].TryGetComponent<ActorView>(out var collider) && collider.Actor is IConsumableTarget) {
                    foodPosition = collider.transform.position;
                    return true;
                };
            }
        }

        return false;
    }

    public void Dispose() {
        // Clean up resources;
        _subscription.Dispose();
        OnDisposed?.Invoke(this, _view);
    }

    
}