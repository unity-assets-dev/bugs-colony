using System;
using System.Linq;
using UnityEngine;

public class SimulationController: ISimulationController {
    
    private readonly ActorContainer _container;
    private readonly ScenarioMap _map;
    private readonly ActorFactory _factory;
    private float _timer;

    public event Action<IActor> ActorDied;
    
    public SimulationController(ActorContainer container, ScenarioMap map, ActorFactory factory) {
        _container = container;
        _map = map;
        _factory = factory;
    }

    public void Startup() {
        // TODO: Place any food and first workers;
        _map.RequestPosition(-.5f, 10).EachNonAlloc(position => AddCocoon<Worker>(position));
        _map.RequestPosition(-.25f, 5).EachNonAlloc(position => AddEgg<Egg>(position));
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
    }

    public void Dispose() {
        // Free resources;
        _container.Dispose().EachNonAlloc(DisposeActor);
        _map.ResetMap();
    }
}