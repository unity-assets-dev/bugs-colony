using System;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IWorkerModel {
    float SearchRadius { get; }
    LayerMask EggMask  { get; }
}

public class Worker : IBugActor, IInteractionHandler, IConsumableTarget {
    
    private readonly Collider[] _cache = new Collider[20];
    private readonly IDisposable _triggerSubscription;
    
    private ActorState _state = ActorState.None; // move to model;
    private int _collectedFood; // move to model;

    private readonly IWorkerModel _model;
    private readonly WorkerView _view;
    private readonly IHeatMap _map;
    private float _timer;
    

    public event Action<ActorStateAction> OnStateChanged;
    public event Action<IActor, ActorView> OnDisposed;

    public Vector3 Position {
        get => _view.Transform.position;
        set => _view.Transform.position = value;
    }
    
    public Worker(IWorkerModel model, WorkerView view, IHeatMap map) {
        _model = model;
        _view = view;
        _view.Actor = this;
        _map = map;
        
        _triggerSubscription = _view.Subscribe(this);
    }

    public void SetState(ActorState state) {
        if (_state != state) {
            _state = state;
            _view.gameObject.SetActive(_state == ActorState.Alive);
            
            OnStateChanged?.Invoke(ActorStateAction.State(this, _state));
        }
    }
    
    public void ConsumeTarget(IActor actor) {
        if (actor is Egg egg) {
            _collectedFood++;
            // Notify only
            OnStateChanged?.Invoke(ActorStateAction.Consume(this, egg));
        }
    }

    public void OnConsume() => SetState(ActorState.Died);

    public void OnMutate(ISimulationController simulation) {
        if (_collectedFood >= 2) {
            _collectedFood = 0;
            
            if(Random.Range(0, 100) <= 10 && simulation.CountOf<Worker>() > 10) {
                simulation.AddCocoon<Predator>(Position);
                return;
            }
            
            simulation.AddCocoon<Worker>(Position);
        }
    }

    public void OnUpdateFrame(float dt) {
        // TODO: update model;
        if (_state != ActorState.Alive) return;
        
        if ((_timer -= dt) <= 0) {
            _timer = 2f + Random.Range(-.2f, .2f);

            if (TryGetEggInRange(_model.SearchRadius, out var eggPosition))
                _view.SetDestination(eggPosition);
            else
                _view.SetDestination(_map.RequestCoolerThan(.25f));
        }
    }

    private bool TryGetEggInRange(float searchRadius, out Vector3 eggPosition) {
        var hits = Physics.OverlapSphereNonAlloc(Position, searchRadius, _cache, _model.EggMask);
        eggPosition = Position;
        
        if (hits > 0) {
            for (var i = 0; i < hits; i++) {
                if (_cache[i].TryGetComponent(out EggView collider)) {
                    eggPosition = collider.transform.position;
                    return true;
                };
            }
        }

        return false;
    }

    public void Dispose() {
        // Clean up resources;
        _triggerSubscription.Dispose();
        OnDisposed?.Invoke(this, _view);
    }

    
}