using UnityEngine;

public interface ISimulationCommand {
    void Execute();
}

public class SpawnCommand<T> : ISimulationCommand where T : IBugActor {
    private readonly ISimulationController _controller;
    private readonly Vector3 _position;

    public SpawnCommand(ISimulationController controller, Vector3 position) {
        _controller = controller;
        _position = position;
    }
    
    public void Execute() {
        var instance = _controller.AddBug<T>(_position);
        instance.SetState(ActorState.Alive);
    }
}