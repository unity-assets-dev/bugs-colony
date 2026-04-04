using BugsColony.App.Models;
using UnityEngine;

public interface ISimulationScenario {
    void Run();
    void UpdateFrame(float dt);
    void Stop();
    void Restart();
}

public interface ISimulationController {
    int CountOf<TActor>() where TActor : IActor;
    void AddCocoon<TActor>(Vector3 senderPosition) where TActor : IBugActor;
    TActor AddBug<TActor>(Vector3 position) where TActor : IBugActor;
}

public class SimulationScenario : ISimulationScenario {
    
    private readonly SimulationModel _model;
    private readonly SimulationController _controller;

    private bool _running;
    private int _insects;
    
    public SimulationScenario(SimulationModel model, SimulationController controller) {
        _model = model;
        _controller = controller;
    }
    
    public void Run() {
        _running = true;
        _controller.ActorDied += OnActorDied;
        _controller.Startup();
    }

    private void OnActorDied(IActor actor) {
        if(actor is Predator) _model.Predators.Add();
        if(actor is Worker) _model.Workers.Add();
    }

    public void UpdateFrame(float dt) {
        if (!_running) return;
        
        _controller.OnFrameUpdate(dt);
    }

    public void Restart() {
        // TODO: Cleanup and restart;
        Stop();
        Run();
    }
    
    public void Stop() {
        _controller.ActorDied -= OnActorDied;
        _running = false;
        
        _controller.Dispose();
        _model.ResetAll();
    }
}

