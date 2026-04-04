using System;
using UnityEngine;
using Random = UnityEngine.Random;

public interface ICocoonModel {
    float TimeToGetNewBorn { get;  }
}

public class Cocoon : ICocoonActor {
    private readonly ICocoonModel _model;
    private readonly CocoonView _view;
    
    private ISimulationCommand _command;
    private ActorState _state;
    private float _timer;
    

    public Vector3 Position {
        get => _view.Transform.position;
        set => _view.Transform.position = value;
    }
    
    public Cocoon(ICocoonModel model, CocoonView view) {
        _model = model;
        _view = view;
        _view.Actor = this;
        _timer = _model.TimeToGetNewBorn;
    }

    public event Action<ActorStateAction> OnStateChanged;
    public event Action<IActor, ActorView> OnDisposed;

    public void OnUpdateFrame(float dt) {
        // TODO: Based on their state;
        // TODO: Evolving timer;
        // TODO: Disappear timer;
        if ((_timer -= dt) > 0) {
            // TODO: Update progress?
            return;
        }

        if (_state == ActorState.Evolution) {
            OnStateChanged?.Invoke(ActorStateAction.Evolution(this));
        }
        
        if (_state == ActorState.Died) {
            OnStateChanged?.Invoke(ActorStateAction.Died(this));
        }
    }

    public void CompleteEvolution() {
        // TODO: Set disappearing animation;
        _command.Execute();
        
        _timer = _model.TimeToGetNewBorn + Random.Range(-.25f, .25f);
        _state = ActorState.Died;
    }
    
    public void AssignChild(ISimulationCommand command) {
        // TODO: Handle split parent type;
        _command = command;
        
        _timer = _model.TimeToGetNewBorn + Random.Range(-.25f, .25f);
        _state = ActorState.Evolution;
    }
    
    public void Dispose() {
        // Properly clean up resources;
        OnDisposed?.Invoke(this, _view);
    }
}