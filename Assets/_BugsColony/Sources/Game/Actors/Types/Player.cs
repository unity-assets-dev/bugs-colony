using System;
using UnityEngine;

public interface IPlayerModel {
    
}

public class Player : IBugActor, IConsumableTarget {
    
    private readonly IPlayerModel _model;
    private readonly PlayerView _view;
    private readonly IHeatMap _map;
    private ActorState _state;
    private readonly Plane _plane;

    public event Action<ActorStateAction> OnStateChanged;
    public event Action<IActor, ActorView> OnDisposed;
    
    public Vector3 Position {
        get => _view.Transform.position;
        set => _view.Transform.position = value;
    }

    public Player(IPlayerModel model, PlayerView view, IHeatMap map) {
        _model = model;
        _view = view;
        _map = map;
        
        _plane = new Plane(Vector3.up, Vector3.zero);
    }
    
    private Vector3 RotateToCameraRotation(Vector3 input, float cameraRotation = 45f) {
        var rotation = Quaternion.Euler(0, cameraRotation, 0);
        return rotation * input;
    }
    
    public void OnUpdateFrame(float dt) {
        if (_state != ActorState.Alive) return;

        var input = HandleInputMovement(dt);
        
        _view.Transform.Translate(RotateToCameraRotation(input));

        HandleInputRotation(dt);
    }

    private Vector3 FromProjectToPlane() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        _plane.Raycast(ray, out var hit);
        return ray.GetPoint(hit);
    }
    
    private void HandleInputRotation(float dt) {
        var mousePosition = FromProjectToPlane();
        var direction = mousePosition - _view.Transform.position;
        
        _view.RotateTo(direction, dt);
    }

    private static Vector3 HandleInputMovement(float dt) {
        var input = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ) * dt * 8;
        return input;
    }

    public void Dispose() {
        OnDisposed?.Invoke(this, _view);
    }

    public void OnConsume() {
        SetState(ActorState.Died);
    }

    public void SetState(ActorState state) {
        if (_state != state) {
            _state = state;
            _view.gameObject.SetActive(_state == ActorState.Alive);
            
            OnStateChanged?.Invoke(ActorStateAction.State(this, _state));
        }
    }

    public void OnMutate(ISimulationController simulation) {}

}