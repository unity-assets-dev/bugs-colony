using System;
using System.Linq;
using UnityEngine;

public class SimulationController: ISimulationController {
    
    private readonly ActorContainer _container;
    private readonly CameraDrag _camera;
    private readonly ScenarioMap _map;
    private readonly ActorFactory _factory;
    private float _timer;
    private Player _player;

    public event Action<IActor> ActorDied;
    public event Action<int> WorkersCountChanged;
    public event Action<IActor> ActorKilled;
    
    public SimulationController(ActorContainer container, CameraDrag camera, ScenarioMap map, ActorFactory factory) {
        _container = container;
        _camera = camera;
        _map = map;
        _factory = factory;
    }

    public void Startup() {
        _container.ActorSpawn += OnActorWasSpawned;
        
        // TODO: Place any food and first workers;
        _map.RequestPosition(-.5f, 10).EachNonAlloc(position => AddCocoon<Worker>(position));
        _map.RequestPosition(-.25f, 5).EachNonAlloc(position => AddEgg<Egg>(position));
        
        AddCocoon<Player>(_map.RequestWarmThan(.5f));
    }

    private void OnActorWasSpawned(IActor actor) {
        if (actor is Player player) {
            _player = player;
            _camera.Follow(_player.Position);
        }

        if (actor is Worker worker) {
            WorkersCountChanged?.Invoke(_container.CountOf<Worker>());
        }
    }

    public int CountOf<TActor>() where TActor : IActor => _container.OfType<TActor>().Count();

    public TActor AddBug<TActor>(Vector3 position) where TActor : IBugActor {
        var actor = _factory.CreateActor<TActor>();
        actor.Position = position;
        actor.OnStateChanged += OnActorStateChanged;
        _container.AddActor(actor);
        
        return actor;
    }
    
    private TActor AddEgg<TActor>(Vector3 position) where TActor : IConsumableTarget {
        var actor = _factory.CreateActor<TActor>();
        actor.Position = position;
        actor.OnStateChanged += OnActorStateChanged;
        _container.AddActor(actor);
        
        return actor;
    }
    
    public void AddCocoon<TActor>(Vector3 senderPosition) where TActor : IBugActor {
        var cocoon = _factory.CreateActor<Cocoon>();
        
        cocoon.Position = senderPosition;
        cocoon.AssignChild(new SpawnCommand<TActor>(this, senderPosition));
        cocoon.OnStateChanged += OnActorStateChanged;
        
        _container.AddActor(cocoon);
    }

    private void OnActorStateChanged(ActorStateAction action) {
        switch (action.SenderState) {
            case ActorState.None: break; // TODO: Cocoon form;
            
            case ActorState.Alive: // running
                // TODO: Additional bug setup;
                break;
            
            case ActorState.Died: // check for insects count
                // TODO: birth, devoured or killed by hunger
                if (action.Sender is Egg) {
                    AddEgg<Egg>(_map.RequestWarmThan(-.5f));
                }
                
                DisposeActor(action.Sender);
                ActorDied?.Invoke(action.Sender);
                break;
            
            case ActorState.Consume:
                
                if (action.Sender is IBugActor bug) {
                    bug.OnMutate(this);
                }

                if (action.Target is IConsumableTarget target) {
                    target.OnConsume();
                }

                if (action.Sender is Player player) {
                    ActorKilled?.Invoke(action.Target);
                }
                
                break;
            
            case ActorState.Evolution:
                if (action.Sender is ICocoonActor cocoon) {
                    cocoon.CompleteEvolution();
                }
                
                break;
            
            default: throw new Exception("Missing action over actor action");
        }
    }

    private void DisposeActor(IActor actor) {
        actor.OnStateChanged -= OnActorStateChanged;
        actor.Dispose();
        _container.RemoveActor(actor);
    }

    public void OnFrameUpdate(float dt) {
        _container.OnUpdateFrame(dt);

        if ((_timer -= dt) <= 0) {
            _timer = 1f;
            var heatMap = _container.Get<IBugActor>().Select(a => a.Position).ToArray();
            _map.Heat(heatMap, .5f);
        }

        if (_player != null) {
            _camera.Follow(_player.Position);
        }
    }

    public void Dispose() {
        // Free resources;
        _container.ActorSpawn -= OnActorWasSpawned;
        _container.EachNonAlloc(DisposeActor);
        _player = null;
        _map.ResetMap();
    }
}